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
 
    public abstract class BaseCookiePenderClient
    {
        
 

        /// <summary>
        /// 从远程客户端 获取平台Cookie
        /// </summary>
        /// <returns></returns>

        protected  virtual List<CefCookie> GetCookiesFromRemoteRedisServer(SupportPlatformEnum platform)
        {
            List<CefCookie> result = null;
           
            try
            {

                var cks_Platform = GlobalContext.DeskPullFromRedisCookies(platform);
                if (null == cks_Platform || cks_Platform.IsEmpty())
                {
                    return result;
                }

                result = cks_Platform;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;

        }


        protected virtual List<CefCookie> GetCookiesFromRemoteRedisServer(string otherPlatform)
        {
            List<CefCookie> result = null;

            try
            {

                var cks_Platform = GlobalContext.DeskPullFromRedisCookies(otherPlatform);
                if (null == cks_Platform || cks_Platform.IsEmpty())
                {
                    return result;
                }

                result = cks_Platform;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;

        }

    }
}
