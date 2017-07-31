using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using Newtonsoft.Json;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.DeskTop.Services
{
    /// <summary>
    /// 淘宝服务
    /// </summary>
    public class TaobaoService
    {

        /// <summary>
        /// 获取淘宝的Cookie
        /// </summary>
        /// <returns></returns>
        public static DataContainer GetLoginTokenCookies()
        {
            var container = new DataContainer();
            var ckVistor = new LazyCookieVistor();
            var _PageCooies = ckVistor.LoadCookiesAsyc(GlobalContext.TaobaoSiteURL).Result;
            var json_cookies = JsonConvert.SerializeObject(_PageCooies);

            container.Result = json_cookies;


            return container;

        }
    }
}
