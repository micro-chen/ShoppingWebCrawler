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
using NTCPMessage.Compress;

using Newtonsoft.Json;
using System.Threading.Tasks;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host.CookiePender
{
    /// <summary>
    /// 阿里妈妈Cookie 刷新 客户端
    /// 从本地 ShoppingWebCrawler.Host.DeskTop UI server中  查询获取登录后的Cookie
    /// </summary>
    public class AlimamaCookiePenderClient : BaseSoapTcpClient
    {
        /// <summary>
        /// 获取远程Cookie的本地server 端口
        /// </summary>
        private const int remotePort = 20086;
        /// <summary>
        /// 构造函数
        /// </summary>
        public AlimamaCookiePenderClient() : base("127.0.0.1", remotePort)
        {

        }

        /// <summary>
        /// 从远程客户端 获取阿里妈妈Cookie
        /// </summary>
        /// <returns></returns>

        public List<CefCookie> GetCookiesFromRemoteServer()
        {
            List<CefCookie> result = null;
            var paras = new SoapMessage() { Head = "alimamatoken" };
            try
            {


                var data = this.SendSoapMessage(paras);
                if (null == data||string.IsNullOrEmpty(data.Result))
                {
                    return result;
                }

                result = JsonConvert.DeserializeObject<List<CefCookie>>(data.Result);

            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }

            return result;

        }

     
    }
}
