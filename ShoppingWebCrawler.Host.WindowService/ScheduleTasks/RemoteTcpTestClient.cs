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
using System.IO;
using ShoppingWebCrawler.Host.Common.Logging;

namespace ShoppingWebCrawler.Host.WindowService.ScheduleTasks
{
    /// <summary>
    /// 客户端探针
    /// 发送心跳包 到本地，监视tcp端口的健康情况
    /// </summary>
    public class RemoteTcpTestClient
    {



        private static string _IPAddress = "127.0.0.1";//本机

        public static string IPAddress
        {
            get
            {
                return _IPAddress;
            }
        }

        private static int _Port = 10086;
        /// <summary>
        /// tcp 远程客户端的端口
        /// </summary>
        public static int Port
        {
            get
            {
                if (_Port <= 0)
                {
                    string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "TCPServerConf.config");
                    var _DefaultSocketPort = ConfigHelper.GetConfigFromConfigFile(configPath, "Port");
                    if (!string.IsNullOrEmpty(_DefaultSocketPort))
                    {
                        _Port = int.Parse(_DefaultSocketPort);
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

            try
            {

                //1 首先检查蜘蛛进程是否运行 如果全部蜘蛛进程都挂掉 那么立刻返回false
                var isCrawlerRun = ShoppingWebCrawlerHostService.IsWebCrawlerHostProcessRunnning;
                if (false==isCrawlerRun)
                {
                    return isServerHelthOK;
                }

                // 2 如果已经有了蜘蛛进程，那么监视主进程是否处于激活
                using (var conn = new SoapTcpConnection(IPAddress, Port,5))
                {
                    var pingResult = conn.Ping();
                    if (pingResult == true)
                    {
                        Logger.Info("探针检测服务端返回正确：pong .");
                    }
                    isServerHelthOK = pingResult;
                }
            }
            catch (Exception ex)
            {

                Logger.Error(ex);
            }

            return isServerHelthOK;

        }

    }

}
