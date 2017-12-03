using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;


using Newtonsoft.Json;
using System.Threading.Tasks;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Cef.Core;
using NTCPMessage.EntityPackage;

namespace ShoppingWebCrawler.Host.CookiePender
{
    /// <summary>
    /// 阿里妈妈Cookie 刷新 客户端
    /// 从本地 ShoppingWebCrawler.Host.DeskTop UI server中  查询获取登录后的Cookie
    /// </summary>
    public class TaobaoCookiePenderClient : BaseCookiePenderClient
    {
     
        /// <summary>
        /// 构造函数
        /// </summary>
        public TaobaoCookiePenderClient()  
        {

        }

        /// <summary>
        /// 从远程客户端 获取阿里妈妈Cookie
        /// </summary>
        /// <returns></returns>

        public List<CefCookie> GetCookiesFromRemoteServer()
        {
            List<CefCookie> cks = null;
          
            try
            {
                cks = this.GetCookiesFromRemoteRedisServer(SupportPlatformEnum.Taobao);
                return cks;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return cks;

        }

     
    }
}
