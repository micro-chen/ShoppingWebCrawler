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
    /// 蘑菇街搜索页面抓取
    /// </summary>
    public class MogujieWebPageService : BaseWebPageService
    {

        private const string mgjSiteUrl = "http://www.mogujie.com/";

        private const string templateOfSearchUrl = "http://list.mogujie.com/s?q={0}&ptp=1._mf1_1239_15261.0.0.YDz7yv";
        /// <summary>
        /// 蘑菇街请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient mgjHttpClient;


        /// <summary>
        /// 请求地址-根据方法传递的参数 动态格式化
        /// </summary>
        protected override string TargetUrl
        {
            get;set;
        }



        

        public MogujieWebPageService()
        {
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static MogujieWebPageService()
        {
            //初始化头信息
            var requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", mgjSiteUrl);
            mgjHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(mgjHttpClient.Client.DefaultRequestHeaders, requestHeaders);

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
            CookieCollection cks = GlobalContext.SupportPlatformsCookiesContainer[mgjSiteUrl];
            mgjHttpClient.ChangeGlobleCookies(cks, mgjSiteUrl);

            string respText = this.QuerySearchContentResonseAsync(mgjHttpClient.Client).Result;

            return respText;
        }


    }
}
