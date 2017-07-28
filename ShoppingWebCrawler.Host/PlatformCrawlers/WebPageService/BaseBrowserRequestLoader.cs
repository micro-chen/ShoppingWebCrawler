using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Headless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 基于绑定浏览器的请求基类
    /// 需要浏览器内核支持刷新页面Cookie
    /// </summary>
    public interface IBrowserRequestLoader
    {

        /// <summary>
        /// 使用指定的参数产生请求，返回请求的http响应内容
        /// </summary>
        /// <param name="queryParas"></param>
        /// <returns></returns>
        string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas);
    }
    /// <summary>
    /// 请求加载基类
    /// </summary>
    public abstract class BaseBrowserRequestLoader<T> : IBrowserRequestLoader where T : IBrowserRequestLoader, new()
    {

        /// <summary>
        /// 初始化浏览器的时候锁
        /// </summary>
        private static ReaderWriterLockSlim _readLock_mixdBrowser = new ReaderWriterLockSlim();
        /// <summary>
        /// CEF组合浏览器
        /// </summary>
        internal static CookiedCefBrowser mixdBrowser;

        //线程队列锁
        private static AutoResetEvent waitHandler = new AutoResetEvent(false);


        /// <summary>
        /// 授权Cookie  自动刷新下 定时器
        /// </summary>
        private System.Timers.Timer _minitor_auto_refesh_cookies;

        /// <summary>
        /// 下次自动更新Cookie的时间
        /// </summary>
        private DateTime NextUpdateCookieTime = DateTime.Now.AddMinutes(5);



        private static ReaderWriterLockSlim _readLock = new ReaderWriterLockSlim();

        private static T _current;

        /// <summary>
        /// 单例模式
        /// 使用了读锁 防止多线程混淆
        /// </summary>
        public static T Current
        {
            get
            {
                if (null == _current)
                {
                    _readLock.EnterReadLock();

                    try
                    {
                        _current = new T();
                    }
                    catch { }
                    finally
                    {
                        if (_readLock.IsReadLockHeld)
                        {
                            _readLock.ExitReadLock();
                        }

                    }
                }
                return _current;
            }
        }

        /// <summary>
        ///  刷新Cookie的页面地址，搜索关键词为随机分配
        /// </summary>
        protected string RefreshCookieUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_RefreshCookieUrlTemplate))
                {
                    throw new Exception("未设置刷新url模板地址！");
                }
                //包含占位符的时候  进行格式化字符串
                if (_RefreshCookieUrlTemplate.IndexOf('{')>-1)
                {
                    string mixWord = HotWordsLoader.GetRandHotWord();
                    return string.Format(_RefreshCookieUrlTemplate, mixWord);
                }

                return _RefreshCookieUrlTemplate;
               
            }
        }

        private string _RefreshCookieUrlTemplate;
        /// <summary>
        /// 刷新Cookie的页面地址模板
        /// </summary>
        protected string RefreshCookieUrlTemplate
        {
            get
            {
                return _RefreshCookieUrlTemplate;
            }
            set
            {
                _RefreshCookieUrlTemplate = value;
            }
        }


        public BaseBrowserRequestLoader()
        {

        }


        /// <summary>
        /// 初始化 cef 浏览器窗口
        /// </summary>
        protected void IntiCefWebBrowser()
        {


            try
            {
                if (null == mixdBrowser)
                {
                    _readLock_mixdBrowser.EnterReadLock();
                    mixdBrowser = CookiedCefBrowser.CreateNewWebBrowser()
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
            catch { }
            finally
            {
                if (_readLock_mixdBrowser.IsReadLockHeld)
                {
                    _readLock_mixdBrowser.ExitReadLock();
                }

            }



            //首先自动刷新下查询页面 会刷新Cookie
            AutoRefeshCookie(this.RefreshCookieUrl);

            //每间隔10s检查一次
            this._minitor_auto_refesh_cookies = new System.Timers.Timer(10000);
            this._minitor_auto_refesh_cookies.Elapsed += (s, e) =>
            {

                if (DateTime.Now > this.NextUpdateCookieTime)
                {
                    AutoRefeshCookie(this.RefreshCookieUrl);
                }

            };
            this._minitor_auto_refesh_cookies.Start();
        }


        /// <summary>
        /// 自动刷新Cookie 
        /// 由于是刷新tab页面 所以 不用删除Cookie
        /// </summary>
        protected void AutoRefeshCookie(string refreshCookieUrl)
        {


            if (string.IsNullOrEmpty(refreshCookieUrl))
            {
                throw new Exception("自动刷新Cookie的刷新地址不能为空！");
            }
            //然后从新加载下链接 即可刷新Cookie

            this.InnerLoadUrlGetSearchApiContent(refreshCookieUrl);
            //不定时刷新
            this.NextUpdateCookieTime = DateTime.Now.AddMinutes(new Random().Next(1, 5));
        }

        /// <summary>
        /// 虚方法  要使用的话 子类必须实重写此方法
        /// </summary>
        /// <param name="loginCookieCollection"></param>
        public virtual void SetLogin(List<CefCookie> loginCookieCollection)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 内部类方法加载指定的搜索url 并拦截调用api的内容
        /// 在初始化的时候 刷新Cookie用
        /// </summary>
        /// <param name="searchUrl">请求指定的地址</param>
        /// <param name="timeOut">超时时间，不小于3000毫秒，超时将返回加载超时</param>
        /// <returns></returns>
        private Task<string> InnerLoadUrlGetSearchApiContent(string searchUrl, int timeOut = 6000)
        {

            if (timeOut <= 1000)
            {
                timeOut = 3000;
            }


            //将事件消息模式转换为 task同步消息
            var tcs = new TaskCompletionSource<string>();

            //注册请求处理委托
            EventHandler<LoadEndEventArgs> handlerRequest = null;
            Action<string> disposeHandler = null;
            //资源释放委托
            disposeHandler = (state) =>
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
                    tcs.SetResult(state);

                    //处理完毕后 一定要记得将处理程序移除掉 防止多播
                    //etaoBrowser.ERequestHandler.OnRequestTheMoniterdUrl -= handlerRequest;
                    if (null != handlerRequest)
                    {
                        mixdBrowser.CefLoader.LoadEnd -= handlerRequest;

                    }

                }
                catch (Exception ex)
                {

                   Logger.WriteException(ex);
                }
                finally
                {
                    //线程锁打开自动进行下一个
                    waitHandler.Set();
                }
            };

            //2 开始发送请求LoadString
            // EventHandler<FilterSpecialUrlEventArgs> handlerRequest = null;

            //var ckVisitor = new LazyCookieVistor();
            handlerRequest = (s, e) =>
            {

                string url = HttpUtility.UrlDecode(e.Frame.Url);
                System.Diagnostics.Debug.WriteLine(string.Format("cef core loaded by :{0} ", url));


                disposeHandler("loaded");

            };
            //etaoBrowser.ERequestHandler.OnRequestTheMoniterdUrl += handlerRequest;

            //必须等待页面加载完毕，否则过期的Cookie无法刷新到最新
            mixdBrowser.CefLoader.LoadEnd += handlerRequest;
            mixdBrowser.CefBrowser.GetMainFrame().LoadUrl(searchUrl);

            //回调终结请求阻塞
            TimerCallback resetHandler = (state) =>
            {
                disposeHandler("timeout");
            };
            //超时监听
            var timeBong = new System.Threading.Timer(resetHandler, null, timeOut, Timeout.Infinite);
            //进入当前线程锁定模式
            waitHandler.WaitOne();

            //线程后续执行后，表示任务完毕或者超时，释放定时器资源
            timeBong.Dispose();
            return tcs.Task;


        }


        /// <summary>
        /// 使用指定的参数产生请求，返回请求的http响应内容
        /// 抽象方法
        /// </summary>
        /// <param name="queryParas"></param>
        /// <returns></returns>
        public abstract string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas);

    }
}
