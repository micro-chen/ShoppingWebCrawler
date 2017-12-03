
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


using System.Net;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;
using ShoppingWebCrawler.Host.Headless;
using System.Threading;
using System.Linq;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;

/*

废弃 在 启动的时候注册cef窗口
 //2 开启无窗口的浏览器 请求平台站点
                //var appForm = new HeadLessMainForm();
                //appForm.Start();
*/
namespace ShoppingWebCrawler.Host
{


    /// <summary>
    /// ---------------这种一开始就刷新全部注册的url的方式不再适用，新的方式是为每个页面请求实例，单例模式
    /// 并且绑定一个cef browser实例 定时刷新-----------------------------
    /// 无头模式的窗口入口
    /// 1 加载 一个空白的cefbrowser 对象
    /// 2 对配置中的站点列表进行 Next方式打开
    /// 3 Next 链 完毕后，开始随机定时，到触发点后，继续轮询URL
    /// （如果进入新的一轮 那么采用强制刷新的方式 无缓存的方式）
    /// </summary>
    [Obsolete("这种一开始就刷新全部注册的url的方式不再适用，新的方式是为每个页面请求实例，单例模式, 并且绑定一个cef browser实例 定时刷新")]
    public sealed class HeadLessMainForm
    {

        #region 属性
        public CefBrowser WebBrowser { get; private set; }

        /// <summary>
        /// 定时调度器
        /// </summary>
        private System.Timers.Timer _timer;

        ///// <summary>
        ///// 定时器 用来监测请求环链，超过
        ///// </summary>
        //private System.Threading.Timer _timer_webbrowser_loop_monitor;
        /// <summary>
        /// 下一次 触发加载全部电商平台的时间
        /// </summary>
        private DateTime _nextInvokeAllSuuportPlatforms = DateTime.Now;

        /// <summary>
        /// 是否正在轮式加载平台链
        /// </summary>
        private bool _is_running_invoke_linkes = false;

        /// <summary>
        /// 浏览器 cef  browser 对象是否创建完毕
        /// </summary>
        private bool _is_cef_browser_has_created = false;


        /// <summary>
        /// 当前遍历的列表的偏移位置
        /// </summary>
        private int _cursor_Links = 0;

        /// <summary>
        /// 一个简单的随机数生成器
        /// </summary>
        private Random _randor = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// 程序阻塞锁
        /// </summary>
        private RunningLocker locker = new RunningLocker();
        #endregion



        /// <summary>
        /// 构造函数
        /// </summary>
        public HeadLessMainForm()
        {

            this.HeadLessMainForm_Load();
        }



        /// <summary>
        /// 浏览器加载完毕后的一个回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebBrowserLoadEnd(object sender, LoadEndEventArgs e)
        {

            //注意 ：不要在加载完毕事件里阻塞时间过长  当前的窗口是单个的，超时太久 会拒绝加载后续请求，表示请求超时

            //---非电商链接  不做接受处理------------
            //if (!_is_running_invoke_linkes)
            //{
            //    return;
            //}

            var code = e.HttpStatusCode;
       
            if (code == (int)HttpStatusCode.OK)//对状态为 200 成功的 将cookie  注册到全局容器
            {
              


                string url = e.Frame.Url;
                var current_brower_frame = e.Frame;
                var html_vistor = new HtmlSourceVistor();
                var src_code = string.Empty;
                src_code = html_vistor.ReadHtmlSourceSync(current_brower_frame);

                System.Diagnostics.Debug.Write(src_code);

                //if (!string.IsNullOrEmpty(url))
                //{
                //    Task.Factory.StartNew(() =>
                //    {

                //        try
                //        {
                //            //将cookies 放置到指定的网址 key的 CookieContainer中
                //            var ckVisitor = new LazyCookieVistor();
                //            var cks = ckVisitor.LoadCookies(url);
                //            GlobalContext.SupportPlatformsCookiesContainer[url] = cks;
                //        }
                //        catch
                //        {
                //            Logger.Info(new LogEventArgs { LogMessage = string.Format("未能从指定的URL 成功获取Cookies.URL: {0} ", url), LogType = LoggingType.Error });

                //        }
                //    });

                //}
            }

            ////超过阈值 那么表示轮链完毕一次周期
            //if (_cursor_Links >= GlobalContext.SupportPlatforms.Count - 1)
            //{
            //    //清屏
            //    Console.Clear();
            //    //重置状态位
            //    _is_running_invoke_linkes = false;
            //    //随机生成浏览器进行轮链的间隔（分钟）
            //    int browserLoopLinksTimeSpan = _randor.Next(1, 10);
            //    _nextInvokeAllSuuportPlatforms = DateTime.Now.AddMinutes(browserLoopLinksTimeSpan);

            //    return;
            //}



            ////向后偏移
            //_cursor_Links += 1;
            ////如果未到最后一个偏移量  那么继续触发下一个
            //string nextSiteUrl = GlobalContext.SupportPlatforms[_cursor_Links].SiteUrl;
            //this.NavigateToUrl(nextSiteUrl);

        }


        /// <summary>
        /// 窗体加载的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeadLessMainForm_Load()
        {
            if (null == WebBrowser)
            {
                CreateNewWebBrowser();
            }
            //_timer = new System.Timers.Timer();
            //_timer.Interval = 1000;
            //_timer.Elapsed += handler_timer_Tick;
        }

        /// <summary>
        /// 创建一个web browser 对象，并打开一个新的空白页
        /// </summary>
        private void CreateNewWebBrowser()
        {
            var autoSet = new AutoResetEvent(false);
          
            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

            // Settings for the browser window itself (e.g. should JavaScript be enabled?).
            var cefBrowserSettings = new CefBrowserSettings();

            // Initialize some the cust interactions with the browser process.
            // The browser window will be 1280 x 720 (pixels).
            var cefClient = new HeadLessCefClient(1024, 720);
            var loader = cefClient.GetCurrentLoadHandler();
            loader.BrowserCreated += (s, e) =>
            {

                //事件通知 当cef  browser 创建完毕
                //创建完毕后 保存 browser 对象的实例
                this.WebBrowser = e.Browser;
                this._is_cef_browser_has_created = true;

                //发送终止信号
                autoSet.Set();
            };
            //注册  加载完毕事件handler
            loader.LoadEnd += this.OnWebBrowserLoadEnd;
            // Start up the browser instance.
            string url = "about:blank";
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, url);

            //等待结束信号
            autoSet.WaitOne();
        }

        //public void Start()
        //{
        //    if (null == _timer)
        //    {
        //        return;
        //    }

        //    _timer.Start();
        //}

        //public void Stop()
        //{
        //    if (null == _timer)
        //    {
        //        return;
        //    }

        //    _timer.Stop();
        //}

        ///// <summary>
        ///// 计时器触发的事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void handler_timer_Tick(object sender, EventArgs e)
        //{
        //    //cef browser 对象如果未创建完毕 那么不能进行请求
        //    if (this._is_cef_browser_has_created == false || _is_running_invoke_linkes == true)
        //    {
        //        return;
        //    }

        //    //触发加载平台列表
        //    if (DateTime.Now >= _nextInvokeAllSuuportPlatforms)
        //    {
        //        this.BeginLoadAllSupportPlatforms();
        //        this._is_running_invoke_linkes = true;
        //    }
        //}
        ///// <summary>
        ///// 开始轮式 触发链
        ///// </summary>
        //private void BeginLoadAllSupportPlatforms()
        //{
        //    if (null == GlobalContext.SupportPlatforms || GlobalContext.SupportPlatforms.Count <= 0)
        //    {
        //        return;
        //    }
        //    _cursor_Links = 0;
        //    string firstSiteUrl = GlobalContext.SupportPlatforms[_cursor_Links].SiteUrl;
        //    string logMsg = string.Format("CEF开始发送对 {0} 的请求 .", firstSiteUrl);
        //    Logger.Info(logMsg);

        //    this.NavigateToUrl(firstSiteUrl);
        //}
        /// <summary>
        /// 转到指定的网址
        /// </summary>
        /// <param name="url"></param>
        public void NavigateToUrl(string url)
        {
            if (null == this.WebBrowser)
            {
                Logger.Error(string.Format("未能加载指定的地址：{0} ,因为CEF浏览器对象未能创建或已经销毁！", url));
                return;
            }

            try
            {
                //开始触发加载首个网址 每当加载完毕时候 触发的事件中 进行加载下一个
                this.WebBrowser.GetMainFrame().LoadUrl(url);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


        }

    }

}
