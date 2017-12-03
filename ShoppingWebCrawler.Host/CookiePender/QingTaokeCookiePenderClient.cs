using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using NTCPMessage.EntityPackage;

using Newtonsoft.Json;
using System.Threading.Tasks;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host.CookiePender
{
    /// <summary>
    /// 轻淘客 Cookie 刷新 客户端
    /// 从redis  查询获取登录后的Cookie
    /// </summary>
    public class QingTaokeCookiePenderClient : BaseCookiePenderClient
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public QingTaokeCookiePenderClient()
        {

        }

        /// <summary>
        /// 从远程客户端 获取轻淘客Cookie
        /// </summary>
        /// <returns></returns>

        public  List<CefCookie> GetCookiesFromRemoteServer()
        {

            List<CefCookie> cks = null;
            try
            {
                cks = this.GetCookiesFromRemoteRedisServer(GlobalContext.QingTaokeSiteName);

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
