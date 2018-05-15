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

namespace NTCPMessage.Event
{
    public class Delegates
    {
        internal delegate void DeleOnInnerBatchReceive(SCB scb, List<ReceiveEventArgs> argsList);

        internal delegate void DeleOnInnerReceive(SCB scb, MessageFlag flag, UInt32 evt, UInt16 cableId, UInt32 channel, byte[] data);

        public delegate void DeleOnError(string func, Exception e);

        public delegate void DeleOnReceive(UInt16 evt, byte[] data, out byte[] retData);

        internal delegate void DeleOnDisconnect(SCB scb);
    }


}
