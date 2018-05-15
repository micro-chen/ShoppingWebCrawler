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

namespace NTCPMessage.Server
{
    /// <summary>
    /// Server side sendmessage pool.
    /// It is used for cache the message send from server to client.
    /// One send message task manage at least one SCB
    /// </summary>
    class SendMessageTask
    {

        #region Fields
        /// <summary>
        /// Event to activate the task
        /// </summary>
        readonly internal AutoResetEvent Event;
        internal object LockObj = new object();
        long _TotalQueueCount;
        ProcessorThread _Thread;
        int _Id;
        NTcpListener _Listener;
        #endregion

        #region Private & internal members
        internal int Id
        {
            get
            {
                return _Id;
            }
        }

        internal long TotalQueueCount
        {
            get
            {
                lock (LockObj)
                {
                    return _TotalQueueCount;
                }
            }
        }

        internal void Start(int id)
        {
            _Id = id;

            _Thread.SetProcessorAffinity(id % Environment.ProcessorCount);
            _Thread.Priority = ThreadPriority.Lowest;
            _Thread.Start();
        }

        private void ThreadProc()
        {
            //DateTime last = DateTime.Now;
            //long sendMessageNumber = 0;
            //bool sleep1 = false;
            //int sendPerSec = 0;
            //int mod = 10;

            IntPtr handle = WinAPI.OpenThread(WinAPI.ThreadAccess.SET_INFORMATION | WinAPI.ThreadAccess.QUERY_INFORMATION, false,
                WinAPI.GetCurrentThreadId());

            WinAPI.SetThreadPriorityBoost(handle, true);

            WinAPI.CloseHandle(handle);

            while (true)
            {
                Event.WaitOne();
                
                //sendPerSec++;

                SCB[] scbs = _Listener.GetAllSCB();

                lock (LockObj)
                {
                    foreach (SCB scb in scbs)
                    {
                        if (this == _Listener.GetTask(scb))
                        {
                            scb.SendFromQueue();
                            //sendMessageNumber += scb.SendFromQueue();
                        }
                    }
                }

                Thread.Sleep(0);

                //DateTime now = DateTime.Now;

                //double elapseMilliseconds = (now - last).TotalMilliseconds;
                
                //if (elapseMilliseconds > 1000)
                //{
                //    double messagePerSecond = sendMessageNumber * 1000 / elapseMilliseconds;

                //    mod = ((int)(10000 / messagePerSecond) + 1) * 10;

                //    if (messagePerSecond > 1000)
                //    {
                //        sleep1 = true;
                //    }
                //    else
                //    {
                //        sleep1 = false;
                //    }

                //    sendPerSec = 0;
                //    sendMessageNumber = 0;
                //    last = DateTime.Now;
                //}

                //if (sleep1)
                //{
                //    if (sendPerSec % mod == 0)
                //    {
                //        Thread.Sleep(1);
                //    }
                //    else
                //    {
                //        Thread.Sleep(0);
                //    }
                //}
                //else
                //{
                //    Thread.Sleep(0);
                //}
            }

        }

        /// <summary>
        /// Increase total queue count
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>return the last total queue count</remarks>
        internal long IncTotalQueueCount(int value)
        {
            lock (LockObj)
            {
                long temp = _TotalQueueCount;
                _TotalQueueCount += value;
                return temp;
            }
        }

        #endregion

        #region constractor

        internal SendMessageTask(NTcpListener listenser)
        {
            _Listener = listenser;
            _TotalQueueCount = 0;
            Event = new AutoResetEvent(false);

            _Thread = new ProcessorThread(ThreadProc);

            _Thread.IsBackground = true;
        }


        #endregion


    }
}
