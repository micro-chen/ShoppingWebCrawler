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
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.WindowService.App_Start;
using System.IO;

namespace ShoppingWebCrawler.Host.WindowService.ScheduleTasks
{
    /// <summary>
    /// 客户端探针
    /// 发送心跳包 到本地，监视tcp端口的健康情况
    /// </summary>
    public class RemoteTcpTestClient
    {



        static string _IPAddress = "127.0.0.1";//本机
        static int _Port = 0;//
        /// <summary>
        /// tcp 远程客户端的端口
        /// </summary>
        public static int Port
        {
            get
            {
                if (_Port<=0)
                {
                    string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "TCPServerConf.config");
                    var _DefaultSocketPort = ConfigHelper.GetConfigFromConfigFile(configPath, "Port");
                    if (!string.IsNullOrEmpty(_DefaultSocketPort))
                    {
                        _Port = int.Parse(_DefaultSocketPort);
                    }
                    else
                    {
                        _Port = 10086;
                    }
                }
                
                return _Port;
            }

            set
            {
                _Port = value;
            }
        }


        /// <summary>
        /// 探针发送ping 心跳包
        /// </summary>
        public static bool TestPingSendMessage()
        {
            bool isServerHelthOK = false;
            //client
            SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse(_IPAddress), Port), 7);
            //client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            //client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            //client.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);


            //---------基本类型 字符串明文消息发送-----------
            string result = string.Empty;
            try
            {
                //可以使用重载 设置连接超时时间
                client.Connect(10*1000);

                var buffer = Encoding.UTF8.GetBytes("ping");
                var resultBytes = client.SyncSend((UInt32)MessageType.None, buffer);
                result = Encoding.UTF8.GetString(resultBytes);

            }
            catch (Exception ex)
            {
                WinServiceConfig.Logger.Error(ex);
            }
            finally
            {
                client.Close();
            }

            if (!string.IsNullOrEmpty(result) && result.Equals("pong"))
            {
                WinServiceConfig.Logger.Info("探针检测服务端返回正确：pong .");
                isServerHelthOK = true;
            }

            return isServerHelthOK;
        }

    }

}
