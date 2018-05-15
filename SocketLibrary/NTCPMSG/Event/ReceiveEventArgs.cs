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
    public class ReceiveEventArgs : EventArgs
    {
        public int SCBID { get; private set; }

        public EndPoint RemoteIPEndPoint { get; private set; }

        public MessageFlag Flag { get; private set; }

        public UInt16 CableId { get; private set; }

        public UInt32 Channel { get; private set; }

        public UInt32 Event { get; private set; }

        public byte[] Data { get; private set; }

        public byte[] ReturnData { get; set; }


        public ReceiveEventArgs(int scbId, EndPoint remoteIPEndPoint, MessageFlag flag, UInt32 evt, UInt16 cableId, UInt32 channel, byte[] data)
        {
            SCBID = scbId;
            RemoteIPEndPoint = remoteIPEndPoint;
            Flag = flag;
            Event = evt;
            CableId = cableId;
            Channel = channel;
            Data = data;
            ReturnData = null;
        }
    }
}
