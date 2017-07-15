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
using System.Text;
using System.IO;


/*

 var etaoWeb = new TmallWebPageService();

            string con = etaoWeb.QuerySearchContent("mini裙子") ;

            System.Diagnostics.Debug.WriteLine(con);


*/
namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 天猫搜索页面抓取
    /// </summary>
    public class TmallWebPageService : BaseWebPageService
    {

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
            string respText = TmallMixReuestLoader.Current.LoadUrlGetSearchApiContent(keyWord);
            return respText;
        }






        ///------------内部类-----------------

        /// <summary>
        /// 天猫的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class TmallMixReuestLoader : BaseBrowserRequestLoader<TmallMixReuestLoader>
        {
            private const string tmallSiteUrl = "https://www.tmall.com/";

            /// <summary>
            /// 天猫请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "https://list.tmall.com/search_product.htm?q={0}&type=p&vmarket=&spm=875.7931836%2FB.a2227oh.d100&xl=ip_1&from=mallfp..pc_1_suggest";

            /// <summary>
            /// 请求客户端
            /// </summary>
            private static  CookedHttpClient TmallHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static TmallMixReuestLoader()
            {
                //静态创建请求客户端
                TmallHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "list.tmall.com");
                //requestHeaders.Add("Referer", TmallSiteUrl);
                TmallHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(TmallHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public TmallMixReuestLoader()
            {
                ///天猫刷新搜索页cookie的地址
                this.RefreshCookieUrl = string.Format("https://list.tmall.com/search_product.htm?spm=a220m.1000858.1000724.3.2a70033eTRXtEm&q={0}&sort=new&style=g&from=mallfp..pc_1_searchbutton#J_Filter", "洗面奶男" + DateTime.Now.Ticks.ToString()); ;

                this.IntiCefWebBrowser();
            }

            public string LoadUrlGetSearchApiContent(string keyWord)
            {
           

                //加载Cookie
                var ckVisitor = new LazyCookieVistor();
                var cks = ckVisitor.LoadCookies(tmallSiteUrl);

                


                string searchUrl = string.Format(templateOfSearchUrl, keyWord);

                var client = TmallHttpClient;

                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, tmallSiteUrl);
      
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
