using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net;
using System.Web;
using ShoppingWebCrawler.Host.Http;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Host.Handlers;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{

    /*
    
测试代码：
            var etaoWeb = new ETaoWebPageService();
            for (int i = 0; i < 10; i++)
            {
                string con = etaoWeb.QuerySearchContent("大米-" + i.ToString() + DateTime.Now.Ticks.ToString());

                System.Diagnostics.Debug.WriteLine(con);

                //var locker1 = RunningLocker.CreateNewLock();
                //locker1.CancelAfter(2000);
            }

        
        */
    /// <summary>
    /// 一淘搜索页面抓取
    /// </summary>
    public class ETaoWebPageService : BaseWebPageService
    {



        /// <summary>
        /// 请求地址-根据方法传递的参数 动态格式化
        /// </summary>
        protected override string TargetUrl
        {
            get; set;
        }





        public ETaoWebPageService()
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
            //格式化一个查询地址

            //this.TargetUrl = string.Format(templateOfSearchUrl, keyWord);



            //string respText = ETaoMixReuestLoader.Current.LoadUrlGetSearchApiContent(this.TargetUrl)
            //    .ConfigureAwait(false)
            //    .GetAwaiter()
            //    .GetResult();

            string respText = ETaoMixReuestLoader.Current.LoadUrlGetSearchApiContent(keyWord);
            return respText;
        }


    }

    /// <summary>
    /// 一淘的混合请求类
    /// 1 根据传入的搜索url  使用 CEF打开 指定地址
    /// 2 拦截出来请求数据的地址
    /// 3 拦截后 把对应的Cookie拿出来
    /// 4  使用.net httpclient 将请求发送出去 得到相应返回
    /// 
    /// 为了保证性能  保持此类单个实例 ，内部维持N>10的webbrowser实例 进行解析
    /// </summary>
    public class ETaoMixReuestLoader
    {
        private const string eTaoSiteUrl = "https://www.etao.com/";

        /// <summary>
        /// 一淘请求 搜索地址页面
        /// </summary>
        private const string templateOfSearchUrl = "https://www.etao.com/search.htm?nq={0}&spm=1002.8113010.2698880.6862";


        private static ReaderWriterLockSlim _readLock = new ReaderWriterLockSlim();
        //线程队列锁
        private static AutoResetEvent waitHandler = new AutoResetEvent(false);

        /// <summary>
        /// 下次自动更新Cookie的时间
        /// </summary>
        private DateTime NextUpdateCookieTime = DateTime.Now.AddMinutes(5);
        /// <summary>
        /// 授权Cookie 默认是7天过期 ，我们定时每间隔一定分钟 30-60min 自动刷新下
        /// </summary>
        private System.Timers.Timer _minitor_auto_refesh_cookies;


        /// <summary>
        /// 初始化浏览器的时候锁
        /// </summary>
        private static ReaderWriterLockSlim _readLock_etaoBrowser = new ReaderWriterLockSlim();
        /// <summary>
        /// CEF组合浏览器
        /// </summary>
        private static ETaoCefBrowser etaoBrowser;

        /// <summary>
        /// 请求客户端
        /// </summary>
        private static CookedHttpClient etaoHttpClient;

        private static ETaoMixReuestLoader _current;

        /// <summary>
        /// 单例模式
        /// 使用了读锁 防止多线程混淆
        /// </summary>
        public static ETaoMixReuestLoader Current
        {
            get
            {
                if (null == _current)
                {

                    try
                    {
                        _readLock.EnterReadLock();
                        _current = new ETaoMixReuestLoader();

                    }
                    catch { }
                    finally
                    {
                        _readLock.ExitReadLock();
                    }
                }
                return _current;
            }
        }


        static ETaoMixReuestLoader()
        {
            //静态创建请求客户端
            etaoHttpClient = new ETaoCefBrowser().ETaoHttpClient;
        }

        public ETaoMixReuestLoader()
        {
            // 初始化 浏览器窗口实例
            IntiCefWebBrowser();
        }

        public string LoadUrlGetSearchApiContent(string keyWord)
        {
            //生成时间戳
            string timestamp = JavascriptContext.getUnixTimestamp();

            //加载Cookie
            var ckVisitor = new LazyCookieVistor();
            var cks = ckVisitor.LoadCookies(eTaoSiteUrl);

            var _m_h5_tk_cookie = cks.FirstOrDefault(x => x.Name == "_m_h5_tk");
            if (null == _m_h5_tk_cookie)
            {
                this.AutoRefeshCookie();//从新刷新页面 获取 服务器颁发的私钥
                cks = ckVisitor.LoadCookies(eTaoSiteUrl);
                _m_h5_tk_cookie = cks.FirstOrDefault(x => x.Name == "_m_h5_tk");
            }
            if (null == _m_h5_tk_cookie || string.IsNullOrEmpty(_m_h5_tk_cookie.Value))
            {
                throw new Exception("加载授权私钥失败！无法获取对应的cookie:_m_h5_tk ");
            }
            string _m_h5_tk_valueString = _m_h5_tk_cookie.Value.Split('_')[0];

            string etao_appkey = "12574478";

            string paras = string.Concat("{\"s\":0,\"n\":40,\"q\":\"", keyWord, "\",\"needEncode\":false,\"sort\":\"sales_desc\",\"maxPrice\":10000000,\"minPrice\":0,\"serviceList\":\"\",\"navigator\":\"all\",\"urlType\":2}");

            string sign = JavascriptContext.getEtaoJSSDKSign(_m_h5_tk_valueString, timestamp, etao_appkey, paras);

            string url = string.Format("https://apie.m.etao.com/h5/mtop.etao.fe.search/1.0/?type=jsonp&api=mtop.etao.fe.search&v=1.0&appKey=12574478&data={0}&t={1}&sign={2}&callback=jsonp28861232595120323", paras, timestamp, sign);


            string searchUrl = string.Format(templateOfSearchUrl, keyWord);
            var client = etaoHttpClient;// etaoBrowser.ETaoHttpClient;

            ////加载cookies
            ////获取当前站点的Cookie
            client.ChangeGlobleCookies(cks, eTaoSiteUrl);
            //修改client 的refer 头
            client.Client.DefaultRequestHeaders.Referrer = new Uri(searchUrl);
            // 4 发送请求
            var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };
            string content = clientProxy.GetRequestTransfer(url, null);

            return content;

        }

        /// <summary>
        /// 内部类方法加载指定的搜索url 并拦截调用api的内容
        /// 在初始化的时候 刷新Cookie用
        /// </summary>
        /// <param name="searchUrl"></param>
        /// <returns></returns>
        private Task<string> InnerLoadUrlGetSearchApiContent(string searchUrl)
        {



            var tcs = new TaskCompletionSource<string>();


            //2 开始发送请求LoadString
            // EventHandler<FilterSpecialUrlEventArgs> handlerRequest = null;
            EventHandler<LoadEndEventArgs> handlerRequest = null;
            var ckVisitor = new LazyCookieVistor();
            handlerRequest = (s, e) =>
            {


                try
                {
                    #region 废弃代码

                    //-------------------下面这段代码 先不移除了，这是基于事件监视的回调方式请求，会发送2次请求 接口不推荐----------------
                    //if (e == null || string.IsNullOrEmpty(e.Url))
                    //{
                    //    throw new Exception("处理一淘监视url丢失！");
                    //}
                    //string url = HttpUtility.UrlDecode(e.Url);
                    ////3 使用配对的httpclient  发送请求
                    //var client = etaoBrowser.ETaoHttpClient;
                    ////修改client 的refer 头
                    //client.Client.DefaultRequestHeaders.Referrer = new Uri(searchUrl);


                    ////加载cookies
                    ////获取当前站点的Cookie

                    //var cks = ckVisitor.LoadCookiesCollection(eTaoSiteUrl);
                    //client.ChangeGlobleCookies(cks, eTaoSiteUrl);
                    ////4 发送请求
                    //var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };
                    //string content = clientProxy.GetRequestTransfer(url, null);

                    #endregion


                    //设置返回结果为固定的内容
                    tcs.SetResult("loaded");

                    //处理完毕后 一定要记得将处理程序移除掉 防止多播
                    //etaoBrowser.ERequestHandler.OnRequestTheMoniterdUrl -= handlerRequest;
                    etaoBrowser.ELoader.LoadEnd -= handlerRequest;

                }
                catch (Exception ex)
                {

                    Logging.Logger.WriteException(ex);
                }
                finally
                {
                    //线程锁打开自动进行下一个
                    waitHandler.Set();
                }
            };
            //etaoBrowser.ERequestHandler.OnRequestTheMoniterdUrl += handlerRequest;

            //必须等待页面加载完毕，否则过期的Cookie无法刷新到最新
            etaoBrowser.ELoader.LoadEnd += handlerRequest;
            etaoBrowser.EBrowser.GetMainFrame().LoadUrl(searchUrl);
            //进入当前线程锁定模式
            waitHandler.WaitOne();

            return tcs.Task;


        }


        /// <summary>
        /// 初始化 cef 浏览器窗口
        /// </summary>
        private void IntiCefWebBrowser()
        {


            try
            {
                if (null == ETaoMixReuestLoader.etaoBrowser)
                {
                    _readLock_etaoBrowser.EnterReadLock();
                    ETaoMixReuestLoader.etaoBrowser = CreateNewWebBrowser().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch { }
            finally
            {
                _readLock_etaoBrowser.ExitReadLock();
            }



            //首先自动刷新下查询页面 会刷新Cookie
            AutoRefeshCookie();

            //每间隔10s检查一次
            this._minitor_auto_refesh_cookies = new System.Timers.Timer(10000);
            this._minitor_auto_refesh_cookies.Elapsed += (s, e) =>
            {

                if (DateTime.Now > this.NextUpdateCookieTime)
                {
                    AutoRefeshCookie();
                }

            };
            this._minitor_auto_refesh_cookies.Start();
        }

        private void AutoRefeshCookie()
        {
            //先尝试清理站点的cookie 注意：这里清理的是CookieManager的Cookie ，静态类中 globalContex中的Cookie仍在
            var ckVisitor = new LazyCookieVistor();
            ckVisitor.DeleteCookies(eTaoSiteUrl, null);

            //然后从新加载下链接 即可刷新Cookie
            string refreshCookieUrl = string.Format("https://www.etao.com/search.htm?nq={0}&spm=1002.8113010.2698880.6862", "洗面奶男" + DateTime.Now.Ticks.ToString());
            this.InnerLoadUrlGetSearchApiContent(refreshCookieUrl);
            //不定时刷新
            this.NextUpdateCookieTime = DateTime.Now.AddMinutes(new Random().Next(5, 10));
        }

        /// <summary>
        /// 创建cef实例
        /// </summary>
        private Task<ETaoCefBrowser> CreateNewWebBrowser()
        {
            //使用任务 锁保证事件变为同步
            var tcs = new TaskCompletionSource<ETaoCefBrowser>();

            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

            // Settings for the browser window itself (e.g. should JavaScript be enabled?).
            var cefBrowserSettings = new CefBrowserSettings();

            // Initialize some the cust interactions with the browser process.
            // The browser window will be 1280 x 720 (pixels).
            var cefClient = new HeadLessCefClient(1, 1);
            var loader = cefClient.GetCurrentLoadHandler();
            loader.BrowserCreated += (s, e) =>
            {

                //事件通知 当cef  browser 创建完毕
                //创建完毕后 保存 browser 对象的实例
                var brw = e.Browser;
                var etaoBrowser = new ETaoCefBrowser { EBrowser = brw, ELoader = loader, ECefClient = cefClient };

                tcs.TrySetResult(etaoBrowser);
            };
            ////注册  加载完毕事件handler
            //loader.LoadEnd += this.OnWebBrowserLoadEnd;
            // Start up the browser instance.
            string url = "about:blank";
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, url);

            return tcs.Task;
        }


    }

    /// <summary>
    /// 一淘对浏览器实例组件的包装
    /// </summary>
    internal class ETaoCefBrowser
    {
        internal HeadLessCefClient ECefClient { get; set; }

        public HeadLessCefLoadHandler ELoader { get; set; }

        /// <summary>
        /// cwf browser 对象实例
        /// </summary>
        public CefBrowser EBrowser { get; set; }

        /// <summary>
        /// 当前浏览器的请求处理实例
        /// </summary>

        public HeadLessWebRequestHandler ERequestHandler
        {
            get
            {

                if (null == this.ECefClient)
                {
                    return null;

                }

                return this.ECefClient.GetRequestHandlerInstance();
            }
        }



        /// <summary>
        /// 一淘请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        public CookedHttpClient ETaoHttpClient { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public ETaoCefBrowser()
        {
            //初始化头信息
            var requestHeaders = BaseRequest.GetCommonRequestHeaders();
            requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
            requestHeaders.Add("Host", "apie.m.etao.com");
            requestHeaders.Add("Upgrade-Insecure-Requests", "1");
            //requestHeaders.Add("Referer", eTaoSiteUrl);
            ETaoHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(ETaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);

        }



    }

}
