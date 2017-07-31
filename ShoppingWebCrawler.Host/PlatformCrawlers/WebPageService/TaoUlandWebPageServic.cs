using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.Net.Http;
using ShoppingWebCrawler.Host.Common.Http;
using System.Net;
using ShoppingWebCrawler.Host.Headless;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.CookiePender;
using ShoppingWebCrawler.Cef.Core;


namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{

    /// <summary>
    /// 淘宝券网页服务
    /// https://uland.taobao.com/coupon/list?pid=mm_31965263_9390671_60792186
    /// </summary>
    public class TaoUlandWebPageServic : BaseWebPageService
    {

        /// <summary>
        /// 淘宝券 网站域名
        /// </summary>
        public const string TaobaoQuanDomain = "https://uland.taobao.com/";
        /// <summary>
        /// 淘宝券列表详细页面
        /// </summary>
        public static readonly string TaobaoQuanListPageUrl = string.Format("https://uland.taobao.com/coupon/list?pid={0}", GlobalContext.Pid);


        static TaoUlandWebPageServic()
        {
        }

        public TaoUlandWebPageServic()
        {
        }


        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return TaoUlandMixReuestLoader.Current;
            }
        }




        ///------------内部类-----------------

        /// <summary>
        /// 淘宝券的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class TaoUlandMixReuestLoader : BaseBrowserRequestLoader<TaoUlandMixReuestLoader>
        {


            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient TaoUlandHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static TaoUlandMixReuestLoader()
            {
                //静态创建请求客户端
                //TaoUlandHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("upgrade-insecure-requests", "1");
                requestHeaders.Add("Host", "uland.taobao.com");
                TaoUlandHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(TaoUlandHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public TaoUlandMixReuestLoader()
            {
                ///淘宝券刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate = TaobaoQuanListPageUrl;

                this.IntiCefWebBrowser();
            }

            /// <summary>
            /// not use
            /// </summary>
            /// <param name="queryParas"></param>
            /// <returns></returns>
            public override string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas)
            {

                return string.Empty;

            }





        }

    }
}
