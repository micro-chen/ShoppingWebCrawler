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
    /// 淘宝网页搜索结果抓取
    /// </summary>
    public class TaobaoWebPageService : BaseWebPageService
    {
        private const string taobaoSiteUrl = "https://www.taobao.com/";
        /// <summary>
        /// 淘宝淘宝网页搜索获取地址
        /// </summary>
        private const string templateOfSearchUrl = "https://s.taobao.com/search?q={0}&imgfile=&commend=all&ssid=s5-e&search_type=item&sourceId=tb.index&spm=a21bo.50862.201856-taobao-item.1&ie=utf8&initiative_id=tbindexz_20170710";

        /// <summary>
        /// 淘宝请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient taobaoHttpClient;

        protected override string TargetUrl
        {
            get; set;
        }


        public TaobaoWebPageService()
        {
        }


        /// <summary>
        /// 静态构造函数
        /// </summary>
        static TaobaoWebPageService()
        {
            //初始化头信息
            var requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", taobaoSiteUrl);
            taobaoHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(taobaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);

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
            CookieContainer ckContainer = GlobalContext.SupportPlatformsCookiesContainer[taobaoSiteUrl];
            taobaoHttpClient.Cookies = ckContainer;

            string respText = this.QuerySearchContentResonseAsync(taobaoHttpClient.Client).Result;

            return respText;
        }

    }
}
