using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Handlers;
using ShoppingWebCrawler.Host.Common.Http;
using ShoppingWebCrawler.Host.PlatformCrawlers;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Cef.Framework;

namespace ShoppingWebCrawler.Host.Headless
{
    /// <summary>
    /// 带有cef浏览器+HttpClient的组合实例组件的包装
    /// </summary>
    internal class CookiedCefBrowser
    {
        public HeadLessCefClient CefClient { get; set; }

        public HeadLessCefLoadHandler CefLoader { get; set; }

        /// <summary>
        /// cwf browser 对象实例
        /// </summary>
        public CefBrowser CefBrowser { get; set; }

        /// <summary>
        /// 当前浏览器的请求处理实例
        /// </summary>

        public HeadLessWebRequestHandler CefRequestHandler
        {
            get
            {

                if (null == this.CefClient)
                {
                    return null;

                }

                return this.CefClient.GetRequestHandlerInstance();
            }
        }



        /// <summary>
        /// 一淘请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        public CookedHttpClient BindingHttpClient { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public CookiedCefBrowser()
        {
          

        }




        /// <summary>
        /// 创建cef,并 打开制定的网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<CookiedCefBrowser> CreateNewWebBrowser(string url, EventHandler<LoadEndEventArgs> handlerRequest)
        {
            //验证是否是合法的URL
            var isUrl = InPutValidate.IsUrl(url);
            if (!isUrl)
            {
                return Task.FromResult<CookiedCefBrowser>(null);
            }
            //使用任务 锁保证事件变为同步
            var tcs = new TaskCompletionSource<CookiedCefBrowser>();

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
                var etaoBrowser = new CookiedCefBrowser { CefBrowser = brw, CefLoader = loader, CefClient = cefClient };

                tcs.TrySetResult(etaoBrowser);
            };
            if (null!=handlerRequest)
            {
                loader.LoadEnd += handlerRequest;
            }
            ////注册  加载完毕事件handler
            //loader.LoadEnd += this.OnWebBrowserLoadEnd;
            // Start up the browser instance.
           // string url = "about:blank";
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, url);

            return tcs.Task;
        }



        /// <summary>
        /// 创建cef 打开的——blank实例
        /// </summary>
        public static Task<CookiedCefBrowser> CreateNewWebBrowser()
        {
            //使用任务 锁保证事件变为同步
            var tcs = new TaskCompletionSource<CookiedCefBrowser>();

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
                var etaoBrowser = new CookiedCefBrowser { CefBrowser = brw, CefLoader = loader, CefClient = cefClient };

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
}
