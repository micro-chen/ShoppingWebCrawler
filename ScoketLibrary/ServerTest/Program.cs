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
using System.Threading;

using NTCPMessage.Server;
using NTCPMessage.Event;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

namespace ServerTest
{
    class Program
    {
        class BinMessageParse : MessageParse
        {
            public BinMessageParse()
                : base(new BinSerializer(), new BinSerializer())
            {

            }

            public override object ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, object obj)
            {
                //Console.WriteLine(obj);

                return null;
            }
        }

        class XmlTestMessageParse : MessageParse
        {
                       
            public XmlTestMessageParse()
                : base(new XMLSerializer(typeof(TestMessage)), new XMLSerializer(typeof(string)))
            {

            }

            public override object ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, object obj)
            {
                //Console.WriteLine(obj);

                return null;
            }
        }

        class JsonTestMessageParse : MessageParse
        {

            public JsonTestMessageParse()
                : base(new JsonSerializer(typeof(TestMessage)), new JsonSerializer(typeof(string)))
            {

            }

            public override object ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, object obj)
            {
                //Console.WriteLine(obj);

                return null;
            }
        }

        class SimpleBinTestMessageParse : MessageParse
        {

            public SimpleBinTestMessageParse()
                : base(new SimpleBinSerializer(typeof(TestMessage)), new SimpleBinSerializer(typeof(string)))
            {

            }

            public override object ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, object obj)
            {
                //Console.WriteLine(obj);

                return null;
            }
        }

        class StructMessageParse : MessageParse<int, StructMessage>
        {
            /// <summary>
            /// Constractor
            /// </summary>
            /// <param name="dataSerializer">serializer for input data</param>
            /// <param name="returnSerializer">serializer for return data</param>
            public StructMessageParse()
                : base(new StructSerializer<StructMessage>(), new StructSerializer<int>())
            {
            }

            public override int ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, StructMessage obj)
            {
                //Console.WriteLine(obj);

                return 0;
            }
        }

        class TestMessageParse : MessageParse<string, TestMessage>
        {
            /// <summary>
            /// Constractor
            /// </summary>
            /// <param name="dataSerializer">serializer for input data</param>
            /// <param name="returnSerializer">serializer for return data</param>
            public TestMessageParse()
                : base(new TestMessageSerializer(), new XMLSerializer<string>())
            {
            }

            public override string ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, TestMessage obj)
            {
                //Console.WriteLine(obj);

                return null;
            }
        }


        static NTCPMessage.Server.NTcpListener listener;
        static Thread _Thread;

        static object _LockObj = new object();

        static long _CurBytes = 0;
        static long _TotalCount = 0;

        static List<UInt32> _Channels = new List<uint>();

        static long TotalCount
        {
            get
            {
                lock (_LockObj)
                {
                    return _TotalCount;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _TotalCount = value;
                }
            }
        }

        static long CurBytes
        {
            get
            {
                lock (_LockObj)
                {
                    return _CurBytes;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _CurBytes = value;
                }
            }
        }

        static void AddChannel(UInt32 channel)
        {
            lock (_LockObj)
            {
                _Channels.Add(channel);
            }
        }

        static void SaveChannel(string filePath)
        {
            lock (_LockObj)
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Append,
                     System.IO.FileAccess.Write))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                    {
                        foreach (UInt32 channel in _Channels)
                        {
                            sw.WriteLine(channel);
                        }
                        _Channels.Clear();

                    }

                }

            }
        }

        static void IncCurBytes(long value)
        {
            lock (_LockObj)
            {
                _CurBytes += value;
            }
        }

        static long _CurCount = 0;
        static long CurCount
        {
            get
            {
                lock (_LockObj)
                {
                    return _CurCount;
                }
            }

            set
            {
                lock (_LockObj)
                {
                    _CurCount = value;
                }
            }
        }

        static void IncCurCount(long value)
        {
            lock (_LockObj)
            {
                _CurCount += value;
                _TotalCount += value;
            }
        }

        static void Statistic()
        {
            while (true)
            {
                long bytes = CurBytes;
                CurBytes = 0;

                long count = CurCount;
                CurCount = 0;

                Console.WriteLine("Get {0} count, {1}KB, total:{2}, Remote Socket number:{3}",
                    count, bytes / 1024, TotalCount, listener.GetRemoteEndPoints().Length);

                //SaveChannel("channel.txt");
                Thread.Sleep(1000);
            }
        }

        static BinMessageParse _sBinParse = new BinMessageParse();
        static XmlTestMessageParse _sXmlParse = new XmlTestMessageParse();
        static JsonTestMessageParse _sJsonParse = new JsonTestMessageParse();
        static SimpleBinTestMessageParse _sSimpleParse = new SimpleBinTestMessageParse();
        static StructMessageParse _sStructMessageParse = new StructMessageParse();
        static TestMessageParse _sCustomParse = new TestMessageParse();

        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            IncCurBytes(args.Data.Length);
            IncCurCount(1);

            switch (args.Event)
            {
                case 10:
                    break;
                case 11:
                    //args.ReturnData = new byte[1];
                    //AddChannel(args.Channel);
                    break;
                case 21:
                    _sBinParse.ReceiveEventHandler(sender, args);
                    break;
                case 22:
                    _sXmlParse.ReceiveEventHandler(sender, args);
                    break;
                case 23:
                    _sJsonParse.ReceiveEventHandler(sender, args);
                    break;
                case 24:
                    _sSimpleParse.ReceiveEventHandler(sender, args);
                    break;
                case 25:
                    _sStructMessageParse.ReceiveEventHandler(sender, args);
                    break;
                case 26:
                    _sCustomParse.ReceiveEventHandler(sender, args);
                    break;
                default:
                    break;
            }
            //Console.WriteLine("get event:{0}", args.Event);
        }

        static void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint);
        }

        static void ErrorEventHandler(object sender, ErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorException);
        }

        static void Main(string[] args)
        {
            if (System.IO.File.Exists("channel.txt"))
            {
                System.IO.File.Delete("channel.txt");
            }

            _Thread = new Thread(Statistic);
            _Thread.IsBackground = true;

            listener = new NTCPMessage.Server.NTcpListener(new IPEndPoint(IPAddress.Any, 2500));
            listener.DataReceived += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            listener.ErrorReceived += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            listener.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);
            _Thread.Start();

            listener.Listen();

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}
