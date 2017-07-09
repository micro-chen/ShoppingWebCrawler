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
using NTCPMessage.MessageConvert;

namespace ShoppingWebCrawler.Host.AppStart
{

    /// <summary>
    /// 开启套接字监听
    /// </summary>
    internal sealed  class RemoteServer
    {


        static NTCPMessage.Server.NTcpListener listener;
        static Thread _Thread;

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
        /// </summary>
        static DefaultMessageConvert _sDefaultConvert = new DefaultMessageConvert();
        /// <summary>
        /// 二进制消息转换器
        /// </summary>
        static BinMessageConvert _sBinConvert = new BinMessageConvert();
        /// <summary>
        /// xml 消息转换器
        /// </summary>
        static XmlMessageConvert _sXmlConvert = new XmlMessageConvert();

        /// <summary>
        /// json 消息转换器
        /// </summary>
        static JsonMessageConvert _sJsonConvert = new JsonMessageConvert();


        /// <summary>
        /// 简单二进制消息转换器
        /// </summary>
        static SimpleBinMessageConvert _sSimpleConvert = new SimpleBinMessageConvert();

        /// <summary>
        /// 自定义的消息转换器
        /// </summary>
        static CustomerSoapMessageConvert _sCustomConvert = new CustomerSoapMessageConvert();


        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {

            //注意 不支持 自定义格式的消息，因为自定义格式的消息 还需要传递 自定义格式转换器类型
            switch ((MessageType)args.Event)
            {
                case MessageType.None:
                    _sDefaultConvert.ReceiveEventHandler(sender, args);
                    break;
                case MessageType.Bin:
                    _sBinConvert.ReceiveEventHandler(sender, args);
                    break;
                case MessageType.Xml:
                    _sXmlConvert.ReceiveEventHandler(sender, args);
                    break;
                case MessageType.Json:
                    _sJsonConvert.ReceiveEventHandler(sender, args);
                    break;
                case MessageType.SimpleBin:
                    _sSimpleConvert.ReceiveEventHandler(sender, args);
                    break;
                case MessageType.Customer:
                    _sCustomConvert.ReceiveEventHandler(sender, args);
                    break;

                default:
                    break;
            }
            //Console.WriteLine("get event:{0}", args.Event);
        }

        static void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            Logging.Logger.WriteToLog(string.Format("Remote socket:{0} disconnected.", args.RemoteIPEndPoint));
            Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint);
        }

        static void ErrorEventHandler(object sender, ErrorEventArgs args)
        {
            Logging.Logger.WriteException(args.ErrorException);
            Console.WriteLine(args.ErrorException);
        }

        /// <summary>
        /// 开启套接字监听
        /// </summary>

        public static void Start()
        {
            if (System.IO.File.Exists("channel.txt"))
            {
                System.IO.File.Delete("channel.txt");
            }


            int port = GlobalContext.SocketPort;
            listener = new NTCPMessage.Server.NTcpListener(new IPEndPoint(IPAddress.Any, port));
            listener.DataReceived += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            listener.ErrorReceived += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            listener.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

            listener.Listen();

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

    }

}
