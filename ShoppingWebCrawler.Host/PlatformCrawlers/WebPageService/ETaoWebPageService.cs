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

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{

    /// <summary>
    /// 一淘搜索页面抓取
    /// </summary>
    public class ETaoWebPageService : BaseWebPageService
    {


        /// <summary>
        /// 一淘请求 搜索地址页面
        /// </summary>
        private const string templateOfSearchUrl = "https://www.etao.com/search.htm?nq={0}&spm=1002.8113010.2698880.6862";


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

            this.TargetUrl = string.Format(templateOfSearchUrl, keyWord);



            string respText = ETaoMixReuestLoader.Current.LoadUrlGetSearchApiContent(this.TargetUrl)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

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

        private static ReaderWriterLockSlim _readLock = new ReaderWriterLockSlim();
        //线程队列锁
        private static AutoResetEvent waitHandler = new AutoResetEvent(false);
        /// <summary>
        /// 浏览器实例数
        /// 默认为5 
        /// </summary>
        private const int cefBrowserCount = 1;

        /// <summary>
        /// cef 实例数目上限从配置文件中：ETaoMixReuestBrowserCount 得到配置
        /// </summary>
        private int limitMaxCefBrowserCount = 20;
        /// <summary>
        /// 打的N个浏览器窗口实例
        /// </summary>
        private ConcurrentQueue<ETaoCefBrowser> _queueOfCefBrowser;



        private int _hasCreateBrowserCount = cefBrowserCount;
        public int HasCreateBrowserCount
        {
            get
            {
                return _hasCreateBrowserCount;
            }
            set
            {
                _hasCreateBrowserCount = value;
            }
        }

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




        public ETaoMixReuestLoader()
        {
            //得到cef实例的上限
            var maxCefCount = ConfigHelper.GetConfigInt("ETaoMixReuestBrowserCount");
            if (maxCefCount > 0)
            {
                limitMaxCefBrowserCount = maxCefCount;

            }
            _queueOfCefBrowser = new ConcurrentQueue<ETaoCefBrowser>();


            //初始化
            this.IntiCefWebBrowsers();
        }


        /// <summary>
        /// 加载指定的搜索url 并拦截调用api的内容
        /// </summary>
        /// <param name="searchUrl"></param>
        /// <returns></returns>
        public Task<string> LoadUrlGetSearchApiContent(string searchUrl)
        {

            var tcs = new TaskCompletionSource<string>();

            // 1 开始从队列拿出一个浏览器对象实例
            ETaoCefBrowser etaoBrowser = null;
            if (!this._queueOfCefBrowser.TryDequeue(out etaoBrowser))
            {
                //如果没有 那么新创建一个 --监测阈值
                if (this.HasCreateBrowserCount < limitMaxCefBrowserCount)
                {
                    etaoBrowser = this.CreateNewWebBrowser().ConfigureAwait(false).GetAwaiter().GetResult();
                    this._queueOfCefBrowser.Enqueue(etaoBrowser);
                }
                else
                {
                    throw new Exception("当前请求达到了一淘处理的峰值！请稍等访问！");
                }
            }

            //2 开始发送请求LoadString
            EventHandler<FilterSpecialUrlEventArgs> handlerRequest = null;
            var ckVisitor = new LazyCookieVistor();
            handlerRequest = (s, e) =>
            {


                if (e == null || string.IsNullOrEmpty(e.Url))
                {
                    throw new Exception("处理一淘监视url丢失！");
                }

                string url = HttpUtility.UrlDecode(e.Url);
                //3 使用配对的httpclient  发送请求
                var client = etaoBrowser.ETaoHttpClient;
                //修改client 的refer 头
                client.Client.DefaultRequestHeaders.Referrer = new Uri(searchUrl);


                //加载cookies
                //获取当前站点的Cookie

                var cks = ckVisitor.LoadCookiesCollection(eTaoSiteUrl);
                //client.ChangeGlobleCookies(cks, eTaoSiteUrl);
                //4 发送请求
                var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };
                string content = clientProxy.GetRequestTransfer(url, null);

                

                //处理完毕后 一定要记得将处理程序移除掉 防止多播
                etaoBrowser.ERequestHandler.OnRequestTheMoniterdUrl -= handlerRequest;

               // waitHandler.Set();//线程锁打开自动进行下一个

                tcs.SetResult(content);
                etaoBrowser.EBrowser.StopLoad();
            };
            etaoBrowser.ERequestHandler.OnRequestTheMoniterdUrl += handlerRequest;
            //先尝试清理站点的cookie 注意：这里清理的是CookieManager的Cookie ，静态类中 globalContex中的Cookie仍在
            ckVisitor.DeleteCookies(eTaoSiteUrl, null);
            etaoBrowser.EBrowser.GetMainFrame().LoadUrl(searchUrl);
            //进入当前线程锁定模式
           // waitHandler.WaitOne();


            return tcs.Task;
        }


        /// <summary>
        /// 初始化 cef队列
        /// </summary>
        private void IntiCefWebBrowsers()
        {

            for (int i = 0; i < cefBrowserCount; i++)
            {
                var etaoBrowser = this.CreateNewWebBrowser().ConfigureAwait(false).GetAwaiter().GetResult();

                this._queueOfCefBrowser.Enqueue(etaoBrowser);
            }
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
            requestHeaders.Add("Host", "apie.m.etao.com");
            requestHeaders.Add("Upgrade-Insecure-Requests", "1");
            //requestHeaders.Add("Referer", eTaoSiteUrl);
            ETaoHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(ETaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);

        }



    }

}
