using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NTCPMSG.Client
{
    public class NTcpConnection : IDisposable
    {
        class SyncBlock
        {
            static Queue<AutoResetEvent> _AutoEventQueue = new Queue<AutoResetEvent>();
            static object _sLockObj = new object();

            static AutoResetEvent Get()
            {
                lock (_sLockObj)
                {
                    if (_AutoEventQueue.Count == 0)
                    {
                        return new AutoResetEvent(false);
                    }
                    else
                    {
                        return _AutoEventQueue.Dequeue();
                    }
                }
            }

            static void Return(AutoResetEvent autoEvent)
            {
                lock (_sLockObj)
                {
                    _AutoEventQueue.Enqueue(autoEvent);
                }
            }

            private bool _Closed;
            internal AutoResetEvent AutoEvent;
            internal byte[] RetData;

            internal SyncBlock()
            {
                RetData = null;
                _Closed = false;
                AutoEvent = Get();
            }

            ~SyncBlock()
            {
                Close();
            }

            internal bool WaitOne(int millisecondsTimeout)
            {
                return AutoEvent.WaitOne(millisecondsTimeout);
            }

            internal void Close()
            {
                if (!_Closed)
                {
                    try
                    {
                        Return(AutoEvent);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        _Closed = true;
                    }
                }
            }
        }

        #region Fields
        SCB _SCB;
        Socket _Socket;
        SendMessageQueue _SendMessageQueue;
        bool _Connected = false;
        object _LockObj = new object();

        object _ChannelSync = new object();
        UInt32 _CurChannel = 0;


        object _SyncMessageLock = new object();
        Dictionary<UInt32, SyncBlock> _SyncMessageDict = new Dictionary<uint, SyncBlock>();
 
        #endregion

        #region Properties

        private UInt32 CurChannel
        {
            get
            {
                lock (_ChannelSync)
                {
                    return _CurChannel;
                }
            }
        }

        private UInt32 IncCurChannel()
        {
            lock (_ChannelSync)
            {
                if (_CurChannel >= int.MaxValue)
                {
                    //the value large than max value of int is reserved by server side channel.
                    _CurChannel = 0;
                }
                else
                {
                    _CurChannel++;
                }

                return _CurChannel;
            }
        }

        private void IncCurChannel(UInt32 value)
        {
            lock (_ChannelSync)
            {
                _CurChannel += value;
            }
        }


        public bool Connected
        {
            get
            {
                lock (_LockObj)
                {
                    if (_Connected)
                    {
                        if (_SendMessageQueue.Closed)
                        {
                            Disconnect();
                        }
                    }

                    return _Connected;
                }
            }

            private set
            {
                lock (_LockObj)
                {
                    _Connected = value;
                }
            }
        }

        /// <summary>
        ///IP End Point that be bound 
        /// </summary>
        public IPEndPoint BindIPEndPoint { get; private set; }

        /// <summary>
        /// Server IP end point
        /// </summary>
        public IPEndPoint RemoteIPEndPoint { get; private set; }

        #endregion

        #region private methods

        bool TryGetSyncChannel(UInt32 channel, out SyncBlock syncBlock)
        {
            lock (_SyncMessageLock)
            {
                return _SyncMessageDict.TryGetValue(channel, out syncBlock);
            }
        }

        UInt32 GetChannelForSync(out SyncBlock syncBlock)
        {
            lock (_SyncMessageLock)
            {
                UInt32 channel = IncCurChannel();

                while (_SyncMessageDict.ContainsKey(channel))
                {
                    channel = IncCurChannel();
                }

                syncBlock = new SyncBlock();

                _SyncMessageDict.Add(channel, syncBlock);

                return channel;
            }
        }

        void ReturnChannelForSync(UInt32 channel)
        {
            lock (_SyncMessageLock)
            {
                if (_SyncMessageDict.ContainsKey(channel))
                {
                    try
                    {
                        _SyncMessageDict[channel].Close();
                    }
                    catch
                    {

                    }
                    _SyncMessageDict.Remove(channel);
                }
            }
        }

        void ClearChannelForSync()
        {
            lock (_SyncMessageLock)
            {
                foreach (SyncBlock syncBlock in _SyncMessageDict.Values)
                {
                    try
                    {
                        syncBlock.Close();
                    }
                    catch
                    {
                    }
                }

                _SyncMessageDict.Clear();

            }
        }

        void OnReadyToSend(byte[] data, int length)
        {
            _SCB.ASend(data, 0, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="evt"></param>
        /// <param name="group"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <exception cref="TcpException"></exception>
        /// <exception cref="socketException"></exception>
        private void ASend(MessageFlag flag, UInt32 evt, UInt16 group, byte[] data)
        {
            if (!Connected)
            {
                throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
            }

            IncCurChannel();

            _SendMessageQueue.ASend(flag, evt, group, CurChannel, data);

            //SCB scb = _SCB;
            //scb.ASend(flag, evt, group, channel, data);
        }

        /// <summary>
        /// Send syncronization message
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="group">group no</param>
        /// <param name="data">data need to send</param>
        /// <param name="timeout">waitting timeout. In millisecond</param>
        /// <returns>data return from client</returns>
        private byte[] SSend(MessageFlag flag, UInt32 evt, UInt16 group, byte[] data, int timeout)
        {
            if (!Connected)
            {
                throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
            }

            SyncBlock syncBlock;
            UInt32 channel = GetChannelForSync(out syncBlock);

            _SendMessageQueue.ASend(flag, evt, group, channel, data);

            bool bSuccess;
            byte[] retData;

            try
            {
                bSuccess = syncBlock.WaitOne(timeout);
                if (bSuccess)
                {
                    retData = syncBlock.RetData;
                }
            }
            catch(Exception e)
            {
                throw new NTcpException(string.Format("Tcp disconnected during ssend, err:{0}",
                    e), ErrorCode.Disconnected);
            }
            finally
            {
                syncBlock.Close();
                ReturnChannelForSync(channel);
            }

            if (bSuccess)
            {
                return syncBlock.RetData;
            }
            else
            {
                throw new TimeoutException("SSend timeout!");
            }
        }


        #endregion

        #region contractor

        public NTcpConnection(string remoteIPAddress, int remotePort)
            : this(new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort))
        {

        }

        public NTcpConnection(IPAddress remoteIPAddress, int remotePort)
            : this(new IPEndPoint(remoteIPAddress, remotePort))
        {

        }

        public NTcpConnection(IPEndPoint remoteIPEndPoint)
        {
            IPAddress bindIP = IPAddress.Any;
            this.BindIPEndPoint = new IPEndPoint(bindIP, 0);
            this.RemoteIPEndPoint = remoteIPEndPoint;
        }

        public NTcpConnection(IPAddress bindIPAddress, IPEndPoint remoteIPEndPoint)
        {
            this.BindIPEndPoint = new IPEndPoint(bindIPAddress, 0);
            this.RemoteIPEndPoint = remoteIPEndPoint;
        }

        public NTcpConnection(IPAddress bindIPAddress, IPAddress remoteIPAddress, int remotePort)
            :this(bindIPAddress, new IPEndPoint(remoteIPAddress, remotePort))
        {
            
        }

        public NTcpConnection(IPAddress bindIPAddress, string remoteIPAddress, int remotePort)
            : this(bindIPAddress, new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort))
        {

        }

        #endregion

        #region Events

        public event EventHandler<Event.ErrorEventArgs> ErrorEventHandler;

        private void OnErrorEvent(string func, Exception e)
        {
            EventHandler<Event.ErrorEventArgs> errorEventHandler = ErrorEventHandler;

            if (errorEventHandler != null)
            {
                errorEventHandler(this, new NTCPMSG.Event.ErrorEventArgs(func, e));
            }
        }

        public event EventHandler<Event.ReceiveEventArgs> ReceiveEventHandler;

        private void OnReceiveEvent(SCB scb, MessageFlag flag, UInt32 evt, UInt16 group, 
            UInt32 channel, byte[] data)
        {
            SyncBlock syncBlock;
            if ((flag & MessageFlag.Sync) != 0)
            {
                if (TryGetSyncChannel(channel, out syncBlock))
                {
                    syncBlock.RetData = data;
                    syncBlock.AutoEvent.Set();
                }
            }
            else
            {
                EventHandler<Event.ReceiveEventArgs> receiveEventHandler = ReceiveEventHandler;

                if (receiveEventHandler != null)
                {
                    receiveEventHandler(this, new Event.ReceiveEventArgs(
                        scb.RemoteIPEndPoint, flag, evt, group, channel, data));
                }
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Connect to server
        /// </summary>
        public void Connect()
        {
            Connect(30);
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="timeout">connect timeout, in seconds</param>
        public void Connect(int timeout)
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Socket.Bind(this.BindIPEndPoint);
            
            _Socket.Connect(this.RemoteIPEndPoint);
            _Socket.NoDelay = true;
            _Socket.SendBufferSize = 16 * 1024;

            _SCB = new SCB(_Socket);
            _SCB.OnReceive = OnReceiveEvent;

            _SendMessageQueue = new SendMessageQueue(OnReadyToSend);

            Connected = true;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect()
        {

            try
            {
                if (_SendMessageQueue != null)
                {
                    if (!_SendMessageQueue.Closed)
                    {
                        _SendMessageQueue.Close();
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (_Socket != null)
                {
                    _Socket.Close();
                }
            }
            catch
            {

            }
            finally
            {
                _Socket = null;
                Connected = false;
            }
        }

        public void Listen(int maxConnectionNum)
        {
        }

        /// <summary>
        /// Send asyncronization message
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="data">data need to send</param>
        public void ASend(UInt32 evt, byte[] data)
        {
            ASend(evt, 0, data);
        }

        /// <summary>
        /// Send asyncronization message
        /// </summary>
        /// <param name="ipEndPoint">ip end point of client</param>        /// <param name="evt">event</param>
        /// <param name="group">group no</param>
        /// <param name="channel">channel no</param>
        /// <param name="data">data need to send</param>
        public void ASend(UInt32 evt, UInt16 group, byte[] data)
        {
            ASend(MessageFlag.None, evt, group, data);
        }

        public byte[] SSend(UInt32 evt, byte[] data)
        {
            return SSend(MessageFlag.Sync, evt, 0, data, Timeout.Infinite);
        }

        public byte[] SSend(UInt32 evt, byte[] data, int millisecondsTimeout)
        {
            return SSend(MessageFlag.Sync, evt, 0, data, millisecondsTimeout);
        }


        public byte[] SSend(UInt32 evt, UInt16 group, byte[] data, int millisecondsTimeout)
        {
            return SSend(MessageFlag.Sync, evt, group, data, millisecondsTimeout);
        }

     

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            Disconnect();
        }

        #endregion
    }
}
