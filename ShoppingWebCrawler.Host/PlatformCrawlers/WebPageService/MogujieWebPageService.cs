using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.Net.Http;
using ShoppingWebCrawler.Host.Http;
using System.Net;
using ShoppingWebCrawler.Host.Headless;
using NTCPMessage.EntityPackage;

/*


 var etaoWeb = new MogujieWebPageService();

            string con = etaoWeb.QuerySearchContent("洗面奶男") ;

            System.Diagnostics.Debug.WriteLine(con);
*/
namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 蘑菇街搜索页面抓取
    /// </summary>
    public class MogujieWebPageService : BaseWebPageService
    {

     

        

        public MogujieWebPageService()
        {
        }


        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return MogujieMixReuestLoader.Current;
            }
        }



        ///------------内部类-----------------

        /// <summary>
        /// 蘑菇街的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class MogujieMixReuestLoader : BaseBrowserRequestLoader<MogujieMixReuestLoader>
        {
            private const string MogujieSiteUrl = "http://www.mogujie.com/";

            /// <summary>
            /// 蘑菇街请求 搜索地址页面
            /// 时间戳，关键词，过滤参数
            /// </summary>
            private const string templateOfSearchUrl = "http://list.mogujie.com/search?callback=jQuery211013398370030336082_{0}&_version=1&q={1}&cKey=43&minPrice=&_mgjuuid=66b111f4-e6ce-4b8b-bf0c-311fa8cf0c31&ppath=&page=1&maxPrice=&sort=pop&userId=&cpc_offset=&ratio=2%3A3&_=1500446274789";

            /// <summary>
            /// 搜索列表页面地址模板
            /// </summary>
            private const string templateOfListSearchPageUrl = "http://list.mogujie.com/s?q={0}&ptp=1._mf1_1239_15261.0.0.YDz7yv";
            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient MogujieHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static MogujieMixReuestLoader()
            {
                //静态创建请求客户端
                MogujieHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "list.mogujie.com");
                requestHeaders.Add("Referer", MogujieSiteUrl);
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                MogujieHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(MogujieHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public MogujieMixReuestLoader()
            {
                ///蘑菇街刷新搜索页cookie的地址
                this.RefreshCookieUrl = string.Format(templateOfListSearchPageUrl, "裙子" + DateTime.Now.Ticks.ToString()); ;

                this.IntiCefWebBrowser();
            }

            public override string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas)
            {

                string keyWord = queryParas.KeyWord;
                if (string.IsNullOrEmpty(keyWord))
                {
                    return string.Empty;
                }

                //加载Cookie
                var ckVisitor = new LazyCookieVistor();
                var cks = ckVisitor.LoadCookies(MogujieSiteUrl);



                string timeToken = JavascriptContext.getUnixTimestamp();
                string searchUrl = string.Format(templateOfSearchUrl, timeToken,keyWord);

                var client = MogujieHttpClient;
                client.Client.DefaultRequestHeaders.Referrer = new Uri(string.Format("http://list.mogujie.com/s?q={0}&ptp=1.eW5XD.0.0.qJUTT", keyWord));
                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, MogujieSiteUrl);

                // 4 发送请求
                var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };

                //注意：对于响应的内容 不要使用内置的文本 工具打开，这个工具有bug.看到的文本不全面
                //使用json格式打开即可看到里面所有的字符串

                string content = clientProxy.GetRequestTransfer(searchUrl, null);



                return content;

            }





        }

    }
}
