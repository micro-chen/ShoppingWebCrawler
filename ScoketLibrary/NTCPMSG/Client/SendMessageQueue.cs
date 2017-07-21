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
using System.Threading;

namespace NTCPMessage.Client
{
    class SendMessageQueue
    {
        internal delegate void DeleReadyToSend(byte[] data, int length);

        const int MStreamCapacity = 8192;

        private System.IO.MemoryStream _MStream;
        private DeleReadyToSend OnReadyToSend;

        AutoResetEvent _Event;
        Thread _Thread;
        private Queue<Message> _Queue;
        private object _LockObj = new object();
        private object _BufLockObj = new object();

        private bool _Closing = false;
        private bool _CLosed = false;
        private readonly bool _SetThreadAffinityMask = false;

        private object _ProcessorCombineLock = new object();
        private ProcessorCombine _ProcessorCombine;
        private uint _CurThreadId = 0;

        private uint CurThreadId
        {
            get
            {
                lock (_ProcessorCombineLock)
                {
                    return _CurThreadId;
                }
            }

            set
            {
                lock (_ProcessorCombineLock)
                {
                    _CurThreadId = value;
                }
            }
        }

        private ProcessorCombine PCombine
        {
            get
            {
                lock (_ProcessorCombineLock)
                {
                    return _ProcessorCombine;
                }
            }

            set
            {
                lock (_ProcessorCombineLock)
                {
                    _ProcessorCombine = value;
                }
            }
        }

        private long _BufferLength = 0;

        private long BufferLength
        {
            get
            {
                lock (_BufLockObj)
                {
                    return _BufferLength;
                }
            }
        }

        private void IncBufferLength(long value)
        {
            lock (_BufLockObj)
            {
                _BufferLength += value;
            }
        }

        internal bool Closed
        {
            get
            {
                lock (_LockObj)
                {
                    return _CLosed;
                }
            }

            private set
            {
                lock (_LockObj)
                {
                    _CLosed = value;
                }
            }
        }

        internal SendMessageQueue(DeleReadyToSend onReadyToSend, bool setThreadAffinityMask)
        {
            _SetThreadAffinityMask = setThreadAffinityMask;

            OnReadyToSend = onReadyToSend;
            _Queue = new Queue<Message>();
            _MStream = new System.IO.MemoryStream(MStreamCapacity);
            _Event = new AutoResetEvent(false);
            _Thread = new Thread(ThreadProc);
            _Thread.Name = "SendMessageQueue";
            _Thread.IsBackground = true;
            _Thread.Start();
        }

        internal void AsyncSend(MessageFlag flag, UInt32 evt, UInt16 cableId, UInt32 channel, byte[] data)
        {
            if (_SetThreadAffinityMask && (flag | MessageFlag.Sync) != 0)
            {
                if (PCombine != null)
                {
                    PCombine.Hit();
                }
            }

            while (BufferLength > 10 * 1024 * 1024)
            {
                System.Threading.Thread.Sleep(1);
            }

            bool needSetEvent = false;

            lock (_LockObj)
            {
                needSetEvent = _Queue.Count == 0;

                IncBufferLength(data.Length);
                _Queue.Enqueue(new Message(flag, evt, cableId, channel, data));
            }

            if (needSetEvent)
            {
                _Event.Set();
            }
        }

        private void ThreadProc()
        {
            if (_SetThreadAffinityMask)
            {
                //_Thread.Priority = ThreadPriority.Lowest;
                CurThreadId = WinAPI.GetCurrentThreadId();
            }

            while (true)
            {
                _Event.WaitOne();

                long processCount = 0;

                lock (_LockObj)
                {
                    if (_Closing)
                    {
                        Closed = true;
                        return;
                    }

                    while (_Queue.Count > 0)
                    {
                        if (_Closing)
                        {
                            Closed = true;
                            return;
                        }

                        Message message = _Queue.Dequeue();

                        if ((message.Flag & MessageFlag.Sync) == 0)
                        {
                            //only asyncronize message need wait
                            processCount++;
                        }

                        message.WriteToStream(_MStream);

                        IncBufferLength(0 - message.Data.Length);

                        if (_MStream.Length > MStreamCapacity / 2)
                        {
                            if (OnReadyToSend != null)
                            {
                                try
                                {
                                    OnReadyToSend(_MStream.GetBuffer(), (int)_MStream.Length);
                                }
                                catch(Exception e)
                                {
                                    //Console.WriteLine(e);
                                    Closed = true;
                                    return;
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
                    }

                    if (_MStream.Length > 0)
                    {
                        if (OnReadyToSend != null)
                        {
                            try
                            {
                                OnReadyToSend(_MStream.GetBuffer(), (int)_MStream.Length);
                            }
                            catch
                            {
                                Closed = true;
                                return;
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
                }

                //Exit current context, realse the processor resource for other thread
                //This is very importent for improve the performance
                //Thread.Sleep(0);
                if (processCount > 2)
                {
                    Thread.Sleep(1);
                }
            }
        }

        internal void SetProcessorId(int processorId)
        {
            while (CurThreadId == 0)
            {
                Thread.Sleep(1); 
            }

            PCombine = new ProcessorCombine(CurThreadId, processorId);
        }

        internal bool Join(int millisecondsTimeout)
        {
            return _Thread.Join(millisecondsTimeout);
        }

        internal void Abort()
        {
            _Thread.Abort();
        }
             

        internal void Close()
        {
            lock (_LockObj)
            {
                _Closing = true;
            }

            _Event.Set();
        }
    }
}
