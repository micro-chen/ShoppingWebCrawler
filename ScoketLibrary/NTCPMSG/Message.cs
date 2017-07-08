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

namespace NTCPMessage
{
    public class Message
    {
        const int MessageHeadSize = 16;

        const int ThreeBytes = 256 * 256 * 256;
        const int MaxDataLength = 256 * 256 * 256;

        public const byte Sync0 = 0xA5;
        public const byte Sync1 = 0xA5;
        public MessageFlag Flag;
        public UInt32 Event;
        public UInt16 CableId;
        public UInt32 Channel;
        public byte[] Data;

        public Message(MessageFlag flag, UInt32 evt, UInt16 cableId, 
            UInt32 channel, byte[] data)
        {
            this.Flag = flag;
            this.Event = evt;
            this.CableId = cableId;
            this.Channel = channel;
            this.Data = data;
        }

        /// <summary>
        /// Write to buffer
        /// </summary>
        /// <param name="buffer">buffer input</param>
        /// <param name="offset">offset</param>
        /// <returns>new position</returns>
        public int WriteToBuffer(ref byte[] buffer, int offset)
        {
            int bufferLen = buffer.Length;

            if (this.Data.Length + MessageHeadSize + offset > bufferLen)
            {
                byte[] temp = buffer;

                buffer = new byte[this.Data.Length + MessageHeadSize + offset + 1024];

                Array.Copy(temp, buffer, temp.Length);

                bufferLen = buffer.Length;
            }

            //Sync head
            buffer[offset++] = Sync0;
            buffer[offset++] = Sync1;

            //Flag
            buffer[offset++] = (byte)Flag;

            //Flag

            //Event
            buffer[offset++] = (byte)(Event / ThreeBytes);
            buffer[offset++] = (byte)((Event % ThreeBytes) / 65536);
            buffer[offset++] = (byte)((Event % 65536) / 256);
            buffer[offset++] = (byte)(Event % 256);

            //CableId
            buffer[offset++] = (byte)(CableId / 256);
            buffer[offset++] = (byte)(CableId % 256);

            //Channel
            buffer[offset++] = (byte)(Channel / ThreeBytes);
            buffer[offset++] = (byte)((Channel % ThreeBytes) / 65536);
            buffer[offset++] = (byte)((Channel % 65536) / 256);
            buffer[offset++] = (byte)(Channel % 256);

            //Length
            int len = Data.Length;
            buffer[offset++] = (byte)(len / 65536);
            buffer[offset++] = (byte)((len % 65536) / 256);
            buffer[offset++] = (byte)(len % 256);

            //Data
            Array.Copy(Data, 0, buffer, offset, Data.Length);
            offset += Data.Length;
            return offset;
        }


        public void WriteToStream(System.IO.Stream stream)
        {
            //Sync head
            stream.WriteByte(Sync0);
            stream.WriteByte(Sync1);
            
            //Flag
            stream.WriteByte((byte)Flag);
            //Flag

            //Event
            stream.WriteByte((byte)(Event / ThreeBytes));
            stream.WriteByte((byte)((Event % ThreeBytes) / 65536));
            stream.WriteByte((byte)((Event % 65536) / 256));
            stream.WriteByte((byte)(Event % 256));

            //CableId
            stream.WriteByte((byte)(CableId / 256));
            stream.WriteByte((byte)(CableId % 256));

            //Channel
            stream.WriteByte((byte)(Channel / ThreeBytes));
            stream.WriteByte((byte)((Channel % ThreeBytes) / 65536));
            stream.WriteByte((byte)((Channel % 65536) / 256));
            stream.WriteByte((byte)(Channel % 256));

            //Length
            int len = Data.Length;
            stream.WriteByte((byte)(len / 65536));
            stream.WriteByte((byte)((len % 65536) / 256));
            stream.WriteByte((byte)(len % 256));

            //Data
            stream.Write(Data, 0, Data.Length);
        }

    }
}
