using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Http;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.CookiePender;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 阿里妈妈 网页授权自动刷新管理类
    /// 远程监听 UI登录程序端口，获取登录的Cookie授权
    /// 刷新到无头浏览器中进行定时刷新保持登录状态
    /// </summary>
    public class AlimamaWebPageService : BaseWebPageService
    {

        /// 阿里妈妈主站地址
        private const string alimamaSiteUrl = GlobalContext.AlimamaSiteURL;

        private static AlimamaCookiePenderClient cookiePender;
        private static System.Timers.Timer _timer_refresh_login_cookie;

        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return AlimamaMixReuestLoader.Current;
            }
        }


        /// <summary>
        /// 静态构造函数
        /// 在构建的时候 进行一次登录初始化
        /// </summary>
        static AlimamaWebPageService()
        {
            BeginTryToLogin();
        }


        /// <summary>
        /// 尝试登录
        /// </summary>
        private static void BeginTryToLogin()
        {
            if (null!=_timer_refresh_login_cookie)
            {
                //有定时任务进行监听的时候 不要重复定时监听
                return;
            }
            cookiePender = new AlimamaCookiePenderClient();

            //-----------首先尝试登录一次，登录不成功，那么进入定时任务中----------
            //表示已经登录 那么刷新登录Cookie
            var cks = cookiePender.GetCookiesFromRemoteServer();
            if (null != cks && cks.FirstOrDefault(x => x.Name == "login") != null)
            {
                //表示已经登录 那么刷新登录Cookie
                SetLogin(cks);
            }
            else
            {

                //开启定时任务刷新登录阿里妈妈Cookie
                _timer_refresh_login_cookie = new System.Timers.Timer(5000);
                _timer_refresh_login_cookie.Elapsed += (s, e) =>
                {
                    cks = cookiePender.GetCookiesFromRemoteServer();
                    if (null != cks && cks.FirstOrDefault(x => x.Name == "login") != null)
                    {
                        //表示已经登录 那么刷新登录Cookie
                        SetLogin(cks);
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
        /// 静态登录Cookie注册
        /// </summary>
        /// <param name="loginCookieCollection">需要提供的已经登录的Cookie集合</param>
        private static void SetLogin(List<CefCookie> loginCookieCollection)
        {
            if (null != loginCookieCollection)
            {
                //注册cookie集合到全局Cookie容器内
                new LazyCookieVistor().RegisterCookieToCookieManager(alimamaSiteUrl, loginCookieCollection);
            }

        }

        /// <summary>
        /// 强制从新登录
        /// </summary>
        public static bool ForceLogin()
        {
            bool success= false;
            var cks = cookiePender.GetCookiesFromRemoteServer();
            if (null != cks && cks.FirstOrDefault(x => x.Name == "login") != null)
            {
                //表示已经登录 那么刷新登录Cookie
                SetLogin(cks);

                success = true;
            }

            return success;
        }




        ///------------内部类-----------------

        /// <summary>
        /// 阿里妈妈的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class AlimamaMixReuestLoader : BaseBrowserRequestLoader<AlimamaMixReuestLoader>
        {



            /// <summary>
            /// 阿里妈妈请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "http://pub.alimama.com/items/search.json?q={0}&_t={1}&toPage=1&perPageSize=40&auctionTag=&shopTag=yxjh&t={1}&_tb_token_={2}&pvid=10_123.127.46.142_367_1501208864509";


            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient alimamaHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static AlimamaMixReuestLoader()
            {
                //静态创建请求客户端
                alimamaHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "pub.alimama.com");
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                //requestHeaders.Add("Referer", alimamaSiteUrl);
                alimamaHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(alimamaHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            /// <summary>
            /// 默认构造无登录Cookie的构造函数
            /// 会跳转到登录页面
            /// </summary>
            public AlimamaMixReuestLoader()
            {

                ///阿里妈妈刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate = "http://pub.alimama.com/myunion.htm?spm=a219t.7900221/10.a214tr8.2.77522c2apY61Kb";

                this.IntiCefWebBrowser();

            }

            /// <summary>
            /// 登录Cookie注册
            /// </summary>
            /// <param name="loginCookieCollection">需要提供的已经登录的Cookie集合</param>
            public override void SetLogin(List<CefCookie> loginCookieCollection)
            {
                if (null != loginCookieCollection)
                {
                    //注册cookie集合到全局Cookie容器内
                    new LazyCookieVistor().RegisterCookieToCookieManager(alimamaSiteUrl, loginCookieCollection);
                }

                this.AutoRefeshCookie(this.RefreshCookieUrlTemplate);
            }


            /// <summary>
            /// 阿里妈妈商品搜索API
            /// </summary>
            /// <param name="queryParas"></param>
            /// <returns></returns>
            public override string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas)
            {

                string keyWord = queryParas.KeyWord;
                if (string.IsNullOrEmpty(keyWord))
                {
                    return string.Empty;
                }
                //生成时间戳
                string timestamp = JavascriptContext.getUnixTimestamp();

                //加载Cookie
                var ckVisitor = new LazyCookieVistor();
                var cks = ckVisitor.LoadCookies(alimamaSiteUrl);

                //在查询字符串中的Cookie
                var queryCookie = cks.FirstOrDefault(x => x.Name == "_tb_token_");
                string queryCookieValue = string.Empty;
                if (null != queryCookie)
                {
                    queryCookieValue = queryCookie.Value;
                }

                string searchUrl = string.Format(templateOfSearchUrl, keyWord, timestamp, queryCookieValue);
                var client = alimamaHttpClient;

                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, alimamaSiteUrl);

                // 4 发送请求
                var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };
                string content = clientProxy.GetRequestTransfer(searchUrl, null);

                return content;

            }



        }

    }


}

