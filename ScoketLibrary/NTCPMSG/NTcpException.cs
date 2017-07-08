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

namespace NTCPMessage
{
    public enum ErrorCode
    {
        Success = 0,

        SocketNotExist = 1,

        QueueAbort = 10,

        //Server
        AlreadyListened = 100,

        //Client

        Disconnected = 1000,
        ConnectTimeout = 1001,
        TryToConenct = 1002,
        AutoConnect  = 1003,
        Closing      = 1004,
    }

    public class NTcpException : Exception
    {
        public ErrorCode Code { get; private set; }

        public NTcpException(string message, ErrorCode code)
            :base(message)
        {
            this.Code = code;
        }
    }
}
