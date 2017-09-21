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
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.Compress;

using Newtonsoft.Json;
using System.Threading.Tasks;
using NTCPMessage;

namespace ShoppingWebCrawler.Client
{
    class TestClient
    {

        const int SyncTestCount = 100000;
        //const int SyncTestCount = 1000;

        const int AsyncTestCount = 1; //100000000;

        static string _IPAddress;
        static int _Port;


        public static void Test(string[] args)
        {

            string strSize = "64";

            Console.Write("Please input server IP Address (Default 127.0.0.1):");
            _IPAddress = Console.ReadLine().Trim().ToLower();

            if (_IPAddress == "")
            {
                _IPAddress = "127.0.0.1";
            }
            Console.Write("Please input server _Port (Default 10086):");
            string port = Console.ReadLine().Trim().ToLower();
            if (!int.TryParse(port, out _Port))
            {
                _Port = 10086;
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


            int count = AsyncTestCount;

            try
            {
                TestASyncMessage(count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }



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


        static void TestASyncMessage(int count)
        {
            Console.Write("Please input serialize type(Default 3:json)   : (3:json, 0:none)");
            string strSerializeType = Console.ReadLine();
            if (string.IsNullOrEmpty(strSerializeType))
            {
                strSerializeType = "3";
            }
            int serializeType = 0;
            int.TryParse(strSerializeType, out serializeType);

            ISerialize<SoapMessage> iSendMessageSerializer = null;
            ISerialize<DataContainer> iReturnDataSerializer = new NTCPMessage.Serialize.JsonSerializer<DataContainer>();

            switch (serializeType)
            {
                case 0:
                    strSerializeType = "none";
                    break;
                case 3:
                    strSerializeType = "json";
                    iSendMessageSerializer = new NTCPMessage.Serialize.JsonSerializer<SoapMessage>();
                    break;
                default:
                    serializeType = 0;
                    strSerializeType = "none";
                    break;
            }

            Console.WriteLine("Serialize type is {0}", strSerializeType);

            SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse(_IPAddress), _Port), 7);
            client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            client.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

            try
            {
                client.Connect();

                Stopwatch sw = new Stopwatch();

                Console.WriteLine("Test send message begin:");

                if (serializeType == 0)
                {
                    sw.Start();

                    //---------基本类型 字符串明文消息发送-----------
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var buffer = Encoding.UTF8.GetBytes("ping");
                            var resultBytes = client.SyncSend((UInt32)MessageType.None, buffer);

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
                    //var paras = new NTCPMessage.EntityPackage.Arguments.ETaoFetchWebPageArgument
                    //{
                    //    KeyWord = "洗面奶女"
                    //};
                    var paras = new YouhuiquanFetchWebPageArgument
                    {
                        ArgumentsForExistsList = new List<YouhuiquanFetchWebPageArgument.QuanArgument>
                         {
                              new YouhuiquanFetchWebPageArgument.QuanArgument
                              {
                                   SellerId=748501705,
                                   ItemId=540267461790
                              },
                               new YouhuiquanFetchWebPageArgument.QuanArgument
                              {
                                   SellerId=1690420968,
                                   ItemId=38002640105
                              },


                         }
                    };
                    string msg = JsonConvert.SerializeObject(paras);
                    SoapMessage testMessage = new SoapMessage()
                    {

                        Head = CommandConstants.CMD_FetchquanExistsList,
                        Body = msg
                    };



                    sw.Start();


                    try
                    {
                        //模拟并发

                        for (int i = 0; i < count; i++)
                        {

                            var repResult = client.SyncSend((UInt32)MessageType.Json,
                            testMessage, 1800000,
                           iSendMessageSerializer);


                            if (null != repResult)
                            {
                                // string content = LZString.DecompressFromBase64(repResult.Result);
                                //Console.Write(content);
                                Console.WriteLine("from server response :{0}", repResult.Result);
                            }
                        }


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
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

    }

}
