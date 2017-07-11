using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.Net.Http;
using ShoppingWebCrawler.Host.Http;
using System.Net;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 一号店搜索页面抓取
    /// </summary>
    public class YiHaoDianWebPageService : BaseWebPageService
    {

        private const string yhdSiteUrl = "http://www.yhd.com/";

        private const string templateOfSearchUrl = "http://search.yhd.com/c0-0/k{0}";
        /// <summary>
        /// 天猫请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient yhdHttpClient;


        /// <summary>
        /// 请求地址-根据方法传递的参数 动态格式化
        /// </summary>
        protected override string TargetUrl
        {
            get;set;
        }



        

        public YiHaoDianWebPageService()
        {
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static YiHaoDianWebPageService()
        {
            //初始化头信息
            var requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", yhdSiteUrl);
            yhdHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(yhdHttpClient.Client.DefaultRequestHeaders, requestHeaders);

        }
        /// <summary>
        /// 查询网页
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public override string QuerySearchContent(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                return null;
            }
            //格式化一个查询地址
           
            this.TargetUrl = string.Format(templateOfSearchUrl, keyWord);

            //获取当前站点的Cookie
            CookieCollection cks = GlobalContext.SupportPlatformsCookiesContainer[yhdSiteUrl];
            yhdHttpClient.ChangeGlobleCookies(cks, yhdSiteUrl);

            string respText = this.QuerySearchContentResonseAsync(yhdHttpClient.Client).Result;

            return respText;
        }


    }
}
