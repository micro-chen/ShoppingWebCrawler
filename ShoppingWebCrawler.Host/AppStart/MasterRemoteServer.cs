using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using NTCPMessage.Client;
using NTCPMessage;
using Newtonsoft.Json;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.Server;
using NTCPMessage.Event;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.MessageConvert;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Common;


namespace ShoppingWebCrawler.Host.AppStart
{

    /// <summary>
    /// 开启套接字监听
    /// </summary>
    internal sealed class MasterRemoteServer
    {
        /// <summary>
        /// 集群最大从节点数
        /// </summary>
        public const int MaxSlaveNodeCount = 20;

        static NTCPMessage.Server.NTcpListener listener;

        static object _LockObj = new object();

        static List<UInt32> _Channels = new List<uint>();

        /// <summary>
        /// 从节点的端口集合
        /// 只有当注册完毕的端口 才会在这里接受注册端口集合
        /// </summary>
        static Dictionary<string, int> _slavePorts = new Dictionary<string, int>();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static MasterRemoteServer()
        {
            //主节点 也在服务端口内
            _slavePorts.Add("master", GlobalContext.MasterSocketPort);
        }
        /// <summary>
        /// 注册从节点端口，并返回可用的端口
        /// </summary>
        /// <param name="slaveIdentity"></param>
        public static int AddSlavePort(string slaveIdentity)
        {
            lock (_LockObj)
            {


                int port = -1;
                ///不得超过最大从节点阈值
                if (_slavePorts.Count < MaxSlaveNodeCount)
                {

                    if (_slavePorts.Keys.Count > 0)
                    {
                        port = _slavePorts.Values.Max() + 1;
                    }
                    else
                    {
                        port = GlobalContext.MasterSocketPort + 1;
                    }
                    int counuter = 1;
                    while (counuter <= 10)
                    {

                        bool isBeUsed = SocketHelper.IsUsedIPEndPoint(port);
                        if (isBeUsed == false)
                        {
                            break;//一旦发现合适端口 那么返回
                        }
                        else
                        {
                            port = port + counuter;
                        }
                        counuter++;
                    }
                    _slavePorts.Add(slaveIdentity, port);

                }
                return port;
            }
        }

        /// <summary>
        /// 获取一个可用的节点端口
        /// </summary>
        public static bool GetOneSlavePort(out int resultPort)
        {
            bool valid = false;

            //1 随机数;
            // 2轮询；（todo）
            // 3压力综合（todo）
            resultPort = -1;
            //随机数
            if (_slavePorts.Count > 0)
            {
                int pos = new Random(DateTime.Now.Millisecond).Next(0, _slavePorts.Count - 1);
                resultPort = _slavePorts.Values.ElementAt(pos);
                valid = true;
            }

            return valid;

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




        /// <summary>
        /// 默认的字符串消息转换器
        /// 提供字符串 cmd的 支持
        /// </summary>
        static DefaultMessageConvert defaultConvertProcesso = new DefaultMessageConvert();

        /// <summary>
        /// json 消息转换器
        /// </summary>
        static JsonMessageConvert jsonConvertProcessor = new MasterJsonMessageConvert();



        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {


            //注意 不支持 自定义格式的消息，因为自定义格式的消息 还需要传递 自定义格式转换器类型
            MessageType msgType = (MessageType)args.Event;
            switch (msgType)
            {
                case MessageType.None:
                    defaultConvertProcesso.ReceiveEventHandler(sender, args);
                    break;

                case MessageType.Json:
                    jsonConvertProcessor.ReceiveEventHandler(sender, args);
                    break;

                default:
                    string errMsg = string.Format("未能识别的消息格式，支持 1普通字符串  2 json格式！传入的格式为：{0}", msgType.ToString());
                    var ex = new Exception(errMsg);
                    Logger.Error(ex);
                    throw ex;


            }
            //Console.WriteLine("get event:{0}", args.Event);
        }

        static void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            //Logging.Logger.Info(string.Format("Remote socket:{0} disconnected.", args.RemoteIPEndPoint));
            //Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint);
        }

        static void ErrorEventHandler(object sender, ErrorEventArgs args)
        {
            Logger.Error(args.ErrorException);
            Console.WriteLine(args.ErrorException);
        }

        /// <summary>
        /// 异步向服务端发送消息
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        public static Task<int> RegisterSlaveToMasterAsync(string slaveIdentity)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = -1;
                bool isBeUsed = SocketHelper.IsUsedIPEndPoint(GlobalContext.MasterSocketPort);
                if (isBeUsed == true)
                {
                    // 发送ping 接受pong 后算是启动完毕
                    SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse("127.0.0.1"), GlobalContext.MasterSocketPort), 7);
                    ISerialize<SoapMessage> iSendMessageSerializer = new NTCPMessage.Serialize.JsonSerializer<SoapMessage>(); ;

                    try
                    {
                        //可以使用重载 设置连接超时时间
                        int timeOut = 20 * 1000;
                        client.Connect(timeOut);
                        var paras = new RegisterPortArgument { SlaveIdentity = slaveIdentity };
                        string msg = JsonConvert.SerializeObject(paras);
                        SoapMessage sopMsg = new SoapMessage()
                        {
                            Head = CommandConstants.CMD_RegisterSlavePort,
                            Body = msg
                        };

                        var repResult = client.SyncSend((UInt32)MessageType.Json,
                        sopMsg, timeOut,
                       iSendMessageSerializer);
                        if (repResult.Status == 1)
                        {

                            result = repResult.Result.ToInt();
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                    finally
                    {
                        client.Close();
                    }

                }

                return result;

            });


        }


        /// <summary>
        /// 主节点服务器是否启动
        /// </summary>
        /// <returns></returns>
        public static bool IsMasterStarted()
        {
            var result = false;
            bool isBeUsed = SocketHelper.IsUsedIPEndPoint(GlobalContext.MasterSocketPort);
            //if (isBeUsed == true)
            //{
            //    // 发送ping 接受pong 后算是启动完毕
            //    SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse("127.0.0.1"), GlobalContext.MasterSocketPort), 7);

            //    string data = string.Empty;
            //    try
            //    {
            //        //可以使用重载 设置连接超时时间
            //        client.Connect(5 * 1000);

            //        var buffer = Encoding.UTF8.GetBytes("ping");
            //        var resultBytes = client.SyncSend((UInt32)MessageType.None, buffer);
            //        data = Encoding.UTF8.GetString(resultBytes);
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.Error(ex);
            //    }
            //    finally
            //    {
            //        client.Close();
            //    }

            //    if (!string.IsNullOrEmpty(data) && result.Equals("pong"))
            //    {
            //        result = true;
            //        Logger.Info("MasterRemoteServer 探针检测服务端返回正确：pong .");

            //    }

            //}

            return isBeUsed;
            //return result;
        }



        /// <summary>
        /// 开启套接字监听
        /// </summary>

        public static void Start()
        {
            try
            {

                int port = GlobalContext.MasterSocketPort;
                //检查是否已经开启了端口
                bool isBeUsed = SocketHelper.IsUsedIPEndPoint(port);
                if (isBeUsed == true)
                {
                    return;
                }

                //开启 异步的启动任务，由于在内部进行了线程阻塞，所以task 永远不会complete

                listener = new NTCPMessage.Server.NTcpListener(new IPEndPoint(IPAddress.Any, port));
                listener.DataReceived += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
                listener.ErrorReceived += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
                listener.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

                listener.Listen();
            }
            catch (Exception ex)
            {

                Logger.Error(ex);
            }
            //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);




        }

        /// <summary>
        /// 终止tcp 服务端的监听
        /// </summary>
        public static void Stop()
        {
            if (null != listener)
            {
                listener.Close();
                listener = null;
            }
        }


    }

}
