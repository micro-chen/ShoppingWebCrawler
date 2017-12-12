using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ShoppingWebCrawler.Host.Common.Http;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.PlatformCrawlers
{
    public  class BaseRequest
    {


        /// <summary>
        /// 通用的请求头信息 
        /// 静态公共头信息  由于是静态头信息，所以不要添加键值对
        /// 直接使用键 访问修改值即可
        /// </summary>
        public static NameValueCollection GetCommonRequestHeaders(bool isMobile=false)
        {

            var commonRequestHeaders = new NameValueCollection();
            commonRequestHeaders.Add("Accept", "*/*");
            commonRequestHeaders.Add("Cache-Control", "no-cache");
            commonRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
            commonRequestHeaders.Add("Connection", "Keep-Alive");
            if (isMobile==true)
            {
                //移动端UA
                commonRequestHeaders.Add("User-Agent", GlobalContext.MobileUserAgent);
            }
            else
            {
                //PC 端UA
                commonRequestHeaders.Add("User-Agent", GlobalContext.ChromeUserAgent);

            }

            return commonRequestHeaders;
        }




    }
}
