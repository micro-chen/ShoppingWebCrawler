using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

using NTCPMessage.Client;
using NTCPMessage.Event;

using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

/*
这个类可以将多个链接绑定到一起和服务器通讯。有用TCP本身是基于滑动窗口协议的，
单链路的发送速度受网络时延的影响，这也是TCP比UDP慢的一个原因。但由于TCP可以提供比UDP可靠的通讯，
大部分对可靠性要求较高的系统还是要采用TCP方式发送消息。为了克服网络时延带来的性能下降，
通过多链路同时发送是一个很好的解决方案，因为每个链路都独立维护一个窗口，链路之间是并行发送。
实测下来，绑定多链路也确实比单链路的发送速度快很多。所以我一般推荐用 SingleConnectionCable
这个类连接服务器。
SingleConnectionCable 和 SingleConnection 的主要方法和事件是相同的，这里就不重复介绍了。
*/
namespace NTCPMessage.Client
{
    /// <summary>
    /// This class bind multiple single tcp connection as one 
    /// logic connection cable.
    /// </summary>
    public class SingleConnectionCable : IDisposable
    {
        #region Fields

        SingleConnection _SyncConnection;
        LinkedList<SingleConnection> _WorkingAsyncConnections;
        Queue<SingleConnection> _PendingAsyncConnections;
        LinkedListNode<SingleConnection> _CurrentWorkingConnection;

        int _ASendCount = 0;

        object _LockObj = new object();
        int _Capacity;
        int _TryConnectInterval = 1000; //in milliseconds. default is 1 second
        bool _AutoConnect;
        bool _TryToConnect = false;
        bool _Closing = false;
        UInt16 _CableId = 0;

        System.Threading.Thread _ConnectThread = null;

        private bool Closing
        {
            get
            {
                lock (_LockObj)
                {
                    return _Closing;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _Closing = value;
                }
            }
        }

        private bool TryToConnect
        {
            get
            {
                lock (_LockObj)
                {
                    return _TryToConnect;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _TryToConnect = value;
                }
            }
        }

        #endregion

        #region public properties 
        /// <summary>
        /// Default serializer for data
        /// </summary>
        public Serialize.ISerialize DefaultDataSerializer { get; set; }

        /// <summary>
        /// Default serializer for return value
        /// </summary>
        public Serialize.ISerialize DefaultReturnSerializer { get; set; }


        /// <summary>
        /// Get cable id
        /// </summary>
        public UInt16 CableId
        {
            get
            {
                lock (_LockObj)
                {
                    return _CableId;
                }
            }

            private set
            {
                lock (_LockObj)
                {
                    _CableId = value;
                }
            }
        }


        /// <summary>
        /// Get the capacity of the single connections inside this cable. 
        /// </summary>
        public int Capacity
        {
            get
            {
                lock (_LockObj)
                {
                    return _Capacity;
                }
            }

            private set
            {
                lock (_LockObj)
                {
                    _Capacity = value;
                }
            }
        }

        /// <summary>
        /// Interval for try to connect to remote host.
        /// In milliseconds
        /// </summary>
        public int TryConnectInterval
        {
            get
            {
                lock (_LockObj)
                {
                    return _TryConnectInterval;
                }
            }

            private set
            {
                lock (_LockObj)
                {
                    if (value <= 100)
                    {
                        _TryConnectInterval = 100;
                    }
                    else
                    {
                        _TryConnectInterval = value;
                    }
                }
            }
        }

        /// <summary>
        /// Get or set do connect to remote server automatically or not.
        /// If set to true, this class will try to connect to server automatically
        /// after disconnect.
        /// </summary>
        public bool AutoConnect
        {
            get
            {
                lock (_LockObj)
                {
                    return _AutoConnect;
                }
            }

            private set
            {
                lock (_LockObj)
                {
                    _AutoConnect = value;
                }
            }
        }

        /// <summary>
        /// Server IP end point
        /// </summary>
        public IPEndPoint RemoteIPEndPoint { get; private set; }

        /// <summary>
        /// Get current connection is connected or not.
        /// True: at least one single connection is connected.
        /// </summary>
        public bool Connected
        {
            get
            {
                lock (_LockObj)
                {
                    return _SyncConnection.Connected;
                }
            }
        }

        #endregion

        #region Constractor

        public SingleConnectionCable(IPEndPoint remoteIPEndPoint)
            :this(remoteIPEndPoint, 6)
        {

        }

        public SingleConnectionCable(IPEndPoint remoteIPEndPoint, int capacity)
        {
            RemoteIPEndPoint = remoteIPEndPoint;

            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be large than 0");
            }

            Capacity = capacity;

            _WorkingAsyncConnections = new LinkedList<SingleConnection>();
            _PendingAsyncConnections = new Queue<SingleConnection>();
            _CurrentWorkingConnection = null;

            for (int i = 1; i < capacity; i++)
            {
                SingleConnection conn = new SingleConnection(remoteIPEndPoint);

                conn.ErrorEventHandler += InnerErrorEventHandler;

                conn.ReceiveEventHandler += InnerReceiveEventHandler;

                conn.RemoteDisconnected += InnerRemoteDisconnected;

                _PendingAsyncConnections.Enqueue(conn);
            }

            DefaultDataSerializer = new Serialize.BinSerializer<object>();
            DefaultReturnSerializer = new Serialize.JsonSerializer<DataResultContainer<object>>();

            _SyncConnection = new SingleConnection(remoteIPEndPoint);

            _SyncConnection.ErrorEventHandler += InnerErrorEventHandler;

            _SyncConnection.ReceiveEventHandler += InnerReceiveEventHandler;

            _SyncConnection.RemoteDisconnected += InnerRemoteDisconnected;

            _ConnectThread = new System.Threading.Thread(ConnectThreadProc);
            _ConnectThread.IsBackground = true;
            _ConnectThread.Start();
        }

        ~SingleConnectionCable()
        {
            Dispose();
        }

        #endregion

        #region Private methods

        private void ConnectThreadProc()
        {
            while (true)
            {
                if (Closing)
                {
                    return;
                }

                if (AutoConnect)
                {
                    InnerConnect(30 * 1000);
                }

                System.Threading.Thread.Sleep(TryConnectInterval);
            }
        }


        private SingleConnection GetAWorkingConnection()
        {
            lock (_LockObj)
            {
                if (_WorkingAsyncConnections.Count <= 0)
                {
                    if (_SyncConnection.Connected)
                    {
                        return _SyncConnection;
                    }
                    else
                    {
                        return null;
                    }
                }

                LinkedListNode<SingleConnection> cur;

                if (_CurrentWorkingConnection == null)
                {
                    _CurrentWorkingConnection = _WorkingAsyncConnections.First;
                    cur = _CurrentWorkingConnection;
                }
                else
                {
                    int sendCount;

                    sendCount = System.Threading.Interlocked.Increment(ref _ASendCount);

                    if (sendCount % 100 != 0)
                    {
                        if (_CurrentWorkingConnection.Value.Connected)
                        {
                            return _CurrentWorkingConnection.Value;
                        }
                    }

                    cur = _CurrentWorkingConnection.Next;

                    if (cur == null)
                    {
                        cur = _WorkingAsyncConnections.First;
                    }
                }

                while (!cur.Value.Connected && _WorkingAsyncConnections.Count > 0)
                {
                    LinkedListNode<SingleConnection> next = cur.Next;
                    
                    _PendingAsyncConnections.Enqueue(cur.Value);

                    _WorkingAsyncConnections.Remove(cur);

                    if (next == null)
                    {
                        next = _WorkingAsyncConnections.First;

                        if (next == null)
                        {
                            break;
                        }
                    }

                    cur = next;
                }

                if (_WorkingAsyncConnections.Count <= 0)
                {
                    _CurrentWorkingConnection = null;

                    OnDisconnectEvent();

                    return null;
                }
                else
                {
                    _CurrentWorkingConnection = cur;
                    return cur.Value;
                }
            }
        }

        private void InnerConnect(int millisecondsTimeout)
        {
            if (Closing)
            {
                throw new NTcpException("Can't operate SingleConnectionCable when it is closing.", ErrorCode.Closing);
            }

            if (TryToConnect)
            {
                throw new NTcpException("Try to connect by other tread now.", ErrorCode.TryToConenct);
            }

            try
            {
                try
                {
                    if (!_SyncConnection.Connected)
                    {
                        TryToConnect = true;

                        CableId = 0;
                        _SyncConnection.Connect(millisecondsTimeout, true);

                        ulong processAffinity = (ulong)System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity;

                        byte[] ret = _SyncConnection.SyncSend(MessageFlag.Inner, (uint)InnerEvent.GetProcessorId,
                            LittleEndianBitConverter.GetBytes(processAffinity));

                        int processorId = LittleEndianBitConverter.ToInt32(ret, 0);
                        CableId = LittleEndianBitConverter.ToUInt16(ret, sizeof(int));
                        _SyncConnection.SetProcessorId(processorId);

                        OnConnectedEvent();
                    }
                }
                catch (Exception e)
                {
                    OnErrorEvent("InnerConnect", e);

                    return;
                }

                if (Capacity == 1)
                {
                    return;
                }

                while (true)
                {
                    SingleConnection pendingConn;

                    lock (_LockObj)
                    {
                        if (_PendingAsyncConnections.Count <= 0)
                        {
                            return;
                        }

                        pendingConn = _PendingAsyncConnections.Dequeue();
                    }

                    try
                    {
                        TryToConnect = true;

                        pendingConn.Connect(millisecondsTimeout);

                        lock (_LockObj)
                        {
                            _WorkingAsyncConnections.AddLast(pendingConn);
                        }
                    }
                    catch (Exception e)
                    {
                        _PendingAsyncConnections.Enqueue(pendingConn);

                        OnErrorEvent("InnerConnect", e);

                        return;
                    }
                }
            }
            finally
            {
                TryToConnect = false;
            }
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
        private void InnerASend(UInt32 evt, byte[] data)
        {
            if (Closing)
            {
                throw new NTcpException("Can't operate SingleConnectionCable when it is closing.", ErrorCode.Closing);
            }

            SingleConnection singleConn = GetAWorkingConnection();

            if (singleConn == null)
            {
                throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
            }


            while (CableId == 0)
            {
                System.Threading.Thread.Sleep(1);

                if (!singleConn.Connected)
                {
                    throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
                }
            }

            singleConn.AsyncSend(evt, CableId, data);
        }

        /// <summary>
        /// Send syncronization message
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="cableId">cableId no</param>
        /// <param name="data">data need to send</param>
        /// <param name="timeout">waitting timeout. In millisecond</param>
        /// <returns>data return from client</returns>
        private byte[] InnerSSend(UInt32 evt, byte[] data, int timeout)
        {
            if (Closing)
            {
                throw new NTcpException("Can't operate SingleConnectionCable when it is closing.", ErrorCode.Closing);
            }

            SingleConnection singleConn = _SyncConnection;

            if (!singleConn.Connected)
            {
                throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
            }

            int millisecondsRemain = timeout;

            while (CableId == 0)
            {
                System.Threading.Thread.Sleep(10);

                if (!singleConn.Connected)
                {
                    throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
                }

                if (timeout != System.Threading.Timeout.Infinite)
                {
                    millisecondsRemain -= 10;

                    if (millisecondsRemain <= 0)
                    {
                        throw new NTcpException("Tcp is establishing.", ErrorCode.Disconnected);
                    }
                }
            }

            return singleConn.SyncSend(evt, CableId, data, timeout);
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
                    connectedEventHandler(this, new CableConnectedEventArgs());
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

        private void InnerErrorEventHandler(object sender, ErrorEventArgs args)
        {
            OnErrorEvent(args.Func, args.ErrorException);
        }

        private void InnerRemoteDisconnected(object sender, DisconnectEventArgs args)
        {
            GetAWorkingConnection();

            if (Capacity == 1)
            {
                OnDisconnectEvent();
            }
        }

        private void OnDisconnectEvent()
        {
            EventHandler<Event.DisconnectEventArgs> disconnectEventHandler = RemoteDisconnected;

            if (disconnectEventHandler != null)
            {
                try
                {
                    disconnectEventHandler(this, new Event.DisconnectEventArgs(RemoteIPEndPoint, CableId));
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Event occurred when data received from server.
        /// </summary>
        public event EventHandler<Event.ReceiveEventArgs> ReceiveEventHandler;
        
        private void InnerReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            EventHandler<Event.ReceiveEventArgs> receiveEventHandler = ReceiveEventHandler;

            if (receiveEventHandler != null)
            {
                try
                {
                    receiveEventHandler(this, args);
                }
                catch
                {

                }
            }
        }

        #endregion


        #region Public methods

        /// <summary>
        /// Connect to remote host specified in RemoteIPEndPoint
        /// </summary>
        public void Connect()
        {
            Connect(30 * 1000);
        }

        /// <summary>
        /// Connect to remote host specified in RemoteIPEndPoint
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        public void Connect(int millisecondsTimeout)
        {
            Connect(millisecondsTimeout, true);
        }

        /// <summary>
        /// Connect to remote host specified in RemoteIPEndPoint
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely. </param>
        /// <param name="autoConnect">set the AutoConnect Mode</param>
        public void Connect(int millisecondsTimeout, bool autoConnect)
        {
            AutoConnect = autoConnect;

            if (!AutoConnect)
            {
                InnerConnect(millisecondsTimeout);
            }
            else
            {
                int times = 0;

                while (++times <= millisecondsTimeout / 100)
                {
                    if (Connected)
                    {
                        return;
                    }

                    System.Threading.Thread.Sleep(100);
                }

                throw new NTcpException("Tcp disconnected", ErrorCode.Disconnected);
            }
        }

        /// <summary>
        /// Disconnect all of the SingleConnections including in this cable.
        /// </summary>
        /// <remarks>If AutoConnect = true, will throw a exception</remarks>
        public void Disconnect()
        {
            if (AutoConnect)
            {
                throw new NTcpException("Can't disconnect in AutoConnect Mode. Need set AutoConnect to false.",
                     ErrorCode.AutoConnect);
            }

            bool connected = this.Connected;

            _SyncConnection.Disconnect();

            lock (_LockObj)
            {

                foreach (SingleConnection conn in _WorkingAsyncConnections)
                {
                    conn.Disconnect();

                    _PendingAsyncConnections.Enqueue(conn);
                }

                _WorkingAsyncConnections.Clear();
                _CurrentWorkingConnection = null;

                if (connected)
                {
                    OnDisconnectEvent();
                }
            }
        }

        public void Close()
        {
            if (_ConnectThread != null)
            {
                try
                {
                    Closing = true;
                    if (!_ConnectThread.Join(1000))
                    {
                        _ConnectThread.Abort();
                    }
                }
                catch
                {
                }
                finally
                {
                    _ConnectThread = null;
                }
            }

            try
            {
                AutoConnect = false;
                Disconnect();
            }
            catch
            {
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
            AsyncSend(evt, serializer.GetBytes(ref obj));
        }

        /// <summary>
        /// Send asyncronization message as object
        /// </summary>
        /// <param name="evt">event</param>
        /// <param name="obj">object need to send</param>
        /// <param name="serializer">serializer</param>
        public void AsyncSend(UInt32 evt, object obj, ISerialize serializer)
        {
            AsyncSend(evt, serializer.GetBytes(obj));
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
            InnerASend(evt, data);
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
            byte[] ret = SyncSend(evt, dataSerializer.GetBytes(ref obj), millisecondsTimeout);

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
            byte[] ret = SyncSend(evt, DefaultDataSerializer.GetBytes(obj), Timeout.Infinite);

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
            byte[] ret = SyncSend(evt, DefaultDataSerializer.GetBytes(obj), millisecondsTimeout);

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
            byte[] ret = SyncSend(evt, serializer.GetBytes(obj), millisecondsTimeout);

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
            byte[] ret = SyncSend(evt, dataSerializer.GetBytes(obj), millisecondsTimeout);

            return (IDataContainer)returnSerializer.GetObject(ret);
        }

        /// <summary>
        /// Synchronously sends data to the remote host specified in the RemoteIPEndPoint
        /// </summary>
        /// <param name="evt">message event</param>
        /// <param name="data">An array of type Byte  that contains the data to be sent. </param>
        /// <returns>An array of type Byte  that contains the data that return from remote host</returns>
        public byte[] SyncSend(UInt32 evt, byte[] data)
        {
            return InnerSSend(evt, data, System.Threading.Timeout.Infinite);
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
            return InnerSSend(evt, data, millisecondsTimeout);
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
