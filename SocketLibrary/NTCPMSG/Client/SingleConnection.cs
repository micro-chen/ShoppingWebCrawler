
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

/*

这个类每次建立1个链接到服务器

主要的方法：

       AsyncSend： 这个方法发送异步消息到服务器

       SyncSend： 这个方法发送同步消息到服务器

       Connect： 这个方法用于和服务器建立连接时调用。这个方法有个 autoConnect 参数，如果这个参数为true，那么SingleConnection 类在和服务器断链后会尝试自动与服务器建链。这个是个比较实用的功能，调用者不需要考虑服务器连接中断后恢复的情况。

主要事件:

       ConnectedEventHandler: 这个事件在建立链接成功时触发

       ErrorEventHandler: 这个事件在发生错误的情况下触发

       RemoteDisconnected: 这个事件在和服务器断链时触发

       ReceiveEventHandler: 这个事件当接收到直接从服务器推送过来的消息时触发。
*/


namespace NTCPMessage.Client
{
    /// <summary>
    /// This class provide one tcp connection link with multiple channel.
    /// </summary>
    public class SingleConnection : INTCPConnection, IDisposable
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

            static internal void ClearAutoEvents()
            {
                lock (_sLockObj)
                {
                    while (_AutoEventQueue.Count > 0)
                    {
                        System.Threading.AutoResetEvent aEvent = _AutoEventQueue.Dequeue();
                        aEvent.Set();
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
        object _ConnectLock = new object();

        System.Threading.AutoResetEvent _ConnectEvent;
        Exception _ConnectException;
        SCB _SCB;
        Socket _Socket;
        SendMessageQueue _SendMessageQueue;
        bool _Connected;
        bool _Closed;

        object _LockObj = new object();

        object _ChannelSync;
        UInt32 _CurChannel;

        object _SyncMessageLock;
        Dictionary<UInt32, SyncBlock> _SyncMessageDict;
 
        #endregion

        #region private Properties

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

        private bool Closed
        {
            get
            {
                lock (_LockObj)
                {
                    return _Closed;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _Closed = value;
                }
            }
        }

       


        #endregion

        #region public Properties

        /// <summary>
        /// Default serializer for data
        /// </summary>
        public Serialize.ISerialize DefaultDataSerializer { get; set; }

        /// <summary>
        /// Default serializer for return value
        /// </summary>
        public Serialize.ISerialize DefaultReturnSerializer { get; set; }

        /// <summary>
        /// Get tcp connected or not.
        /// </summary>
        public bool Connected
        {
            get
            {
                lock (_LockObj)
                {
                    if (_Connected)
                    {
                        if (_SendMessageQueue != null)
                        {
                            if (_SendMessageQueue.Closed)
                            {
                                Close();
                            }
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

        private void InitVar()
        {
            _ConnectEvent = new AutoResetEvent(false);
            _ConnectException = null;
            _SCB = null;
            _Socket = null;
            _SendMessageQueue = null;
            _Connected = false;
            _Closed = false;

            _ChannelSync = new object();
            _CurChannel = 0;

            _SyncMessageLock = new object();
            _SyncMessageDict = new Dictionary<uint, SyncBlock>();
        }

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
            if (null== _SyncMessageLock||null== _SyncMessageDict)
            {
                return;
            }
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

            SyncBlock.ClearAutoEvents();
        }

        private void Async_Connection(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;

                if (socket != null)
                {
                    socket.EndConnect(ar);
                }
            }
            catch (Exception ex)
            {
                _ConnectException = ex;
            }
            finally
            {
                _ConnectEvent.Set();
            }
        }

        void OnReadyToSend(byte[] data, int length)
        {
            _SCB.AsyncSend(data, 0, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="evt"></param>
        /// <param name="cableId"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <exception cref="TcpException"></exception>
        /// <exception cref="socketException"></exception>
        private void InnerASend(MessageFlag flag, UInt32 evt, UInt16 cableId, byte[] data)
        {
            if (!Connected)
            {
                throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
            }

            IncCurChannel();

            _SendMessageQueue.AsyncSend(flag, evt, cableId, CurChannel, data);

            //SCB scb = _SCB;
            //scb.AsyncSend(flag, evt, cableId, channel, data);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="data">An array of type Byte  that contains the data to be sent. </param>
        /// <returns>An array of type Byte  that contains the data that return from remote host</returns>
        internal byte[] SyncSend(MessageFlag flag, UInt32 evt, byte[] data)
        {
            return InnerSSend(MessageFlag.Sync | flag, evt, 0, data, Timeout.Infinite);
        }

        /// <summary>
        /// Send syncronization message
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="cableId">cableId no</param>
        /// <param name="data">data need to send</param>
        /// <param name="timeout">waitting timeout. In millisecond</param>
        /// <returns>data return from client</returns>
        private byte[] InnerSSend(MessageFlag flag, UInt32 evt, UInt16 cableId, byte[] data, int timeout)
        {
            if (!Connected)
            {
                throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
            }

            SyncBlock syncBlock;
            UInt32 channel = GetChannelForSync(out syncBlock);

            _SendMessageQueue.AsyncSend(flag, evt, cableId, channel, data);

            bool bSuccess;
            byte[] retData;

            try
            {
                bSuccess = syncBlock.WaitOne(timeout);
                if (bSuccess)
                {
                    if (!Connected)
                    {
                        throw new NTcpException("Tcp disconnected during ssend", ErrorCode.Disconnected);
                    }

                    retData = syncBlock.RetData;
                }
            }
            catch (NTcpException)
            {
                throw;
            }
            catch (Exception e)
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
                throw new TimeoutException("SyncSend timeout!");
            }
        }

        internal void SetProcessorId(int processorId)
        {
            _SendMessageQueue.SetProcessorId(processorId);
        }

        #endregion

        #region contractor

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="remoteIPAddress">remote server ip address</param>
        /// <param name="remotePort">remote server tcp port</param>
        public SingleConnection(string remoteIPAddress, int remotePort)
            : this(new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort))
        {

        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="remoteIPAddress">remote server ip address</param>
        /// <param name="remotePort">remote server tcp port</param>
        public SingleConnection(IPAddress remoteIPAddress, int remotePort)
            : this(new IPEndPoint(remoteIPAddress, remotePort))
        {

        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="remoteIPEndPoint">remote server IPEndPoint</param>
        public SingleConnection(IPEndPoint remoteIPEndPoint)
        {
            IPAddress bindIP = IPAddress.Any;
            this.BindIPEndPoint = new IPEndPoint(bindIP, 0);
            this.RemoteIPEndPoint = remoteIPEndPoint;
            DefaultDataSerializer = new Serialize.BinSerializer<object>();
            DefaultReturnSerializer = new Serialize.JsonSerializer<DataContainer>();
        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="bindIPAddress">local ip address bind in this socket</param>
        /// <param name="remoteIPEndPoint">remote IPEndPoint</param>
        public SingleConnection(IPAddress bindIPAddress, IPEndPoint remoteIPEndPoint)
        {
            this.BindIPEndPoint = new IPEndPoint(bindIPAddress, 0);
            this.RemoteIPEndPoint = remoteIPEndPoint;
            DefaultDataSerializer = new Serialize.BinSerializer<object>();
            DefaultReturnSerializer = new Serialize.JsonSerializer<DataContainer>();
        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="bindIPAddress">local ip address bind in this socket</param>
        /// <param name="remoteIPAddress">remote server ip address</param>
        /// <param name="remotePort">remote server tcp port</param>
        public SingleConnection(IPAddress bindIPAddress, IPAddress remoteIPAddress, int remotePort)
            :this(bindIPAddress, new IPEndPoint(remoteIPAddress, remotePort))
        {
            
        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="bindIPAddress">local ip address bind in this socket</param>
        /// <param name="remoteIPAddress">remote server ip address</param>
        /// <param name="remotePort">remote server tcp port</param>
        public SingleConnection(IPAddress bindIPAddress, string remoteIPAddress, int remotePort)
            : this(bindIPAddress, new IPEndPoint(IPAddress.Parse(remoteIPAddress), remotePort))
        {

        }

        #endregion

        #region Events

        /// <summary>
        /// Event occurred when this cable connected.
        /// </summary>
        public event EventHandler<Event.CableConnectedEventArgs> ConnectedEventHandler;

        private void OnConnectedEvent()
        {
            EventHandler<Event.CableConnectedEventArgs> connectedEventHandler = ConnectedEventHandler;

            if (connectedEventHandler != null)
            {
                try
                {
                    connectedEventHandler(this, new NTCPMessage.Event.CableConnectedEventArgs());
                }
                catch
                {
                }
            }
        }


        /// <summary>
        /// Event occurred when some error raised during sending message.
        /// </summary>
        public event EventHandler<Event.ErrorEventArgs> ErrorEventHandler;

        private void OnErrorEvent(string func, Exception e)
        {
            EventHandler<Event.ErrorEventArgs> errorEventHandler = ErrorEventHandler;

            if (errorEventHandler != null)
            {
                try
                {
                    errorEventHandler(this, new NTCPMessage.Event.ErrorEventArgs(func, e));
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Event occurred when remote socket disconnected
        /// </summary>
        public event EventHandler<Event.DisconnectEventArgs> RemoteDisconnected;

        private void OnDisconnectEvent(SCB scb)
        {
            if (_SCB != scb)
            {
                //if connect immediately after call disconnect.
                //the scb is the last connection and ignore it.
                return;
            }

            Connected = false;

            ClearChannelForSync();

            EventHandler<Event.DisconnectEventArgs> disconnectEventHandler = RemoteDisconnected;

            if (disconnectEventHandler != null)
            {
                try
                {
                    disconnectEventHandler(this, new Event.DisconnectEventArgs(scb.RemoteIPEndPoint, scb.CableId));
                }
                catch
                {
                }
            }

            Disconnect();
        }

        /// <summary>
        /// Event occurred when data received from server.
        /// </summary>
        public event EventHandler<Event.ReceiveEventArgs> ReceiveEventHandler;

        private void OnReceiveEvent(SCB scb, MessageFlag flag, UInt32 evt, UInt16 cableId, 
            UInt32 channel, byte[] data)
        {
            SyncBlock syncBlock;
            if ((flag & MessageFlag.Sync) != 0)
            {
                if (channel > int.MaxValue)
                {
                    //server side ssend

                    EventHandler<Event.ReceiveEventArgs> receiveEventHandler = ReceiveEventHandler;

                    if (receiveEventHandler != null)
                    {
                        try
                        {
                            Event.ReceiveEventArgs args = new Event.ReceiveEventArgs(scb.Id,
                                scb.RemoteIPEndPoint, flag, evt, cableId, channel, data);

                            receiveEventHandler(this, args);

                            InnerASend(flag, evt, cableId, args.ReturnData);
                        }
                        catch
                        {
                        }
                    }


                }
                else if (TryGetSyncChannel(channel, out syncBlock))
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
                    try
                    {
                        receiveEventHandler(this, new Event.ReceiveEventArgs(scb.Id,
                            scb.RemoteIPEndPoint, flag, evt, cableId, channel, data));
                    }
                    catch
                    {
                    }
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
            Connect(30000); //Default timeout is 30 seconds
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="millisecondsTimeout">connect timeout, in millisecond</param>
        public void Connect(int millisecondsTimeout)
        {
            Connect(millisecondsTimeout, false);
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="millisecondsTimeout">connect timeout, in millisecond</param>
        /// <param name="setThreadAffinityMask">need set the thread affinity mask</param>
        public void Connect(int millisecondsTimeout, bool setThreadAffinityMask)
        {
            lock (_ConnectLock)
            {
                if (Connected)
                {
                    return;
                }
                else
                {
                    Disconnect();
                }

                if (millisecondsTimeout < 0)
                {
                    throw new ArgumentException("milliseconds can't be negative");
                }

                InitVar();

                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _Socket.Bind(this.BindIPEndPoint);

                _Socket.BeginConnect(this.RemoteIPEndPoint, Async_Connection, _Socket);
                if (!_ConnectEvent.WaitOne(millisecondsTimeout))
                {
                    Disconnect();
                    throw new NTcpException(string.Format("Try to connect to remote server {0} timeout", this.RemoteIPEndPoint), 
                        ErrorCode.ConnectTimeout);
                }
                else if (_ConnectException != null)
                {
                    Disconnect();
                    throw _ConnectException;
                }
                
                _Socket.NoDelay = true;
                _Socket.SendBufferSize = 16 * 1024;

                _SCB = new SCB(_Socket);
                _SCB.OnReceive = OnReceiveEvent;
                _SCB.OnError = OnErrorEvent;
                _SCB.OnDisconnect = OnDisconnectEvent;

                _SendMessageQueue = new SendMessageQueue(OnReadyToSend, setThreadAffinityMask);

                Connected = true;

                this.OnConnectedEvent();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            try
            {
                Connected = false;

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
                ClearChannelForSync();
            }
            catch
            {

            }

            try
            {
                if (_SendMessageQueue != null)
                {
                    try
                    {
                        if (!_SendMessageQueue.Join(10000))
                        {
                            _SendMessageQueue.Abort();
                        }
                    }
                    catch
                    {
                        _SendMessageQueue.Abort();
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
                _SendMessageQueue = null;
                _Socket = null;
            }
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            if (Closed)
            {
                return;
            }

            try
            {
                Disconnect();
            }
            catch
            {

            }
            finally
            {
                Closed = true;
            }
        }

        /// <summary>
        /// Send asyncronization message as object
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="obj">object need to send</param>
        /// <param name="serializer">serializer</param>
        public void AsyncSend<T>(UInt32 evt, ref T obj, ISerialize<T> serializer)
        {
            AsyncSend(evt, 0, serializer.GetBytes(ref obj));
        }

        /// <summary>
        /// Send asyncronization message as object
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="obj">object need to send</param>
        /// <param name="serializer">serializer</param>
        public void AsyncSend(UInt32 evt, object obj, ISerialize serializer)
        {
            AsyncSend(evt, 0, serializer.GetBytes(obj));
        }

        /// <summary>
        /// Send asyncronization message as object
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="obj">object need to send</param>
        public void AsyncSend(UInt32 evt, object obj)
        {
            AsyncSend(evt, obj, DefaultDataSerializer);
        }

        /// <summary>
        /// Send asyncronization message
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="data">data need to send</param>
        public void AsyncSend(UInt32 evt, byte[] data)
        {
            AsyncSend(evt, 0, data);
        }

        /// <summary>
        /// Send asyncronization message
        /// </summary>
        /// <param name="ipEndPoint">ip end point of client</param>        
        /// <param name="evt">event</param>
        /// <param name="cableId">cableId No.</param>
        /// <param name="channel">channel no</param>
        /// <param name="data">data need to send</param>
        internal void AsyncSend(UInt32 evt, UInt16 cableId, byte[] data)
        {
            InnerASend(MessageFlag.None, evt, cableId, data);
        }


        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint using Default serializer
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="obj">object need to be sent</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <param name="dataSerializer">serializer for data</param>
        /// <param name="returnSerializer">serilaizer for return data</param>
        /// <returns>R data type that contains the data that return from remote host</returns>
        public R SyncSend<T, R>(UInt32 evt, ref T obj, int millisecondsTimeout, ISerialize<T> dataSerializer, ISerialize<R> returnSerializer)
        {
            byte[] ret = SyncSend(evt, 0, dataSerializer.GetBytes(ref obj), millisecondsTimeout);

            return returnSerializer.GetObject(ret);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint using Default serializer
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="obj">object need to be sent</param>
        /// <returns>object that contains the data that return from remote host</returns>
        public IDataContainer SyncSend(UInt32 evt, object obj)
        {
            byte[] ret = SyncSend(evt, 0, DefaultDataSerializer.GetBytes(obj), Timeout.Infinite);

            return (IDataContainer)DefaultReturnSerializer.GetObject(ret);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint using Default serializer
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="obj">object need to be sent</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <returns>object that contains the data that return from remote host</returns>
        public IDataContainer SyncSend(UInt32 evt, object obj, int millisecondsTimeout)
        {
            byte[] ret = SyncSend(evt, 0, DefaultDataSerializer.GetBytes(obj), millisecondsTimeout);

            return (IDataContainer)DefaultReturnSerializer.GetObject(ret);
        }


        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="obj">object need to be sent</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <param name="serializer">serializer for data</param>
        /// <returns>object that contains the data that return from remote host</returns>
        public IDataContainer SyncSend(UInt32 evt, object obj, int millisecondsTimeout, ISerialize serializer)
        {
            byte[] ret = SyncSend(evt, 0, serializer.GetBytes(obj), millisecondsTimeout);

            return (IDataContainer)DefaultReturnSerializer.GetObject(ret);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="obj">object need to be sent</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <param name="dataSerializer">serializer for data</param>
        /// <param name="returnSerializer">serilaizer for return data</param>
        /// <returns>object that contains the data that return from remote host</returns>
        public IDataContainer SyncSend(UInt32 evt, object obj, int millisecondsTimeout, ISerialize dataSerializer, ISerialize returnSerializer)
        {
            byte[] ret = SyncSend(evt, 0, dataSerializer.GetBytes(obj), millisecondsTimeout);

            return (IDataContainer)returnSerializer.GetObject(ret);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="cableId">cableId No.</param>
        /// <param name="obj">object need to be sent</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <param name="dataSerializer">serializer for data</param>
        /// <param name="returnSerializer">serilaizer for return data</param>
        /// <returns>object that contains the data that return from remote host</returns>
        internal object SyncSend(UInt32 evt, UInt16 cableId, object obj, int millisecondsTimeout, ISerialize dataSerializer, ISerialize returnSerializer)
        {
            byte[] ret = SyncSend(evt, cableId, dataSerializer.GetBytes(obj), millisecondsTimeout);

            return returnSerializer.GetObject(ret);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="data">An array of type Byte  that contains the data to be sent. </param>
        /// <returns>An array of type Byte  that contains the data that return from remote host</returns>
        public byte[] SyncSend(UInt32 evt, byte[] data)
        {
            return InnerSSend(MessageFlag.Sync, evt, 0, data, Timeout.Infinite);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="data">An array of type Byte  that contains the data to be sent. </param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <returns>An array of type Byte  that contains the data that return from remote host</returns>
        public byte[] SyncSend(UInt32 evt, byte[] data, int millisecondsTimeout)
        {
            return InnerSSend(MessageFlag.Sync, evt, 0, data, millisecondsTimeout);
        }


        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="cableId">cableId No.</param>
        /// <param name="data">An array of type Byte  that contains the data to be sent. </param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <returns>An array of type Byte  that contains the data that return from remote host</returns>
        internal byte[] SyncSend(UInt32 evt, UInt16 cableId, byte[] data, int millisecondsTimeout)
        {
            return InnerSSend(MessageFlag.Sync, evt, cableId, data, millisecondsTimeout);
        }

     

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
