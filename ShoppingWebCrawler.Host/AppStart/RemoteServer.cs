using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using NTCPMessage.Server;
using NTCPMessage.Event;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.MessageConvert;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.AppStart
{

    /// <summary>
    /// 开启套接字监听
    /// </summary>
    internal sealed class RemoteServer
    {


        static NTCPMessage.Server.NTcpListener listener;

        static object _LockObj = new object();

        static List<UInt32> _Channels = new List<uint>();



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
        static JsonMessageConvert jsonConvertProcessor = new JsonMessageConvert();



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
                    var ex= new Exception(errMsg);
                    Logger.WriteException(ex);
                    throw ex;
                  
                    
            }
            //Console.WriteLine("get event:{0}", args.Event);
        }

        static void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            //Logging.Logger.WriteToLog(string.Format("Remote socket:{0} disconnected.", args.RemoteIPEndPoint));
            //Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint);
        }

        static void ErrorEventHandler(object sender, ErrorEventArgs args)
        {
            Logger.WriteException(args.ErrorException);
            Console.WriteLine(args.ErrorException);
        }

        /// <summary>
        /// 开启套接字监听
        /// </summary>

        public static Task Start()
        {
           

            //开启 异步的启动任务，由于在内部进行了线程阻塞，所以task 永远不会complete
            return Task.Factory.StartNew(() =>
            {

                int port = GlobalContext.SocketPort;
                listener = new NTCPMessage.Server.NTcpListener(new IPEndPoint(IPAddress.Any, port));
                listener.DataReceived += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
                listener.ErrorReceived += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
                listener.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

                listener.Listen();

                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            });






        }

    }

}
