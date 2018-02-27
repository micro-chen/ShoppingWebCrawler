
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Headless;
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

        /// <summary>
        /// 导航到指定的地址
        /// </summary>
        /// <param name="searchUrl"></param>
        void NavigateUrlByCefBrowser(string searchUrl);
    }
    /// <summary>
    /// 请求加载基类
    /// </summary>
    public abstract class BaseBrowserRequestLoader<T> : IBrowserRequestLoader where T : IBrowserRequestLoader, new()
    {

        /// <summary>
        /// 初始化浏览器的时候锁
        /// </summary>
        private  object _readLock_mixdBrowser = new object();
        /// <summary>
        /// CEF组合浏览器
        /// </summary>
        internal CookiedCefBrowser mixdBrowser;

        //线程队列锁
        private  AutoResetEvent waitHandler = new AutoResetEvent(false);


        /// <summary>
        /// 授权Cookie  自动刷新下 定时器
        /// </summary>
        private System.Timers.Timer _minitor_auto_refesh_cookies;

        /// <summary>
        /// 下次自动更新Cookie的时间
        /// </summary>
        private DateTime NextUpdateCookieTime = DateTime.Now;



        private static object _readLock = new object();

        private static T _current;

        /// <summary>
        /// 单例模式
        /// 使用了读锁 防止多线程混淆
        /// </summary>
        public static T Current
        {
            get
            {
                lock (_readLock)
                {
                    if (null == _current)
                    {
                        _current = new T();
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
                if (_RefreshCookieUrlTemplate.IndexOf('{') > -1)
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

            if (GlobalContext.IsInSlaveMode)//在集群模式下，从节点不允许创建tab
            {
                return;
            }

            try
            {



                //首先自动刷新下查询页面 会刷新Cookie
                AutoRefeshCookie(this.RefreshCookieUrl);
                //每间隔 检查一次
                if (null==this._minitor_auto_refesh_cookies)
                {
                    this._minitor_auto_refesh_cookies = new System.Timers.Timer(5000);
                    this._minitor_auto_refesh_cookies.Elapsed += (s, e) =>
                    {

                        if (DateTime.Now > this.NextUpdateCookieTime)
                        {
                            //不定时刷新--时间段在redis cookie  过期之间，redis 过期为5 min
                            int randNumber = NumbericExtension.GetRandomNumber(60, 120);//debug 的时候 可以延长
                            this.NextUpdateCookieTime = DateTime.Now.AddSeconds(randNumber);

                            AutoRefeshCookie(this.RefreshCookieUrl);

                           
                        }

                    };
                    this._minitor_auto_refesh_cookies.Start();
                }
                

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
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
            this.SyncCookieFromRedisToLocal(refreshCookieUrl);
            this.LoadUrlGetContentByCefBrowser(refreshCookieUrl);
           
        }

        /// <summary>
        /// 将redis 中的cookie 同步到cef 运行时中
        /// </summary>
        /// <param name=""></param>
        private void SyncCookieFromRedisToLocal(string url)
        {
            try
            {
                var cookiesInRedis = GlobalContext.PullFromRedisCookies(url);
                if (cookiesInRedis.IsNotEmpty())
                {
                    new LazyCookieVistor().SetCookieToCookieManager(url, cookiesInRedis);
                }
            }
            catch (Exception ex)
            {

                Logger.Error(ex);
            }
          

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
        /// 内部类方法加载指定的搜索url 
        /// 比如：在初始化的时候 刷新Cookie用 或者刷新 Cookie 获取其他
        /// </summary>
        /// <param name="searchUrl">请求指定的地址</param>
        /// <param name="timeOut">超时时间，不小于10秒，超时将返回加载超时</param>
        /// <returns></returns>
        protected Task<string> LoadUrlGetContentByCefBrowser(string searchUrl, int timeOut = 10000)
        {

            if (timeOut <= 10000)
            {
                timeOut = 10000;
            }

            try
            {


                //将事件消息模式转换为 task同步消息
                var tcs = new TaskCompletionSource<string>();
                bool isProcessRequestEnd = false;


                //注册请求处理委托
                EventHandler<LoadEndEventArgs> handlerRequest = null;
                Action<string> disposeHandler = null;
                //资源释放委托
                disposeHandler = (state) =>
                {
                    try
                    {
                        if (tcs.Task.IsCompleted != true)
                        {
                            //设置返回结果为固定的内容
                            tcs.SetResult(state);
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                    finally
                    {
                        //线程锁打开自动进行下一个
                        waitHandler.Set();
                    }
                };

                //2 开始发送请求LoadString
                handlerRequest = (s, e) =>
                {

                    isProcessRequestEnd = true;//标识正在接受请求

                    try
                    {
                        string url = HttpUtility.UrlDecode(searchUrl);//e.Frame.Url
                        System.Diagnostics.Debug.WriteLine(string.Format("cef core loaded by :{0} ", url));
                        //刷新 cookie
                        if (!string.IsNullOrEmpty(url) && !url.Equals("about:blank"))
                        {
                            var ckVisitor = new LazyCookieVistor();
                            ckVisitor.LoadCookiesAsyc(url, true);

                        }
                    }
                    catch
                    { }


                    //mixdBrowser.CefLoader.LoadEnd -= handlerRequest;
                    disposeHandler("loaded");

                };
                //etaoBrowser.ERequestHandler.OnRequestTheMoniterdUrl += handlerRequest;
                if (null == mixdBrowser || null == mixdBrowser.CefBrowser)
                {
                    mixdBrowser = CookiedCefBrowser.CreateNewWebBrowser(searchUrl, handlerRequest)
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult();
                }
                else
                {
                    //必须等待页面加载完毕，否则过期的Cookie无法刷新到最新
                    mixdBrowser.CefLoader.LoadEnd += handlerRequest;
                    mixdBrowser.CefBrowser.GetMainFrame().LoadUrl(searchUrl);
                }


                //回调终结请求阻塞
                TimerCallback resetHandler = (state) =>
                {
                    //对于超时的 直接杀死render 进程
                    if (isProcessRequestEnd==true)
                    {
                        return;
                    }
                    disposeHandler("timeout");
                };

                //超时监听
                var timeBong = new System.Threading.Timer(resetHandler, null, timeOut, Timeout.Infinite);
                
                //进入当前线程锁定模式
                waitHandler.WaitOne();

                mixdBrowser.Dispose();
                //线程后续执行后，表示任务完毕或者超时，释放定时器资源
                timeBong.Dispose();
                return tcs.Task;
            }
            catch (Exception ex)
            {

                Logger.Error(ex);
            }
            return Task.FromResult<string>(string.Empty);

        }

        /// <summary>
        /// 导航到指定的地址
        /// </summary>
        /// <param name="searchUrl"></param>
        public void NavigateUrlByCefBrowser(string searchUrl)
        {
            mixdBrowser.CefBrowser.GetMainFrame().LoadUrl(searchUrl);

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
