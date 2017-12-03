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

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 轻淘客页面抓取
    /// </summary>
    public class QingTaokeWebPageService : BaseWebPageService
    {





        private static QingTaokeCookiePenderClient cookiePender_qingTaoke;


        private static System.Timers.Timer _timer_refresh_login_cookie;


        public QingTaokeWebPageService()
        {
        }


        static QingTaokeWebPageService()
        {
            BeginTryToLogin();
        }


        /// <summary>
        /// 尝试登录
        /// </summary>
        private static void BeginTryToLogin()
        {
            if (null != _timer_refresh_login_cookie)
            {
                //有定时任务进行监听的时候 不要重复定时监听
                return;
            }
            cookiePender_qingTaoke = new QingTaokeCookiePenderClient();
            //-----------首先尝试登录一次，登录不成功，那么进入定时任务中----------
            //表示已经登录 那么刷新登录Cookie
            var cks_qingTaoke = cookiePender_qingTaoke.GetCookiesFromRemoteServer();
            if (null != cks_qingTaoke )
            {


                //开启定时任务刷新登录阿里妈妈Cookie
                _timer_refresh_login_cookie = new System.Timers.Timer(5000);
                _timer_refresh_login_cookie.Elapsed += (s, e) =>
                {
                    cks_qingTaoke = cookiePender_qingTaoke.GetCookiesFromRemoteServer();
                    if (null != cks_qingTaoke)
                    {
                        //表示已经登录 那么刷新登录Cookie
                        //注册cookie集合到全局Cookie容器内
                        new LazyCookieVistor().SetCookieToCookieManager(GlobalContext.QingTaokeSiteURL, cks_qingTaoke);
                        //一旦登录成功不再定时从远程获取，后续让自身无头浏览器 刷新登录Cookie
                        _timer_refresh_login_cookie.Stop();
                        _timer_refresh_login_cookie.Dispose();
                        _timer_refresh_login_cookie = null;
                    }
                };
                _timer_refresh_login_cookie.Start();
            }
        }



        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return QingTaokeMixReuestLoader.Current;
            }
        }





        ///------------内部类-----------------

        /// <summary>
        /// 轻淘客的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class QingTaokeMixReuestLoader : BaseBrowserRequestLoader<QingTaokeMixReuestLoader>
        {

            private const string QingTaokeSiteUrl = GlobalContext.QingTaokeSiteURL;

            /// <summary>
            /// 轻淘客请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "http://www.qingtaoke.com/qingsou?new=0&istmall=0&gold=0&show_type=0&isTaoQiangGou=0&isJu=0&type=0&isJyj=0&isSeaBuy=0&isPremium=0&brandIds=&cat=0&yongjin=&dsr=&xiaoliang=&min_price=&max_price=&min_coupon=&max_coupon=&sort=1&s_type=1&title={0}&f=1&isAli=1";

            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient QingTaokeHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static QingTaokeMixReuestLoader()
            {
                //静态创建请求客户端
               // QingTaokeHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "www.qingtaoke.com");
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                QingTaokeHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(QingTaokeHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public QingTaokeMixReuestLoader()
            {
                ///轻淘客刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate = GlobalContext.QingTaokeSiteURL;

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
                var cks = ckVisitor.LoadCookies(QingTaokeSiteUrl);




                string searchUrl = string.Format(templateOfSearchUrl, keyWord);

                var client = QingTaokeHttpClient;
                //设置跳转头 Referrer
                //client.Client.DefaultRequestHeaders.Referrer = new Uri(string.Format("http://mobile.yangkeduo.com/search_result.html?search_key={0}&search_src=new&refer_page_name=search&refer_page_id=search_1500439537429_yr7sonlWB0", keyWord));
                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, QingTaokeSiteUrl);

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
