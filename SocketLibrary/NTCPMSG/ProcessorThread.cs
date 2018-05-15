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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;

namespace System.Threading
{
    /// <summary>
    /// This class is used to bind the thread
    /// to specified logical processors
    /// </summary>
    public class ProcessorThread
    {
        [DllImport("kernel32")]
        static extern int GetCurrentThreadId();

        #region Private member
        static long _FullProcessorAffinity = 0;
        static object _sLockObj = new object();

        Thread _Thread;

        ParameterizedThreadStart _ParaThreadStart = null;
        ThreadStart _ThreadStart = null;
        
        int _ThreadId = 0;

        IntPtr _ProcessorAffinity, _DefaultProcessorAffinity;

        private void ParaStartProc(object state)
        {
            _ThreadId = GetCurrentThreadId();
            Process Proc = Process.GetCurrentProcess();
            foreach (ProcessThread pt in Proc.Threads)
            {
                if (pt.Id == _ThreadId)
                {
                    pt.ProcessorAffinity = _ProcessorAffinity;
                }
            }

            _ParaThreadStart(state);
        }

        private void StartProc()
        {
            _ThreadId = GetCurrentThreadId();
            Process Proc = Process.GetCurrentProcess();
            foreach (ProcessThread pt in Proc.Threads)
            {
                if (pt.Id == _ThreadId)
                {
                    pt.ProcessorAffinity = _ProcessorAffinity;
                }
            }

            _ThreadStart();
        }

        private void InitVar()
        {
            _DefaultProcessorAffinity = _ProcessorAffinity = Process.GetCurrentProcess().ProcessorAffinity;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get full processor affinity mask
        /// </summary>
        static public long FullProcessorAffinity
        {
            get
            {
                lock (_sLockObj)
                {
                    if (_FullProcessorAffinity == 0)
                    {
                        long shift = 0x0000000000000001;

                        for (int i = 0; i < Environment.ProcessorCount; i++)
                        {
                            _FullProcessorAffinity |= shift;

                            shift <<= 1;
                        }
                    }

                    return _FullProcessorAffinity;
                }  
            }
        }

        /// <summary>
        /// Sets or gets the processor affinity
        /// </summary>

        public IntPtr ProcessorAffinity
        {
            get
            {
                return _ProcessorAffinity;
            }

            set
            {
                if (((long)value | (long)_DefaultProcessorAffinity) != (long)_DefaultProcessorAffinity)
                {
                    _ProcessorAffinity = _DefaultProcessorAffinity;
                }
                else
                {
                    _ProcessorAffinity = value;
                }

                if (_ThreadId != 0)
                {
                    Process Proc = Process.GetCurrentProcess();
                    foreach (ProcessThread pt in Proc.Threads)
                    {
                        if (pt.Id == _ThreadId)
                        {
                            pt.ProcessorAffinity = _ProcessorAffinity;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Gets or sets the culture for the current thread.
        /// </summary>
        public CultureInfo CurrentCulture
        {
            get
            {
                return _Thread.CurrentCulture;
            }

            set
            {
                _Thread.CurrentCulture = value;
            }
        }

        /// <summary>
        /// Gets or sets the current culture used by the Resource Manager to look up culture-specific resources at run time.
        /// </summary>
        public CultureInfo CurrentUICulture
        {
            get
            {
                return _Thread.CurrentUICulture;
            }

            set
            {
                _Thread.CurrentUICulture = value;
            }
        }

        /// <summary>
        /// Gets an ExecutionContext object that contains information about the various contexts of the current thread. 
        /// </summary>
        public ExecutionContext ExecutionContext
        {
            get
            {
                return _Thread.ExecutionContext;
            }
        }

        /// <summary>
        /// Gets a value indicating the execution status of the current thread.
        /// true if this thread has been started and has not terminated normally or aborted; otherwise, false.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return _Thread.IsAlive;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a thread is a background thread.
        /// true if this thread is or is to become a background thread; otherwise, false.
        /// </summary>
        public bool IsBackground
        {
            get
            {
                return _Thread.IsBackground;
            }

            set
            {
                _Thread.IsBackground = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a thread belongs to the managed thread pool.
        /// true if this thread belongs to the managed thread pool; otherwise, false.
        /// </summary>
        public bool IsThreadPoolThread
        {
            get
            {
                return _Thread.IsThreadPoolThread;
            }
        }

        /// <summary>
        /// Gets a unique identifier for the current managed thread.
        /// An integer that represents a unique identifier for this managed thread.
        /// </summary>
        public int ManagedThreadId
        {
            get
            {
                return _Thread.ManagedThreadId;
            }
        }

        /// <summary>
        /// Gets or sets the name of the thread.
        /// A string containing the name of the thread, or null if no name was set.
        /// </summary>
        public string Name
        {
            get
            {
                return _Thread.Name;
            }

            set
            {
                _Thread.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of a thread.
        /// One of the ThreadPriority values. The default value is Normal.
        /// </summary>
        public ThreadPriority Priority
        {
            get
            {
                return _Thread.Priority;
            }

            set
            {
                _Thread.Priority = value;
            }
        }

        /// <summary>
        /// Gets a value containing the states of the current thread.
        /// One of the ThreadState values indicating the state of the current thread. The initial value is Unstarted.
        /// </summary>
        public System.Threading.ThreadState ThreadState
        {
            get
            {
                return _Thread.ThreadState;
            }
        }

        #endregion

        #region constractor

        /// <summary>
        /// Initializes a new instance of the Thread class, specifying a delegate that allows an object to be passed to the thread when the thread is started.
        /// </summary>
        /// <param name="start">A ParameterizedThreadStart delegate that represents the methods to be invoked when this thread begins executing.</param>
        public ProcessorThread(ParameterizedThreadStart start)
        {
            InitVar();
            _ParaThreadStart = start;
            _Thread = new Thread(ParaStartProc);
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="start">A ThreadStart delegate that represents the methods to be invoked when this thread begins executing.</param>
        public ProcessorThread(ThreadStart start)
        {
            InitVar();
            _ThreadStart = start;
            _Thread = new Thread(StartProc);
        }

        /// <summary>
        /// Initializes a new instance of the Thread class, specifying a delegate that allows an object to be passed to the thread when the thread is started and specifying the maximum stack size for the thread.
        /// </summary>
        /// <param name="start">A ParameterizedThreadStart delegate that represents the methods to be invoked when this thread begins executing.</param>
        /// <param name="maxStackSize">The maximum stack size to be used by the thread, or 0 to use the default maximum stack size specified in the header for the executable.
        ///Important   For partially trusted code, maxStackSize is ignored if it is greater than the default stack size. No exception is thrown.
        ///</param>
        public ProcessorThread(ParameterizedThreadStart start, int maxStackSize)
        {
            InitVar();
            _ParaThreadStart = start;
            _Thread = new Thread(ParaStartProc, maxStackSize);
        }

        /// <summary>
        /// Initializes a new instance of the Thread class, specifying the maximum stack size for the thread.
        /// </summary>
        /// <param name="start">A ThreadStart delegate that represents the methods to be invoked when this thread begins executing.</param>
        /// <param name="maxStackSize">The maximum stack size to be used by the thread, or 0 to use the default maximum stack size specified in the header for the executable.
        ///Important   For partially trusted code, maxStackSize is ignored if it is greater than the default stack size. No exception is thrown.
        ///</param>
        public ProcessorThread(ThreadStart start, int maxStackSize)
        {
            InitVar();
            _ThreadStart = start;
            _Thread = new Thread(StartProc, maxStackSize);
        }
        #endregion


        #region Public methods

        /// <summary>
        /// Raises a ThreadAbortException in the thread on which it is invoked, to begin the process of terminating the thread. Calling this method usually terminates the thread.
        /// </summary>
        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void Abort()
        {
            _Thread.Abort();
        }

        /// <summary>
        /// Raises a ThreadAbortException in the thread on which it is invoked, to begin the process of terminating the thread while also providing exception information about the thread termination. Calling this method usually terminates the thread.
        /// </summary>
        /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted. </param>
        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void Abort(Object stateInfo)
        {
            _Thread.Abort(stateInfo);
        }

        public override bool Equals(object obj)
        {
            return _Thread.Equals(obj);
        }

        /// <summary>
        /// Returns an ApartmentState value indicating the apartment state.
        /// </summary>
        /// <returns>One of the ApartmentState values indicating the apartment state of the managed thread. The default is ApartmentState.Unknown.</returns>
        public ApartmentState GetApartmentState()
        {
            return _Thread.GetApartmentState();
        }

        public override int GetHashCode()
        {
            return _Thread.GetHashCode();
        }

        /// <summary>
        /// Interrupts a thread that is in the WaitSleepJoin thread state.
        /// </summary>
        /// <remarks>
        /// If this thread is not currently blocked in a wait, sleep, or join state, it will be interrupted when it next begins to block.
        /// ThreadInterruptedException is thrown in the interrupted thread, but not until the thread blocks. If the thread never blocks, the exception is never thrown, and thus the thread might complete without ever being interrupted.
        /// </remarks>
        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void Interrupt()
        {
            _Thread.Interrupt();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates, while continuing to perform standard COM and SendMessage pumping.
        /// </summary>
        [HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public void Join()
        {
            _Thread.Join();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait for the thread to terminate. </param>
        /// <returns>true if the thread has terminated; false if the thread has not terminated after the amount of time specified by the millisecondsTimeout parameter has elapsed.</returns>
        [HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public bool Join(int millisecondsTimeout)
        {
            return Join(millisecondsTimeout);
        }
        
        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.
        /// </summary>
        /// <param name="timeout">A TimeSpan set to the amount of time to wait for the thread to terminate. </param>
        /// <returns>true if the thread terminated; false if the thread has not terminated after the amount of time specified by the timeout parameter has elapsed.</returns>
        [HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public bool Join(TimeSpan timeout)
        {
            return Join(timeout);
        }

        /// <summary>
        /// Sets the apartment state of a thread before it is started.
        /// </summary>
        /// <param name="state">The new apartment state.</param>
        [HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, SelfAffectingThreading = true)]
        public void SetApartmentState(ApartmentState state)
        {
            _Thread.SetApartmentState(state);
        }

        /// <summary>
        ///  sets the processor affinity for a managed thread. Processor affinity determines the processors on which a thread runs.
        /// </summary>
        /// <param name="processors">
        /// An array of identifiers that specify the logic processor on which the managed thread is permitted to run.
        /// id of logic processor start from 0. 
        /// </param>
        public void SetProcessorAffinity(params int[] processors)
        {
            if (processors == null)
            {
                throw new ArgumentException("processors can't be null!");
            }

            long affinity = 0;

            foreach (int processorId in processors)
            {
                if (processorId >= 64)
                {
                    throw new ArgumentException("processors id can't be large than or equal 64!");
                }

                affinity |= (long)((ulong)0x0000000000000001 << processorId); 
            }

            ProcessorAffinity = (IntPtr)affinity;
        }

        /// <summary>
        /// Causes the operating system to change the state of the current instance to ThreadState.Running.
        /// </summary>
        public void Start()
        {
            _Thread.Start();
        }

        /// <summary>
        /// Causes the operating system to change the state of the current instance to ThreadState.Running, and optionally supplies an object containing data to be used by the method the thread executes.
        /// </summary>
        /// <param name="parameter">An object that contains data to be used by the method the thread executes.</param>
        [HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public void Start(Object parameter)
        {
            _Thread.Start(parameter);
        }

        /// <summary>
        /// Sets the apartment state of a thread before it is started.
        /// </summary>
        /// <param name="state">The new apartment state.</param>
        /// <returns>true if the apartment state is set; otherwise, false.</returns>
        [HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true, SelfAffectingThreading = true)]
        public bool TrySetApartmentState( ApartmentState state)
        {
            return _Thread.TrySetApartmentState(state);
        }

        public override string ToString()
        {
            return _Thread.ToString();
        }

        #endregion
    }
}
