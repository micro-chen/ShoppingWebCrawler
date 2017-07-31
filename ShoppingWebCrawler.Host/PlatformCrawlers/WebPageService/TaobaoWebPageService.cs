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

/*

var etaoWeb = new TaobaoWebPageService();

            var paras = new NTCPMessage.EntityPackage.Arguments.TaobaoFetchWebPageArgument { KeyWord = "洗面奶男" };

            var con = etaoWeb.QuerySearchContent(paras);

            System.Diagnostics.Debug.WriteLine(con.Result);

*/
namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{

    /// <summary>
    /// 淘宝网页搜索结果抓取
    /// </summary>
    public class TaobaoWebPageService : BaseWebPageService
    {

        private static System.Timers.Timer _timer_refresh_login_cookie;

        private static TaobaoCookiePenderClient cookiePender_taobao;

        public static bool IsHasLoginTaobao= false;

        static TaobaoWebPageService()
        {
            BeginTryToLogin();
        }

        public TaobaoWebPageService()
        {
        }


        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return TaobaoMixReuestLoader.Current;
            }
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
            cookiePender_taobao = new TaobaoCookiePenderClient();
            //-----------首先尝试登录一次，登录不成功，那么进入定时任务中----------
            //表示已经登录 那么刷新登录Cookie
            var cks_taobao= cookiePender_taobao.GetCookiesFromRemoteServer();
            if (null != cks_taobao && cks_taobao.FirstOrDefault(x => x.Name == "_nk_") != null)
            {
                //表示已经登录 那么刷新登录Cookie
                SetLogin(cks_taobao);

            }
            else
            {

                //开启定时任务刷新登录阿里妈妈Cookie
                _timer_refresh_login_cookie = new System.Timers.Timer(5000);
                _timer_refresh_login_cookie.Elapsed += (s, e) =>
                {
                    cks_taobao = cookiePender_taobao.GetCookiesFromRemoteServer();
                    if (null != cks_taobao && cks_taobao.FirstOrDefault(x => x.Name == "_nk_") != null)
                    {
                        //表示已经登录 那么刷新登录Cookie
                        SetLogin(cks_taobao);
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
                new LazyCookieVistor().RegisterCookieToCookieManager(GlobalContext.TaobaoSiteURL, loginCookieCollection);


                IsHasLoginTaobao= true;
            }




        }

        /// <summary>
        /// 强制从新登录
        /// </summary>
        public static bool ForceLogin()
        {
            bool success = false;
            var cks = cookiePender_taobao.GetCookiesFromRemoteServer();
            if (null != cks && cks.FirstOrDefault(x => x.Name == "_nk_") != null)
            {
                //表示已经登录 那么刷新登录Cookie
                SetLogin(cks);

                success = true;
            }

            return success;
        }



        /// <summary>
        /// 爆淘宝联盟的链接-根据淘宝官方跳转js解析算法
        /// 使用postman  发送淘宝客链接可以得到这个算法
        /// </summary>
        /// <param name="encryUrl"></param>
        public static string GetTaobaoUnionOfficalUrl(string encryUrl)
        {
            //示范：下面为淘宝客加密的链接--从一淘过来的
            //string url = "http://s.click.taobao.com/t?spm=1002.8113010.1999451596.1.197829d2jvKq9J&e=m%3D2%26s%3Dxx7h3yvW%2FlwcQipKwQzePOeEDrYVVa64szgHCoaJEBXomhrxaV0k4ZAA5CqNKnVlalBUWfSYtdXqadVuhJq1oW37Sy0WpaHc0S8eIUiNHrwNztF5RF%2BnklwTri0BQMnX1tZRX7Kk0roGkzEdSUwZLhvt%2FrpwP7nD09XRW5e8YPIgsgo%2FaWiDiMYl7w3%2FA2kb";

            var httpHelper = new HttpRequestHelper();
            var requestHeaders = new NameValueCollection();
            requestHeaders.Add("Host", "s.click.taobao.com");
            requestHeaders.Add("Upgrade-Insecure-Requests", "1");
            var resp = httpHelper.CreateGetHttpResponse(encryUrl, requestHeaders);//.// new Http.CookedHttpClient().Client.GetStringAsync(url).Result;

            string tuUrl = resp.ResponseUri.AbsoluteUri;

            string realUrl = TaobaoWebPageService.ConvertTaobaoKeUrlToRealUrl(tuUrl);


            requestHeaders.Add("Referer", tuUrl);
            var resp2 = httpHelper.CreateGetHttpResponse(realUrl, requestHeaders, 50000);

            string carshedUrl = resp2.ResponseUri.AbsoluteUri;

            return carshedUrl;
        }


        /// <summary>
        /// 获取二级tu跳转 tuUrl
        /// </summary>
        /// <param name="tuUrl"></param>
        /// <returns></returns>
        private static string ConvertTaobaoKeUrlToRealUrl(string tuUrl)
        {

            var schema = string.Empty;
            if (tuUrl.IndexOf("https://") == 0)
            {
                schema = "https";
            }
            else
            {
                schema = "http";
            }

            var qs = tuUrl.Split('?')[tuUrl.Split('?').Length - 1].Split('&');
            var qso = new Dictionary<string, string>();
            for (var i = 0; i < qs.Length; i++)
            {
                if (qs[i] != "")
                {
                    var tmpa = qs[i].Split('=');
                    qso[tmpa[0]] = !string.IsNullOrEmpty(tmpa[1]) ? tmpa[1] : "";
                }
            }

            if (!qso.ContainsKey("tu"))
            {
                throw new Exception(string.Concat("未能转换此加密淘宝客链接：", tuUrl));

            }

            string jump_url = string.Empty;
            if (qso["tu"].IndexOf("https") == 0)
            {
                jump_url = qso["tu"].Substring(5);
            }
            else if (qso["tu"].IndexOf("http") == 0)
            {
                jump_url = qso["tu"].Substring(4);
            }


            var jump_address = schema + jump_url;

            var real_jump_address = Microsoft.JScript.GlobalObject.unescape(jump_address);

            return real_jump_address;

        }






        ///------------内部类-----------------

        /// <summary>
        /// 淘宝的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class TaobaoMixReuestLoader : BaseBrowserRequestLoader<TaobaoMixReuestLoader>
        {
            public const string TaobaoSiteUrl = "https://www.taobao.com/";

            /// <summary>
            /// 淘宝淘宝网页搜索获取地址
            /// </summary>
            private const string templateOfSearchUrl = "https://s.taobao.com/search?data-key=sort&data-value=renqi-desc&ajax=true&_ksTS=1500517782768_897&callback=jsonp898&q={0}&commend=all&ssid=s5-e&search_type=item&sourceId=tb.index&spm=a21bo.50862.201856-taobao-item.1&ie=utf8&initiative_id=tbindexz_20170720";


            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient TaobaoHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static TaobaoMixReuestLoader()
            {
                //静态创建请求客户端
                //TaobaoHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("upgrade-insecure-requests", "1");
                requestHeaders.Add("Referer", TaobaoSiteUrl);
                requestHeaders.Add("Host", "www.taobao.com");
                TaobaoHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(TaobaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public TaobaoMixReuestLoader()
            {
                ///淘宝刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate =string.Concat("https://s.taobao.com/search?initiative_id=tbindexz_",DateTime.Now.ToString("yyyyMMdd"),"&ie=utf8&spm=a21bo.50862.201856-taobao-item.2&sourceId=tb.index&search_type=item&ssid=s5-e&commend=all&imgfile=&q={0}&suggest=history_1&_input_charset=utf-8&wq=&suggest_query=&source=suggest") ;

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
                var cks = ckVisitor.LoadCookies(TaobaoSiteUrl);




                string searchUrl = string.Format(templateOfSearchUrl, keyWord);

                var client = TaobaoHttpClient;

                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, TaobaoSiteUrl);

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
