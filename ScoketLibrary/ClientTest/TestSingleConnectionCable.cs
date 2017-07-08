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

namespace ClientTest
{
    class TestSingleConnectionCable
    {
        static byte[] buf;

        const int SyncTestCount = 100000;
        //const int SyncTestCount = 1000;

        const int AsyncTestCount = 100000000;
        //const int AsyncTestCount = 1000;

        static string _IPAddress;

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
            Console.Write("Please input serialize type:(0:none, 1:bin, 2:xml, 3:json, 4: simplebin, 5: struct, 6: customer)");
            string strSerializeType = Console.ReadLine();
            int serializeType = 0;
            int.TryParse(strSerializeType, out serializeType);

            ISerialize iSerializer = null;
            ISerialize<StructMessage> iStructMessageSerializer = null;
            ISerialize<TestMessage> iTestMessageSerializer = null;

            switch (serializeType)
            {
                case 0:
                    strSerializeType = "none";
                    break;
                case 1:
                    strSerializeType = "bin";
                    iSerializer = new BinSerializer();
                    
                    break;
                case 2:
                    strSerializeType = "xml";
                    iSerializer = new XMLSerializer(typeof(TestMessage));
                    break;
                case 3:
                    strSerializeType = "json";
                    iSerializer = new JsonSerializer(typeof(TestMessage));
                    break;
                case 4:
                    iSerializer = new SimpleBinSerializer(typeof(TestMessage));
                    strSerializeType = "simplebin";
                    break;
                case 5:
                    iStructMessageSerializer = new StructSerializer<StructMessage>();
                    strSerializeType = "struct";
                    break;
                case 6:
                    iTestMessageSerializer = new TestMessageSerializer();
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

                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            client.AsyncSend(10, buf);
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
                    TestMessage testMessage = new TestMessage()
                    {
                        Id = 1001,
                        Name = "0123456789",
                        Data = new byte[buf.Length]
                    };

                    StructMessage structMessage = new StructMessage()
                    {
                        Id = 1001,
                        Name = "0123456789",
                        //Url = "http://www.google.com",
                        //Site = "google.com",
                        Data = new byte[4]
                    };

                    for (int i = 0; i < testMessage.Data.Length; i++)
                    {
                        testMessage.Data[i] = (byte)i;
                    }

                    for (int i = 0; i < structMessage.Data.Length; i++)
                    {
                        structMessage.Data[i] = (byte)i;
                    }


                    sw.Start();

                    if (serializeType == 5)
                    {
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                client.AsyncSend<StructMessage>((UInt32)(20 + serializeType), ref structMessage, iStructMessageSerializer);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else if (serializeType == 6)
                    {
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                client.AsyncSend<TestMessage>((UInt32)(20 + serializeType), ref testMessage, iTestMessageSerializer);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                client.AsyncSend((UInt32)(20 + serializeType), testMessage, iSerializer);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
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
