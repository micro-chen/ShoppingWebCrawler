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
    ///京东搜索页面抓取
    /// </summary>
    public class JingdongWebPageService : BaseWebPageService
    {
        /// <summary>
        /// 京东获取关键词地址
        /// </summary>
        private const string templateOfSearchUrl = "https://search.jd.com/Search?keyword={0}&enc=utf-8&suggest=2.def.0.T17&wq={0}&pvid=e155e1803dba45d1b160a943ba803ea1";

        private const string jdSiteUrl= "https://www.jd.com/";

        /// <summary>
        /// 京东请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient jdHttpClient;


        /// <summary>
        /// 请求的地址
        /// </summary>
        protected override string TargetUrl
        {
            get; set;
        }

      

        public JingdongWebPageService()
        {
        }

        /// <summary>
        /// 静态构造函数 进行一次初始化
        /// </summary>
        static JingdongWebPageService()
        {
            //初始化头信息
           var  requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", jdSiteUrl);

            jdHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(jdHttpClient.Client.DefaultRequestHeaders, requestHeaders);

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
            var cks = GlobalContext.SupportPlatformsCookiesContainer[jdSiteUrl];
            jdHttpClient.ChangeGlobleCookies(cks, jdSiteUrl);

            string respText = this.QuerySearchContentResonseAsync(jdHttpClient.Client).Result;

            return respText;
        }


    }


}
