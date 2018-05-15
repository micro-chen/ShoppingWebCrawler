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
using NTCPMessage.Event;

namespace NTCPMessage.Server
{
    public class ReceiveMessageQueue
    {
        public delegate void OnMessage(ReceiveEventArgs message);
        public delegate void OnError(Exception e);

        private static int _RecvId = 0;
        private static object _sLockObj = new object();


        #region Private fields
        
        private long _RestartTimes = 0;

        private object _LockObj = new object();

        private System.Threading.ProcessorThread _Thread;

        private Queue<ReceiveEventArgs> _Queue = new Queue<ReceiveEventArgs>();

        private OnMessage _OnMessageEvent;
        private OnError _OnError;
        private System.Threading.AutoResetEvent _Event;

        private System.Threading.ManualResetEvent _CloseEvent = null;
        private bool _Started = false;
        private bool _Closing = false;

        #endregion

        #region Properties

        public int Id { get; private set; }


        public int ManagedThreadId
        {
            get
            {
                if (_Thread == null)
                {
                    return -1;
                }
                else
                {
                    return _Thread.ManagedThreadId;
                }
            }
        }

        public long RestartTimes
        {
            get
            {
                lock (_LockObj)
                {
                    return _RestartTimes;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _RestartTimes = value;
                }
            }
        }


        public bool Started
        {
            get
            {
                lock (_LockObj)
                {
                    return _Started;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _Started = value;
                }
            }
        }

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

        protected OnMessage OnMessageEvent
        {
            get
            {
                return _OnMessageEvent;
            }

            set
            {
                _OnMessageEvent = value;
            }
        }

        /// <summary>
        /// call back when error raise
        /// </summary>
        public OnError OnErrorEvent
        {
            get
            {
                return _OnError;
            }

            set
            {
                _OnError = value;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Process message
        /// </summary>
        private void MessageProc()
        {
            try
            {
                while (true)
                {
                    _Event.WaitOne();

                    if (Closing)
                    {
                        try
                        {
                            bool close = true;

                            lock (_LockObj)
                            {
                                if (_Queue.Count > 0)
                                {
                                    close = false;
                                }
                            }

                            if (close)
                            {
                                _CloseEvent.Set();
                                return;
                            }
                        }
                        catch
                        {
                        }
                    }

                    ReceiveEventArgs msg = default(ReceiveEventArgs);

                    try
                    {
                        int queueCount = 0;

                        lock (_LockObj)
                        {
                            while (_Queue.Count > 0)
                            {
                                msg = _Queue.Dequeue();

                                if ((msg.Flag & MessageFlag.Sync) == 0)
                                {
                                    //only asyncronize message need wait
                                    queueCount++;
                                }

                                if (_OnMessageEvent != null)
                                {
                                    try
                                    {
                                        _OnMessageEvent(msg);
                                    }
                                    catch (Exception e)
                                    {
                                        if (OnErrorEvent != null)
                                        {
                                            try
                                            {
                                                OnErrorEvent(e);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Exit current context, realse the processor resource for other thread
                        //This is very importent for improve the performance
                        System.Threading.Thread.Sleep(0);
                        //if (queueCount > 2)
                        //{
                        //    System.Threading.Thread.Sleep(1);
                        //}
                    }
                    catch (Exception e)
                    {
                        if (OnErrorEvent != null)
                        {
                            try
                            {
                                OnErrorEvent(e);
                            }
                            catch
                            {
                            }
                        }

                    }
                    finally
                    {

                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Constructor

        public ReceiveMessageQueue()
            : this(null)
        {
        }



        public ReceiveMessageQueue(OnMessage onMessageEvent)
        {
            _OnMessageEvent = onMessageEvent;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Start to receive message
        /// </summary>
        public void Start()
        {
            lock (_sLockObj)
            {
                Id = _RecvId++;
            }

            lock (_LockObj)
            {
                if (!Started)
                {
                    _CloseEvent = new System.Threading.ManualResetEvent(false);
                    Closing = false;
                    _Event = new System.Threading.AutoResetEvent(false);
                    _Thread = new System.Threading.ProcessorThread(new System.Threading.ThreadStart(MessageProc));
                    _Thread.IsBackground = true;
                    _Thread.SetProcessorAffinity(Id % Environment.ProcessorCount);
                    _Thread.Start();

                    RestartTimes++;

                    Started = true;
                }
            }
        }

        public void AbortAndRestart()
        {
            Abort();
            Start();
        }


        public void Abort()
        {
            Closing = true;

            lock (_LockObj)
            {
                try
                {
                    _Thread.Abort();
                    ReleaseResources();

                    try
                    {
                        OnErrorEvent(new NTcpException("Message queue abort because of timeout", ErrorCode.QueueAbort));
                    }
                    catch
                    {
                    }
                }
                catch
                {
                    ReleaseResources();
                }
                finally
                {

                    //_OnMessageEvent = null;
                    //_OnError = null;
                }
            }
        }

        private void ReleaseResources()
        {
            try
            {
                _CloseEvent.Close();
            }
            catch
            {
            }

            try
            {
                _Event.Close();
            }
            catch
            {
            }

            _CloseEvent = null;
            _Event = null;
            _Thread = null;
            Started = false;

        }

        /// <summary>
        /// Close message queue
        /// Exit thread
        /// </summary>
        /// <param name="millisecondsTimeout">wait timeout for close</param>
        public bool Close(int millisecondsTimeout)
        {
            Closing = true;

            lock (_LockObj)
            {
                try
                {
                    _Event.Set();

                    if (!_CloseEvent.WaitOne(millisecondsTimeout, true))
                    {
                        _Thread.Abort();
                    }
                    else
                    {
                        return true;
                    }
                }
                catch
                {
                }
                finally
                {
                    ReleaseResources();

                    //_OnMessageEvent = null;
                    //_OnError = null;
                }

                return false;
            }
        }

        public void ASendMessages(IList<ReceiveEventArgs> messages)
        {
            int queueCount = 0;

            lock (_LockObj)
            {
                if (!Started)
                {
                    Start();
                }

                queueCount = _Queue.Count;

                foreach (ReceiveEventArgs args in messages)
                {
                    _Queue.Enqueue(args);
                }
            }

            if (queueCount == 0)
            {
                _Event.Set();
            }


        }

        /// <summary>
        /// Send asynchronous message from another thread
        /// </summary>
        /// <param name="evt">Message event number</param>
        /// <param name="data">Message data</param>
        public void ASendMessage(ReceiveEventArgs message)
        {
            int queueCount = 0;

            lock (_LockObj)
            {
                if (!Started)
                {
                    Start();
                }

                queueCount = _Queue.Count;

                _Queue.Enqueue(message);
            }

            if (queueCount == 0)
            {
                _Event.Set();
            }
        }

        #endregion
    }
}
