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
 
namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 唯品会搜索页面抓取
    /// 未完成--再具体做这家的时候 再做详细的地址解析参数
    /// </summary>
    public class VipWebPageService : BaseWebPageService
    {


        

        public VipWebPageService()
        {
        }


        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return VipMixReuestLoader.Current;
            }
        }







        ///------------内部类-----------------

        /// <summary>
        /// 唯品会的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class VipMixReuestLoader : BaseBrowserRequestLoader<VipMixReuestLoader>
        {
            public static readonly string VipSiteUrl = GlobalContext.SupportPlatforms.Find(x => x.Platform ==  SupportPlatformEnum.Vip).SiteUrl;

            /// <summary>
            /// 唯品会请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getSearchList&f=&_=1513070519238";

            /// <summary>
            /// 检索当前关键词下的-【品类】
            /// </summary>
            private const string queryCategoryUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getCategoryTree&f=&_=1513070166483";
            /// <summary>
            /// 检索当前关键词下的-【查询品牌】
            /// </summary>
            private const string queryBrandUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getBrandStoreList&f=&_=1513070611746";

            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient YihaodianHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static VipMixReuestLoader()
            {
                //静态创建请求客户端
               // YihaodianHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders(true);
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "m.vip.com");
                requestHeaders.Add("Referer", VipSiteUrl);
             
                
                YihaodianHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(YihaodianHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public VipMixReuestLoader()
            {
                ///唯品会刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate = VipSiteUrl;

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
                var cks = ckVisitor.LoadCookies(VipSiteUrl);




                string searchUrl = string.Format(templateOfSearchUrl, keyWord);

                var client = YihaodianHttpClient;
                client.Client.DefaultRequestHeaders.Referrer = new Uri(string.Format("http://search.Vip.com/c0-0/k{0}/?tp=1.1.12.0.3.LpPV5SK-10-93L!6", keyWord));
                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, VipSiteUrl);

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
