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
    /// 阿里妈妈服务
    /// </summary>
    public class AlimamaService
    {
        /// <summary>
        /// 阿里妈妈主站地址
        /// </summary>
        public const string AlimamaSiteUrl = GlobalContext.AlimamaSiteURL;

        /// <summary>
        /// 获取阿里妈妈的Cookie
        /// </summary>
        /// <returns></returns>
        public static DataContainer GetLoginTokenCookies()
        {
            var container = new DataContainer();
            var ckVistor = new LazyCookieVistor();
            var _PageCooies = ckVistor.LoadCookiesAsyc(AlimamaSiteUrl).Result;
            var json_cookies = JsonConvert.SerializeObject(_PageCooies);

            container.Result = json_cookies;


            return container;

        }
    }
}
