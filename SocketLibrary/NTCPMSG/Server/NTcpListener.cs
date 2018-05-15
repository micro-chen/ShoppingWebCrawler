/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NTCPMessage.Server
{
    public class NTcpListener
    {
        #region static public properties

        /// <summary>
        /// Get or set the capacity of send message task
        /// </summary>
        //public static int SendMessageTaskCapacity
        //{
        //    get
        //    {
        //        return _SendMessageTaskCapacity;
        //    }

        //    set
        //    {
        //        _SendMessageTaskCapacity = value;

        //        if (_SendMessageTaskCapacity <= 0)
        //        {
        //            _SendMessageTaskCapacity = 1;
        //        }
        //    }
        //}

        #endregion

        #region SendMessageTask
        readonly SendMessageTask[] _SendMessageTaskPool = null; //Send message task pool

        /// <summary>
        /// Init send message task pool
        /// </summary>
        private void InitSendMessageTaskPool()
        {
            for (int i = 0; i < _SendMessageTaskPool.Length; i++)
            {
                _SendMessageTaskPool[i] = new SendMessageTask(this);
            }

            for (int i = 0; i < _SendMessageTaskPool.Length; i++)
            {
                _SendMessageTaskPool[i].Start(i);
            }
        }

        internal SendMessageTask GetTask(SCB scb)
        {
            return _SendMessageTaskPool[scb.Id % _SendMessageTaskPool.Length];
        }


        #endregion

        #region SCB Management

        object _SCBLockObj = new object();
        Dictionary<IPEndPoint, SCB> _RemoteIPToSCB = new Dictionary<IPEndPoint, SCB>();

        void AddSCB(SCB scb)
        {
            lock (_SCBLockObj)
            {
                if (_RemoteIPToSCB.ContainsKey(scb.RemoteIPEndPoint))
                {
                    //I don't think it will happen, because IPEndPoint can't be same.
                    if (_RemoteIPToSCB[scb.RemoteIPEndPoint].WorkSocket.Connected)
                    {
                        _RemoteIPToSCB[scb.RemoteIPEndPoint].WorkSocket.Close();
                    }

                    _RemoteIPToSCB[scb.RemoteIPEndPoint] = scb;
                }
                else
                {
                    _RemoteIPToSCB.Add(scb.RemoteIPEndPoint, scb);
                }
            }
        }

        SCB GetSCB(IPEndPoint ipEndPoint)
        {
            lock (_SCBLockObj)
            {
                SCB result;

                if (_RemoteIPToSCB.TryGetValue(ipEndPoint, out result))
                {
                    return result;
                }
                else
                {
                    throw new NTcpException("Socket doesn't exist in server side", ErrorCode.SocketNotExist);
                }
            }
        }

        internal SCB[] GetAllSCB()
        {
            lock (_SCBLockObj)
            {
                SCB[] scbs = new SCB[_RemoteIPToSCB.Values.Count];
                int i = 0;

                foreach (SCB scb in _RemoteIPToSCB.Values)
                {
                    scbs[i++] = scb;
                }

                return scbs;
            }
        }

        internal void RemoteSCB(SCB scb)
        {
            lock (_SCBLockObj)
            {
                scb.Close();

                if (_RemoteIPToSCB.ContainsKey(scb.RemoteIPEndPoint))
                {
                    try
                    {
                        if (_RemoteIPToSCB[scb.RemoteIPEndPoint].WorkSocket.Connected)
                        {
                            _RemoteIPToSCB[scb.RemoteIPEndPoint].WorkSocket.Close();
                        }
                    }
                    catch
                    {
                    }

                    _RemoteIPToSCB.Remove(scb.RemoteIPEndPoint);
                }
            }
        }

        #endregion

        #region static members
        static int _SendMessageTaskCapacity = Environment.ProcessorCount;
        
        
        #endregion

        #region Fields
        const int DEFAULT_WORK_THREAD_NUM = 4;
        Socket _Server;
        readonly ReceiveMessageQueue[] _WorkThreads;

        object _ChannelSync = new object();
        UInt32 _CurChannel = int.MaxValue;

        ClientProcessorAllocator _AllocClientProcessor = new ClientProcessorAllocator();
        CableIdAllocator _CableIdAllocator = new CableIdAllocator();
        #endregion

        #region Properties
        
        /// <summary>
        ///IP End Point that be bound 
        /// </summary>
        public IPEndPoint BindIPEndPoint { get; private set; }

        /// <summary>
        /// Get The maximum length of the pending connections queue. 
        /// </summary>
        public int MaxPendingLength { get; private set; }

        #endregion

        #region private methods


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
                if (_CurChannel >= UInt32.MaxValue)
                {
                    //the value large than max value of int is reserved by server side channel.
                    _CurChannel = int.MaxValue;
                }
                else
                {
                    _CurChannel++;
                }

                return _CurChannel;
            }
        }

        private void RemoteClientPID(IPAddress ipAddress, int scbId)
        {
            _AllocClientProcessor.RemoveProcessorId(ipAddress, scbId);
        }

        private void ProcInnerMessage(Event.ReceiveEventArgs message)
        {
            try
            {
                switch ((InnerEvent)message.Event)
                {
                    case InnerEvent.GetProcessorId:
                        {
                            //Get processorid and cableid
                            //Only syncronize connection of single connection cable will
                            //Send this event to server to alloc
                            
                            //Processor mask from client
                            ulong mask = LittleEndianBitConverter.ToUInt64(message.Data, 0);

                            //Get Processor id
                            int pId = _AllocClientProcessor.GetProcessorId(((IPEndPoint)message.RemoteIPEndPoint).Address,
                                mask, message.SCBID);

                            //Get cable id
                            SCB scb = GetSCB((IPEndPoint)message.RemoteIPEndPoint);
                            UInt16 cableId = _CableIdAllocator.Alloc(scb.RemoteIPEndPoint);
                            scb.CableId = cableId;

                            message.ReturnData = new byte[sizeof(int) + sizeof(UInt16)];

                            Array.Copy(LittleEndianBitConverter.GetBytes(pId), 0,
                                message.ReturnData, 0, sizeof(int));

                            Array.Copy(LittleEndianBitConverter.GetBytes(cableId), 0,
                                message.ReturnData, sizeof(int), sizeof(UInt16));

                        }
                        break;

                }
            }
            catch (Exception e)
            {
                OnErrorEvent("ProcInnerMessage", e);
            }
        }

        private void AsyncAccept(IAsyncResult iar)
        {
            //get orginal socket we input as the argument in BeginAccept
            Socket orginalServer = (Socket)iar.AsyncState;

            //Get new socket based on orginal socket
            try
            {
                Socket workSocket = orginalServer.EndAccept(iar);

                workSocket.NoDelay = true;
                workSocket.SendBufferSize = 16 * 1024;

                try
                {
                    OnAcceptEvent(workSocket.RemoteEndPoint);
                }
                catch (Exception e)
                {
                    OnErrorEvent("OnAcceptEvent", e);
                }

                try
                {
                    SCB scb = new SCB(this, workSocket);
                    scb.OnError = OnErrorEvent;
                    //scb.OnReceive = OnReceiveEvent; //OnReceiveEvent and OnBatchReceive can't be set in same time.
                    scb.OnBatchReceive = OnBatchReceive;
                    scb.OnDisconnect = OnDisconnectEvent;
                    AddSCB(scb);
                }
                catch (Exception e)
                {
                    OnErrorEvent("Accept", e);
                }
            }
            catch (Exception e)
            {
                OnErrorEvent("Accept", e);
            }

            try
            {
                _Server.BeginAccept(new AsyncCallback(AsyncAccept), _Server);
            }
            catch (Exception e)
            {
                OnErrorEvent("BeginAccept", e);
            }
        }

        /// <summary>
        /// Inner Asend to the client specified in ipEndPoint.
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="evt"></param>
        /// <param name="cableId"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <exception cref="TcpException"></exception>
        /// <exception cref="socketException"></exception>
        private bool InnerASend(IPEndPoint ipEndPoint, MessageFlag flag, UInt32 evt, UInt16 cableId, byte[] data)
        {
            SCB scb = GetSCB(ipEndPoint);

            if (scb == null)
            {
                return false;
            }

            IncCurChannel();

            //scb.AsyncSend(flag, evt, cableId, channel, data);
            scb.ASendFromServer(flag, evt, cableId, CurChannel, data);

            return true;
            //SCB scb = _SCB;
            //scb.AsyncSend(flag, evt, cableId, channel, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="flag"></param>
        /// <param name="evt"></param>
        /// <param name="cableId"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <exception cref="TcpException"></exception>
        /// <exception cref="socketException"></exception>
        private void InnerASendResponse(IPEndPoint ipEndPoint, MessageFlag flag, UInt32 evt, UInt16 cableId, UInt32 channel, byte[] data)
        {
            SCB scb = GetSCB(ipEndPoint);
            //scb.AsyncSend(flag, evt, cableId, channel, data);
            scb.ASendFromServer(flag, evt, cableId, channel, data);
        }

        #endregion

        #region contractor
        public NTcpListener(int bindPort)
            :this(DEFAULT_WORK_THREAD_NUM, bindPort)
        {

        }


        public NTcpListener(int workThreadNum, int bindPort)
            :this(workThreadNum, new IPEndPoint(IPAddress.Any, bindPort))
        {
        }

        public NTcpListener(IPEndPoint bindIPEndPoint)
            : this(DEFAULT_WORK_THREAD_NUM, bindIPEndPoint)
        {

        }

        public NTcpListener(int workThreadNum, IPEndPoint bindIPEndPoint)
        {
            _SendMessageTaskPool = new SendMessageTask[workThreadNum];

            InitSendMessageTaskPool(); //Init server side sendmenssage pool

            MaxPendingLength = 1024;
            this.BindIPEndPoint = bindIPEndPoint;

            if (workThreadNum <= 0)
            {
                workThreadNum = 1;
            }
            
            _WorkThreads = new ReceiveMessageQueue[workThreadNum];
        }

        #endregion

        #region Events

     
        /// <summary>
        /// Event occurred when error has beed received.
        /// </summary>
        public event EventHandler<Event.ErrorEventArgs> ErrorReceived;

        private void OnErrorEvent(string func, Exception e)
        {
            EventHandler<Event.ErrorEventArgs> errorEventHandler = ErrorReceived;

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
        /// Event occurred when Remote socket accepted
        /// </summary>
        public event EventHandler<Event.AcceptEventArgs> Accepted;

        private void OnAcceptEvent(EndPoint remoteEndPoint)
        {
            EventHandler<Event.AcceptEventArgs> acceptEventHandler = Accepted;

            if (acceptEventHandler != null)
            {
                try
                {
                    acceptEventHandler(this, new Event.AcceptEventArgs(remoteEndPoint));
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
            RemoteClientPID(scb.RemoteIPEndPoint.Address, scb.Id);

            if (scb.CableId != 0)
            {
                _CableIdAllocator.Return(scb.CableId);
            }

            RemoteSCB(scb);

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
        }

        private void OnQueueMessage(Event.ReceiveEventArgs message)
        {
            EventHandler<Event.ReceiveEventArgs> receiveEventHandler = DataReceived;

            if ((message.Flag & MessageFlag.Inner) != 0)
            {
                //Inner message

                ProcInnerMessage(message);

                if ((message.Flag & MessageFlag.Sync) != 0)
                {
                    //Sync message
                    if (message.ReturnData == null)
                    {
                        message.ReturnData = new byte[0];
                    }

                    InnerASendResponse((IPEndPoint)message.RemoteIPEndPoint, MessageFlag.Sync,
                        message.Event, message.CableId, message.Channel, message.ReturnData);
                }
            }
            else if (receiveEventHandler != null)
            {
                receiveEventHandler(this, message);

                if ((message.Flag & MessageFlag.Sync) != 0)
                {
                    //Sync message
                    if (message.ReturnData == null)
                    {
                        message.ReturnData = new byte[0];
                    }

                    InnerASendResponse((IPEndPoint)message.RemoteIPEndPoint, MessageFlag.Sync,
                        message.Event, message.CableId, message.Channel, message.ReturnData);
                }
            }
        }

        /// <summary>
        /// Event occurred when data has beed received from remote socket.
        /// </summary>
        public event EventHandler<Event.ReceiveEventArgs> DataReceived;

        private void OnBatchReceive(SCB scb, List<Event.ReceiveEventArgs> argsList)
        {
            EventHandler<Event.ReceiveEventArgs> receiveEventHandler = DataReceived;

            if (receiveEventHandler != null)
            {
                _WorkThreads[scb.Id % _WorkThreads.Length].ASendMessages(argsList);
            }
        }

        private void OnReceiveEvent(SCB scb, MessageFlag flag, UInt32 evt, UInt16 cableId, 
            UInt32 channel, byte[] data)
        {
            EventHandler<Event.ReceiveEventArgs> receiveEventHandler = DataReceived;

            if (receiveEventHandler != null)
            {
                Event.ReceiveEventArgs args = new Event.ReceiveEventArgs(scb.Id, 
                    scb.RemoteIPEndPoint, flag, evt, cableId, channel, data);

                _WorkThreads[scb.Id % _WorkThreads.Length].ASendMessage(args);
            }

        }

        #endregion

        #region public methods

        /// <summary>
        /// Get all of the cable ids that are connecting to me.
        /// </summary>
        /// <returns></returns>
        public UInt16[] GetCableIds()
        {
            return _CableIdAllocator.CableIds;
        }


        /// <summary>
        /// Get all remote end points that are connecting to this listener currently.
        /// </summary>
        /// <returns></returns>
        public IPEndPoint[] GetRemoteEndPoints()
        {
            lock (_SCBLockObj)
            {
                IPEndPoint[] endPoints = new IPEndPoint[_RemoteIPToSCB.Values.Count];
                int i = 0;

                foreach (SCB scb in _RemoteIPToSCB.Values)
                {
                    endPoints[i++] = scb.RemoteIPEndPoint;
                }

                return endPoints;
            }
        }

        /// <summary>
        /// Places to listening state.
        /// </summary>
        public void Listen()
        {
            Listen(this.MaxPendingLength);
        }

        /// <summary>
        /// Places to listening state.
        /// </summary>
        /// <param name="backlog">The maximum length of the pending connections queue. </param>
        public void Listen(int backlog)
        {
            if (_Server != null)
            {
                throw new NTcpException("Already listened", ErrorCode.AlreadyListened);
            }

            this.MaxPendingLength = backlog;
            
            for(int i = 0; i < _WorkThreads.Length; i++)
            {
                if (_WorkThreads[i] != null)
                {
                    try
                    {
                        if (!_WorkThreads[i].Close(1000))
                        {
                            _WorkThreads[i].Abort();
                        }
                    }
                    catch (Exception e)
                    {
                        OnErrorEvent("Close", e);
                    }
                }

                _WorkThreads[i] = new ReceiveMessageQueue(OnQueueMessage);
                _WorkThreads[i].Start();
            }

            _Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Server.Bind(this.BindIPEndPoint);
            _Server.Listen(backlog);
            _Server.BeginAccept(new AsyncCallback(AsyncAccept), _Server);

        }


        /// <summary>
        /// Send asyncronization message
        /// </summary>
        /// <param name="ipEndPoint">ip end point of client</param>
        /// <param name="evt">event</param>
        /// <param name="data">data need to send</param>
        public bool AsyncSend(IPEndPoint ipEndPoint, UInt32 evt, byte[] data)
        {
            return AsyncSend(ipEndPoint, evt, 0, data);
        }

        /// <summary>
        /// Send asynchronous message to client that specified by cableid.
        /// </summary>
        /// <param name="cableId">calbeid</param>
        /// <param name="evt">event </param>
        /// <param name="data">data</param>
        /// <returns></returns>
        public bool AsyncSend(UInt16 cableId, UInt32 evt, byte[] data)
        {
            IPEndPoint ipEndPoint = _CableIdAllocator.Get(cableId);

            if (ipEndPoint == null)
            {
                return false;
            }
            else
            {
                return AsyncSend(ipEndPoint, evt, cableId, data);
            }
        }

        /// <summary>
        /// Send asyncronization message
        /// </summary>
        /// <param name="ipEndPoint">ip end point of client</param>
        /// <param name="evt">event</param>
        /// <param name="cableId">cableId No.</param>
        /// <param name="channel">channel no</param>
        /// <param name="data">data need to send</param>
        /// <returns>if ipendpoint doesn't connect to server, return false</returns>
        public bool AsyncSend(IPEndPoint ipEndPoint, UInt32 evt, UInt16 cableId, byte[] data)
        {
            return InnerASend(ipEndPoint, MessageFlag.None, evt, cableId, data);
        }

        /// <summary>
        /// Send syncronization message
        /// </summary>
        /// <param name="ipEndPoint">ip end point of client</param>
        /// <param name="evt">event</param>
        /// <param name="cableId">cableId No.</param>
        /// <param name="data">data need to send</param>
        /// <returns>data return from client</returns>
        private byte[] SyncSend(IPEndPoint ipEndPoint, UInt32 evt, UInt16 cableId, byte[] data)
        {
            //It is not a good design for send a synchronization message from server.
            //Easy to hang the server resource.
            //Don't want to implement it.
            throw new NotImplementedException("I will implement this function in the future");
        }

        /// <summary>
        /// Close listener
        /// </summary>
        public void Close()
        {
            for (int i = 0; i < _WorkThreads.Length; i++)
            {
                try
                {
                    if (!_WorkThreads[i].Close(1000))
                    {
                        _WorkThreads[i].Abort();
                    }
                }
                catch (Exception e)
                {
                    OnErrorEvent("Close", e);
                }
                finally
                {
                    _WorkThreads[i] = null;
                }
            }

            try
            {
                if (_Server != null)
                {
                    _Server.Close();
                }
            }
            catch (Exception e)
            {
                OnErrorEvent("Close", e);
            }
            finally
            {
                _Server = null;
            }
        }

        #endregion

    }
}
