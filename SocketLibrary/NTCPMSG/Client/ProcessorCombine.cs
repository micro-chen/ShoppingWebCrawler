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
using System.Diagnostics;
using System.Text;

using System.Threading;

namespace NTCPMessage.Client
{
    /// <summary>
    /// Combine sync thread to same processor when it reach the threshold
    /// </summary>
    class ProcessorCombine
    {
        class ThreadInfo
        {
            internal uint ThreadId { get; private set; }
            internal int Count { get; set; }

            private IntPtr _OriginalAffinityMask;
            private int _ChangeAffinityTimes = 0;

            private bool _Setuped;

            internal ThreadInfo(uint threadId)
            {
                ThreadId = threadId;
                Count = 1;
                _Setuped = false;
            }

            internal void Reset()
            {
                _Setuped = false;
            }

            internal void SetAffinityMask(IntPtr affinity)
            {
                if (_Setuped)
                {
                    return;
                }

                IntPtr handle = WinAPI.OpenThread(WinAPI.ThreadAccess.SET_INFORMATION | WinAPI.ThreadAccess.QUERY_INFORMATION, false,
                    ThreadId);

                IntPtr last = WinAPI.SetThreadAffinityMask(handle, affinity);

                WinAPI.CloseHandle(handle);

                if (last != IntPtr.Zero)
                {
                    _Setuped = true;

                    if (_ChangeAffinityTimes++ == 0)
                    {
                        _OriginalAffinityMask = last;
                    }
                }
            }

            internal void Restore()
            {
                if (_OriginalAffinityMask != IntPtr.Zero)
                {
                    IntPtr handle = WinAPI.OpenThread(WinAPI.ThreadAccess.SET_INFORMATION | WinAPI.ThreadAccess.QUERY_INFORMATION, false,
                         ThreadId);
                    WinAPI.SetThreadAffinityMask(handle, _OriginalAffinityMask);

                    WinAPI.CloseHandle(handle);
                }
            }
        }

        Thread _Thread;
        private IntPtr _ThreadAffinityMask;

        readonly uint _SendMessageQueueThreadId;

        static IntPtr GetProcessorAffinity(int processorId)
        {
            long shift;
            shift = 0x0000000000000001;
            int r = processorId;

            for (int i = 0; i < r; i++)
            {
                shift <<= 1;
            }

            return (IntPtr)shift;
        }

        internal bool Running { get; private set; }

        private object _LockObj = new object();

        private Dictionary<uint, ThreadInfo> _ThreadIdToThreadInfo;

        private void ThreadProc()
        {
            IntPtr handle = WinAPI.OpenThread(WinAPI.ThreadAccess.SET_INFORMATION | WinAPI.ThreadAccess.QUERY_INFORMATION, false,
                WinAPI.GetCurrentThreadId());

            IntPtr last = WinAPI.SetThreadAffinityMask(handle, _ThreadAffinityMask);

            WinAPI.CloseHandle(handle);

            while (true)
            {
                Thread.Sleep(1000);

                lock (_LockObj)
                {
                    List<uint> delThreads = new List<uint>();

                    foreach (ThreadInfo tInfo in _ThreadIdToThreadInfo.Values)
                    {
                        if (tInfo.Count > 10)
                        {
                            //If hit more than 10 times in one second
                            //Combine the affinity
                            tInfo.SetAffinityMask(_ThreadAffinityMask);
                        }
                        else if (tInfo.Count == 0)
                        {
                            tInfo.Restore();
                            delThreads.Add(tInfo.ThreadId);
                        }

                        tInfo.Count = 0;
                    }

                    //Delete the thread that didn't hit in one second
                    foreach (uint threadId in delThreads)
                    {
                        _ThreadIdToThreadInfo.Remove(threadId);
                    }
                }
            }
        }

        internal ProcessorCombine(uint sendMessageQueueThreadId, int processorId)
        {
            _SendMessageQueueThreadId = sendMessageQueueThreadId;

            IntPtr handle = WinAPI.OpenThread(WinAPI.ThreadAccess.SET_INFORMATION | WinAPI.ThreadAccess.QUERY_INFORMATION, false,
                sendMessageQueueThreadId);

            _ThreadAffinityMask = GetProcessorAffinity(processorId);

            if (WinAPI.SetThreadAffinityMask(handle, _ThreadAffinityMask) == IntPtr.Zero)
            {
                //Can't set affinity mask. Maybe no enough rights.
                Running = false;

                WinAPI.CloseHandle(handle);

                return;
            }

            WinAPI.CloseHandle(handle);

            _ThreadIdToThreadInfo = new Dictionary<uint, ThreadInfo>();
            _Thread = new Thread(ThreadProc);
            _Thread.IsBackground = true;
            _Thread.Start();
            Running = true;
        }


        internal void Hit()
        {
            lock (_LockObj)
            {
                uint id = WinAPI.GetCurrentThreadId();

                if (id != 0)
                {
                    ThreadInfo threadInfo;

                    if (!_ThreadIdToThreadInfo.TryGetValue(id, out threadInfo))
                    {
                        _ThreadIdToThreadInfo.Add(id, new ThreadInfo(id));
                    }
                    else
                    {
                        threadInfo.Count++;
                    }
                }
            }
        }
    }
}
