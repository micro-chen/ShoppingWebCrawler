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
    /// 天猫搜索页面抓取
    /// </summary>
    public class TmallWebPageService : BaseWebPageService
    {

        private const string tmallSiteUrl = "https://www.tmall.com/";

        private const string templateOfSearchUrl = "https://list.tmall.com/search_product.htm?q={0}&type=p&vmarket=&spm=875.7931836%2FB.a2227oh.d100&xl=ip_1&from=mallfp..pc_1_suggest";
        /// <summary>
        /// 天猫请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient tmallHttpClient;


        /// <summary>
        /// 请求地址-根据方法传递的参数 动态格式化
        /// </summary>
        protected override string TargetUrl
        {
            get;set;
        }



        

        public TmallWebPageService()
        {
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static TmallWebPageService()
        {
            //初始化头信息
            var requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", tmallSiteUrl);
            tmallHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(tmallHttpClient.Client.DefaultRequestHeaders, requestHeaders);

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
            CookieContainer ckContainer = GlobalContext.SupportPlatformsCookiesContainer[tmallSiteUrl];
            tmallHttpClient.Cookies = ckContainer;

            string respText = this.QuerySearchContentResonseAsync(tmallHttpClient.Client).Result;

            return respText;
        }


    }
}
