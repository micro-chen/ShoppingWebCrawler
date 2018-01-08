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

namespace NTCPMessage
{
    /// <summary>
    /// Socket control block.
    /// </summary>
    /// <remarks>
    /// Message Head: Sync Flag Event     CableId Channel  Length
    ///               A5A5 00   00000000  0000  00000000 000000
    /// Max message data length is 16MB
    /// </remarks>
    class SCB
    {
        enum State
        {
            Sync0,
            Sync1,
            Flag,
            Event0,
            Event1,
            Event2,
            Event3,

            CableId0,
            CableId1,
            Channel0,
            Channel1,
            Channel2,
            Channel3,

            Length0,
            Length1,
            Length2,
            Data,
        }

        const int MStreamCapacity = 2048;
        const int MaxDataLength = 256 * 256 * 256;
        const int ThreeBytes = 256 * 256 * 256;

        static int _SCB_ID;
        static object _SCBID_LOCK = new object();

        #region Fields

        internal int Id { get; private set; }
        internal UInt16 _CableId = 0;
        private object _SendLockObj = new object();
        private byte[] _Buffer = new byte[4 * 1024];
        private byte[] _MSGHead = new byte[16];

        private MessageFlag _CurFlag;
        private UInt32 _CurEvent;
        private int _CurLength;
        private UInt16 _CurCableId;
        private UInt32 _CurChannel;
        private byte[] _CurData;
        private int _CurDataOffset;
        private State _CurState;

        private bool _Closed = false;

        private object _BufferLockObj = new object();
        private long _BufferLength = 0;

        private System.IO.MemoryStream _MStream;
        private object _QueueLockObj = new object();
        private Queue<Message> _Queue = new Queue<Message>();
        private Server.NTcpListener _Listener; //for tcp listenser in server side. If in client side, is null.

        private long BufferLength
        {
            get
            {
                lock (_BufferLockObj)
                {
                    return _BufferLength;
                }
            }
        }

        private void IncBufferLength(long value)
        {
            _BufferLength += value;
        }


        #endregion

        #region Properties
        internal Event.Delegates.DeleOnError OnError { get; set; }

        internal Event.Delegates.DeleOnInnerBatchReceive OnBatchReceive { get; set; }

        internal Event.Delegates.DeleOnInnerReceive OnReceive { get; set; }

        internal Event.Delegates.DeleOnDisconnect OnDisconnect { get; set; }

        internal Socket WorkSocket { get; set; }

        internal IPEndPoint RemoteIPEndPoint { get; private set; }

        internal UInt16 CableId
        {
            get
            {
                lock (_SCBID_LOCK)
                {
                    return _CableId;
                }
            }

            set
            {
                lock (_SCBID_LOCK)
                {
                    _CableId = value;
                }
            }
        }

        internal bool SocketConnected
        {
            get
            {
                try
                {
                    bool part1 = this.WorkSocket.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (this.WorkSocket.Available == 0);
                    if (part1 & part2)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (SocketException)
                {
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
                catch (Exception e)
                {
                    OnError("SocketConnected", e);
                    return false;
                }
            }

        }


        #endregion

        #region Private methods
        private State NextState(byte b)
        {
            switch (_CurState)
            {
                case State.Sync0:
                    if (b != 0xA5)
                    {
                        return State.Sync0;
                    }
                    else
                    {
                        return State.Sync1;
                    }
                case State.Sync1:
                    if (b != 0xA5)
                    {
                        return State.Sync0;
                    }
                    else
                    {
                        return State.Flag;
                    }
                case State.Flag:
                    _CurFlag = (MessageFlag)b;
                    _CurEvent = 0;
                    _CurCableId = 0;
                    _CurChannel = 0;
                    return State.Event0;
                case State.Event0:
                    _CurEvent = (UInt32)(b * ThreeBytes);
                    return State.Event1;
                case State.Event1:
                    _CurEvent += (UInt32)(b * 65536);
                    _CurLength = 0;
                    return State.Event2;
                case State.Event2:
                    _CurEvent += (UInt32)(b * 256);
                    _CurLength = 0;
                    return State.Event3;
                case State.Event3:
                    _CurEvent += b;
                    _CurLength = 0;
                    return State.CableId0;
                case State.CableId0:
                    _CurCableId = (UInt16)(b * 256);
                    return State.CableId1;
                case State.CableId1:
                    _CurCableId += b;
                    return State.Channel0;
                case State.Channel0:
                    _CurChannel = (UInt32)(b * ThreeBytes);
                    return State.Channel1;
                case State.Channel1:
                    _CurChannel += (UInt32)(b * 65536);
                    return State.Channel2;
                case State.Channel2:
                    _CurChannel += (UInt32)(b * 256);
                    return State.Channel3;
                case State.Channel3:
                    _CurChannel += b;
                    return State.Length0;
                case State.Length0:
                    _CurLength = b * 65536;
                    return State.Length1;
                case State.Length1:
                    _CurLength += b * 256;
                    return State.Length2;
                case State.Length2:
                    _CurLength += b;
                    _CurData = new byte[_CurLength];
                    _CurDataOffset = 0;
                    return State.Data;
                default:
                    return State.Sync0;

            }
        }

        private void Async_Receive(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            int read = 0;
            try
            {
                read = s.EndReceive(ar);

                if (read == 0)
                {
                    //Remote connection call shut down.
                    if (!SocketConnected)
                    {
                        //Disconnected
                        OnDisconnect(this);
                    }
                    else
                    {
                        try
                        {
                            WorkSocket.Close();
                        }
                        catch
                        {
                        }

                        OnDisconnect(this);

                        WriteError("EndReceive", new Exception("Remote shut down"));
                    }

                    return;
                }
            }
            catch (Exception e)
            {
                if (!SocketConnected)
                {
                    //Disconnected
                    OnDisconnect(this);
                }
                else
                {
                    WriteError("EndReceive", e);
                }

                return;
            }

            int offset = 0;

            List<Event.ReceiveEventArgs> recvArgsList = null;

            if (OnBatchReceive != null)
            {
                recvArgsList = new List<NTCPMessage.Event.ReceiveEventArgs>();
            }

            while (offset < read)
            {
                try
                {
                    if (_CurState == State.Data)
                    {
                        int copyLen = Math.Min(read - offset, _CurLength - _CurDataOffset);
                        Array.Copy(_Buffer, offset, _CurData, _CurDataOffset, copyLen);
                        offset += copyLen;

                        _CurDataOffset += copyLen;

                        if (_CurDataOffset >= _CurData.Length)
                        {
                            if (OnBatchReceive != null&&null!= recvArgsList)
                            {
                                recvArgsList.Add(new NTCPMessage.Event.ReceiveEventArgs(
                                    this.Id, this.RemoteIPEndPoint, _CurFlag, _CurEvent, _CurCableId, _CurChannel, _CurData));
                            }

                            if (OnReceive != null)
                            {

                                OnReceive(this, _CurFlag, _CurEvent, _CurCableId, _CurChannel, _CurData);

                            }

                            _CurState = State.Sync0;
                        }

                        if (offset >= read)
                        {
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    WriteError("OnReceive", e);
                }

                _CurState = NextState(_Buffer[offset++]);
            }

            if (_CurState == State.Data && _CurLength == 0)
            {
                if (_CurDataOffset >= _CurData.Length)
                {
                    if (OnBatchReceive != null)
                    {
                        recvArgsList.Add(new NTCPMessage.Event.ReceiveEventArgs(
                            this.Id, this.RemoteIPEndPoint, _CurFlag, _CurEvent, _CurCableId, _CurChannel, _CurData));
                    }

                    if (OnReceive != null)
                    {
                        try
                        {
                            OnReceive(this, _CurFlag, _CurEvent, _CurCableId, _CurChannel, new byte[0]);
                        }
                        catch (Exception e)
                        {
                            WriteError("OnReceive", e);
                        }
                    }

                    _CurState = State.Sync0;
                }
            }

            if (OnBatchReceive != null)
            {
                OnBatchReceive(this, recvArgsList);
            }

            if (read > 0)
            {
                try
                {
                    s.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None,
                                           new AsyncCallback(Async_Receive), WorkSocket);
                }
                catch (Exception e)
                {
                    if (!SocketConnected)
                    {
                        //Disconnected
                        OnDisconnect(this);
                    }
                    else
                    {
                        WriteError("BeginReceive", e);
                    }
                }
            }
        }

        private void WriteError(string func, Exception e)
        {
            if (OnError != null)
            {
                OnError(func, e);
            }
        }

        private void Async_Send(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;

            try
            {
                int send = s.EndSend(ar);

                IncBufferLength(0 - send);
            }
            catch (Exception e)
            {
                Console.WriteLine("Async_Send");
                WriteError("Async_Send", e);
            }
        }
        #endregion

        #region internal methods

        internal void Close()
        {
            if (_Listener != null)
            {
                Server.SendMessageTask task = _Listener.GetTask(this);

                int queueCount;

                lock (_QueueLockObj)
                {
                    _Closed = true;
                    queueCount = _Queue.Count;
                }

                task.IncTotalQueueCount(0 - queueCount);
            }

            try
            {
                WorkSocket.Close();
            }
            catch
            {
            }


        }

        internal void ASendFromServer(MessageFlag flag, UInt32 evt, UInt16 cableId, UInt32 channel, byte[] data)
        {
            while (BufferLength > 10 * 1024 * 1024)
            {
                System.Threading.Thread.Sleep(1);
            }

            Server.SendMessageTask task = _Listener.GetTask(this);

            bool needSetEvent = false;

            lock (task.LockObj)
            {
                needSetEvent = task.IncTotalQueueCount(1) == 0;

                lock (_QueueLockObj)
                {
                    IncBufferLength(data.Length);
                    _Queue.Enqueue(new Message(flag, evt, cableId, channel, data));
                }
            }

            if (needSetEvent)
            {
                task.Event.Set();
            }

        }

        /// <summary>
        /// send message from queue. only server side use it.
        /// </summary>
        /// <returns>how many messages has been send</returns>
        internal int SendFromQueue()
        {
            lock (_QueueLockObj)
            {
                if (_Closed)
                {
                    return 0;
                }

                int queueCount = _Queue.Count;

                try
                {
                    Server.SendMessageTask task = _Listener.GetTask(this);

                    task.IncTotalQueueCount(0 - queueCount);

                    while (_Queue.Count > 0)
                    {
                        Message message = _Queue.Dequeue();

                        message.WriteToStream(_MStream);

                        IncBufferLength(0 - message.Data.Length);

                        if (_MStream.Length > MStreamCapacity / 2)
                        {
                            try
                            {
                                AsyncSend(_MStream.GetBuffer(), 0, (int)_MStream.Length);
                            }
                            catch (Exception e)
                            {
                                _Queue.Clear();

                                try
                                {
                                    if (WorkSocket.Connected)
                                    {
                                        WorkSocket.Close();
                                    }
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    //Server.NTcpListener.RemoteSCB(this);
                                }

                                WriteError("SendFromQueue", e);
                                return queueCount;
                            }

                            if (_MStream.Length > MStreamCapacity)
                            {
                                _MStream = new System.IO.MemoryStream(MStreamCapacity);
                            }
                            else
                            {
                                _MStream.SetLength(0);
                                _MStream.Position = 0;
                            }
                        }
                    }

                    if (_MStream.Length > 0)
                    {
                        try
                        {
                            AsyncSend(_MStream.GetBuffer(), 0, (int)_MStream.Length);
                        }
                        catch (Exception e)
                        {
                            _Queue.Clear();

                            try
                            {
                                if (WorkSocket.Connected)
                                {
                                    WorkSocket.Close();
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                //Server.NTcpListener.RemoteSCB(this);
                            }

                            WriteError("SendFromQueue", e);
                            return queueCount;
                        }

                        if (_MStream.Length > MStreamCapacity)
                        {
                            _MStream = new System.IO.MemoryStream(MStreamCapacity);
                        }
                        else
                        {
                            _MStream.SetLength(0);
                            _MStream.Position = 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteError("SendFromQueue", e);
                }

                return queueCount;
            }

        }


        internal void AsyncSend(byte[] buf, int offset, int size)
        {
            //while (BufferLength > 1024 * 1024)
            //{
            //    System.Threading.Thread.Sleep(1);
            //}

            //IncBufferLength(size);
            //WorkSocket.BeginSend(buf, offset, size, SocketFlags.None,
            //    new AsyncCallback(Async_Send), WorkSocket);

            WorkSocket.Send(buf, offset, size, SocketFlags.None);

        }

        internal void AsyncSend(byte[] buf)
        {
            //while (BufferLength > 1024 * 1024)
            //{
            //    System.Threading.Thread.Sleep(1);
            //}
            //IncBufferLength(buf.Length);
            //WorkSocket.BeginSend(buf, 0, buf.Length, SocketFlags.None,
            //    new AsyncCallback(Async_Send), WorkSocket);

            WorkSocket.Send(buf, SocketFlags.None);
        }

        /// <summary>
        /// Send async message
        /// </summary>
        /// <param name="flag">flag</param>
        /// <param name="evt">event</param>
        /// <param name="data">data</param>
        internal void AsyncSend(MessageFlag flag, UInt32 evt, UInt16 cableId, UInt32 channel, byte[] data)
        {
            lock (_SendLockObj)
            {
                int offset = 2;

                //Flag
                _MSGHead[offset++] = (byte)flag;

                //Event
                _MSGHead[offset++] = (byte)(evt / ThreeBytes);
                _MSGHead[offset++] = (byte)((evt % ThreeBytes) / 65536);
                _MSGHead[offset++] = (byte)((evt % 65536) / 256);
                _MSGHead[offset++] = (byte)(evt % 256);

                //CableId
                _MSGHead[offset++] = (byte)(cableId / 256);
                _MSGHead[offset++] = (byte)(cableId % 256);

                //Channel
                _MSGHead[offset++] = (byte)(channel / ThreeBytes);
                _MSGHead[offset++] = (byte)((channel % ThreeBytes) / 65536);
                _MSGHead[offset++] = (byte)((channel % 65536) / 256);
                _MSGHead[offset++] = (byte)(channel % 256);

                //Length
                int len = data.Length;
                _MSGHead[offset++] = (byte)(len / 65536);
                _MSGHead[offset++] = (byte)((len % 65536) / 256);
                _MSGHead[offset++] = (byte)(len % 256);

                byte[] buf = new byte[_MSGHead.Length + data.Length];
                Array.Copy(_MSGHead, 0, buf, 0, _MSGHead.Length);

                Array.Copy(data, 0, buf, _MSGHead.Length, data.Length);

                AsyncSend(buf);
            }

            //AsyncSend(_MSGHead, 0, _MSGHead.Length);

            //AsyncSend(data);
        }


        #endregion

        #region constractor
        internal SCB(Socket workSocket)
            : this(null, workSocket)
        {

        }

        internal SCB(Server.NTcpListener listener, Socket workSocket)
        {
            _Listener = listener;

            lock (_SCBID_LOCK)
            {
                Id = _SCB_ID++;
            }

            this._CurState = State.Sync0;
            this.WorkSocket = workSocket;
            _MSGHead[0] = 0xA5;
            _MSGHead[1] = 0xA5;
            RemoteIPEndPoint = (IPEndPoint)workSocket.RemoteEndPoint;
            //AddTCB(this);

            _MStream = new System.IO.MemoryStream(MStreamCapacity);

            WorkSocket.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None,
                                       new AsyncCallback(Async_Receive), WorkSocket);


        }

        #endregion


    }

}
