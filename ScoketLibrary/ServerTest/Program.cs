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
using ServerTest.MessageConvert;

namespace ServerTest
{
    class Program
    {
      

        




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

        static DefaultMessageConvert _sDefaultMsgConvert = new DefaultMessageConvert();
        static BinMessageConvert _sBinConvert = new BinMessageConvert();
        static XmlMessageConvert _sXmlConvert = new XmlMessageConvert();
        static JsonMessageConvert _sJsonConvert = new JsonMessageConvert();
        static SimpleBinMessageConvert _sSimpleConvert = new SimpleBinMessageConvert();
        static CustomerSoapMessageConvert _sCustomConvert = new CustomerSoapMessageConvert();

        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            IncCurBytes(args.Data.Length);
            IncCurCount(1);

            switch (args.Event)
            {
                case 10:
                    _sDefaultMsgConvert.ReceiveEventHandler(sender, args);
                    break;
                case 11:
                    //args.ReturnData = new byte[1];
                    //AddChannel(args.Channel);
                    break;
                case 21:
                    _sBinConvert.ReceiveEventHandler(sender, args);
                    break;
                case 22:
                    _sXmlConvert.ReceiveEventHandler(sender, args);
                    break;
                case 23:
                    _sJsonConvert.ReceiveEventHandler(sender, args);
                    break;
                case 24:
                    _sSimpleConvert.ReceiveEventHandler(sender, args);
                    break;
                case 25:
                    _sCustomConvert.ReceiveEventHandler(sender, args);
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
