using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using NTCPMessage.Client;
using NTCPMessage.Event;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

using Newtonsoft.Json;

namespace ClientTest
{
    class TestSingleConnectionCable
    {
        static byte[] buf;

        const int SyncTestCount = 100000;
        //const int SyncTestCount = 1000;

        const int AsyncTestCount = 1; //100000000;
        //const int AsyncTestCount = 1000;

        static string _IPAddress;

        /// <summary>
        /// 注意 这种数据接收 是接收服务器推送来的数据
        /// 发送请求响应模式 必须用同步发送方法
        /// 异步 是单向通信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            Console.WriteLine("get event:{0}", args.Event);
        }

        static void ErrorEventHandler(object sender, ErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorException);
        }

        static void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            Console.WriteLine("Disconnect from {0}", args.RemoteIPEndPoint);
        }

        static void TestSyncMessage(object state)
        {
            SingleConnectionCable client = (SingleConnectionCable)state;
            int count = SyncTestCount;

            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Test sync message");
            sw.Start();

            try
            {
                for (int i = 0; i < count; i++)
                {
                    client.SyncSend(11, buf, 60000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            sw.Stop();
            Console.WriteLine("Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);
        }

        static void TestASyncMessage(int count)
        {
            Console.Write("Please input serialize type:(0:none, 1:bin, 2:xml, 3:json, 4: simplebin, 5: customer)");
            string strSerializeType = Console.ReadLine();
            int serializeType = 0;
            int.TryParse(strSerializeType, out serializeType);

            ISerialize<SoapMessage> iSendMessageSerializer = null;
            ISerialize<DataResultContainer<string>> iReturnDataSerializer = new NTCPMessage.Serialize.JsonSerializer<DataResultContainer<string>>();

            switch (serializeType)
            {
                case 0:
                    strSerializeType = "none";
                    break;
                case 1:
                    strSerializeType = "bin";
                    iSendMessageSerializer = new BinSerializer<SoapMessage>();

                    break;
                case 2:
                    strSerializeType = "xml";
                    iSendMessageSerializer = new XMLSerializer<SoapMessage>();
                    break;
                case 3:
                    strSerializeType = "json";
                    iSendMessageSerializer = new NTCPMessage.Serialize.JsonSerializer<SoapMessage>();
                    break;
                case 4:
                    iSendMessageSerializer = new SimpleBinSerializer<SoapMessage>();
                    strSerializeType = "simplebin";
                    break;
                case 5:
                    iSendMessageSerializer = new SoapMessageSerializer();
                    strSerializeType = "customer";
                    break;

                default:
                    serializeType = 0;
                    strSerializeType = "none";
                    break;
            }

            Console.WriteLine("Serialize type is {0}", strSerializeType);

            SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse(_IPAddress), 2500), 7);
            client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            client.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

            try
            {
                client.Connect();

                Stopwatch sw = new Stopwatch();

                Console.WriteLine("Test async message");

                if (serializeType == 0)
                {
                    sw.Start();

                    //---------基本类型 字符串明文消息发送-----------
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var buffer = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
                            var resultBytes = client.SyncSend(10, buffer);

                            var str = Encoding.UTF8.GetString(resultBytes);

                            Console.WriteLine(str);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    sw.Stop();
                    Console.WriteLine("Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);
                }
                else
                {

                    ///标准soap消息发送
                    var obj = new { spid = 1, ValueType = 9999 };
                    string msg = JsonConvert.SerializeObject(obj);
                    SoapMessage testMessage = new SoapMessage()
                    {

                        Head = "student",
                        Body = msg
                    };



                    sw.Start();


                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var repResult = client.SyncSend((UInt32)(20 + serializeType),
                                testMessage,
                                300000,
                                iSendMessageSerializer);

                            if (null != repResult)
                            {
                                Console.WriteLine("from server response :{0}", repResult.Status);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    //else
                    //{
                    //    try
                    //    {
                    //        for (int i = 0; i < count; i++)
                    //        {
                    //            //client.AsyncSend((UInt32)(20 + serializeType), testMessage, iSerializer);
                    //            var repResult = client.SyncSend((UInt32)(20 + serializeType),ref testMessage, 88888888, iSendMessageSerializer, iReturnDataSerializer);

                    //            if (null != repResult)
                    //            {
                    //                Console.WriteLine("from server response :{0}", repResult.Status);
                    //            }
                    //        }
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e);
                    //    }
                    //}
                    sw.Stop();
                    Console.WriteLine("Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            finally
            {
                client.Close();
            }
        }

        public static void Test(string[] args)
        {
            Console.WriteLine("Start to test SigleConnectionCable");

            Console.Write("Please input package size:");
            string strSize = Console.ReadLine();

            Console.Write("Test Sync message? y /n :");
            string testSyncMessage = Console.ReadLine().Trim().ToLower();

            Console.Write("Please input server IP Address:");
            _IPAddress = Console.ReadLine().Trim().ToLower();

            if (_IPAddress == "")
            {
                _IPAddress = "127.0.0.1";
            }

            int packageSize;

            if (!int.TryParse(strSize, out packageSize))
            {
                packageSize = 64;
            }

            if (packageSize < 0)
            {
                packageSize = 0;
            }

            Console.WriteLine("IPAddress = {0}", _IPAddress);
            Console.WriteLine("Package size = {0}", packageSize);

            buf = new byte[packageSize];
            int count = AsyncTestCount;
            //int count = 10000;

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)i;
            }

            try
            {
                if (testSyncMessage == "y")
                {
                    Console.Write("Please input test thread number:");
                    string strThreadNumber = Console.ReadLine();
                    int threadNumber;

                    if (!int.TryParse(strThreadNumber, out threadNumber))
                    {
                        threadNumber = 1;
                    }

                    Console.WriteLine("Actual test thread number = {0}", threadNumber);

                    SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse(_IPAddress), 2500), 1);

                    client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
                    client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
                    client.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

                    client.Connect();

                    for (int i = 0; i < threadNumber; i++)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(TestSyncMessage);
                        thread.IsBackground = true;
                        thread.Start(client);
                    }
                }
                else
                {
                    TestASyncMessage(count);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }

}
