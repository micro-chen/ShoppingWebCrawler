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
using ShoppingWebCrawler.Host.Common.Common;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Headless;

namespace ShoppingWebCrawler.Host.AppStart
{

    /// <summary>
    /// 开启 子节点监听
    /// </summary>
    internal sealed class SlaveRemoteServer
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
        /// 开启套接字监听
        /// </summary>

        public static void StartAsync(HeadLessWebBrowerApp app)
        {
            Task.Run(() =>
            {

                try
                {
                    if (false == GlobalContext.IsClusteringMode)//是否开启集群模式
                    {
                        return;
                    }
                    if (null == app)
                    {
                        return;
                    }
                    //一旦主控节点开启并正确返回结果
                    if (!MasterRemoteServer.IsMasterStarted())
                    {
                        return;
                    }


                    int port = 0;

                    var slaveIdentity = Guid.NewGuid().ToString().ToLower();
                    //开启监听前 ,发送注册当前从节点到主节点，如果可以登记注册成功，那么服务端分配端口
                    port = MasterRemoteServer.RegisterSlaveToMasterAsync(slaveIdentity).Result;
                    if (port <= 0)
                    {
                        return;//一旦服务端返回无效端口 那么禁止从节点启动监听
                    }
                    listener = new NTCPMessage.Server.NTcpListener(new IPEndPoint(IPAddress.Any, port));
                    listener.DataReceived += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
                    listener.ErrorReceived += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
                    listener.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

                    GlobalContext.IsInSlaveMode = true;//标识正在从节点下工作

                    //开启从节点的监听
                    listener.Listen();
                }

                catch (Exception ex)
                {
                    Logger.Error(ex);
                }




            });

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

    public class CookiesSetTask : CefTask
    {
        private readonly IList<Cookie> _cookies;

        internal CookiesSetTask(IList<Cookie> cookies)
        {
            _cookies = cookies;
        }

        protected override void Execute()
        {

            foreach (var cookie in _cookies)
            {
                CefCookieManager.GetGlobal(null).SetCookie(cookie.Domain, new CefCookie()
                {
                    Creation = cookie.TimeStamp,
                    Domain = cookie.Domain,
                    Expires = cookie.Expires,
                    HttpOnly = cookie.HttpOnly,
                    LastAccess = cookie.TimeStamp,
                    Name = cookie.Name,
                    Path = cookie.Path,
                    Secure = cookie.Secure,
                    Value = cookie.Value
                }, null


                );
            }
        }
    }

    //public class MyCefTask : CefTask
    //{
    //    public CefCookieManager MyProperty { get; set; }
    //    protected override void Execute()
    //    {
    //        //MyProperty = GlobalContext.DefaultCEFGlobalCookieManager;

    //    }
    //}

}
