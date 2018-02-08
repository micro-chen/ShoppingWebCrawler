using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingWebCrawler.Host.Handlers
{

    public class BrowserCreatedEventArgs : EventArgs
    {
        public CefBrowser Browser { get; set; }
    }

    internal class HeadLessCefLoadHandler : CefLoadHandler
    {

        /// <summary>
        /// cef browser 对象
        /// </summary>
        public CefBrowser Browser { get; private set; }
        /// <summary>
        /// 浏览器 cefbrowser 对象创建完毕事件通知
        /// </summary>
        public event EventHandler<BrowserCreatedEventArgs> BrowserCreated;

        /// <summary>
        /// 加载完毕事件
        /// </summary>
        public event EventHandler<LoadEndEventArgs> LoadEnd;





        protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
            this.Browser = browser;
            try
            {



                //通知事件创建完毕 cef  browser 对象
                if (null != this.BrowserCreated)
                {
                    this.BrowserCreated.Invoke(this, new BrowserCreatedEventArgs { Browser = browser });
                }

                // A single CefBrowser instance can handle multiple requests
                //   for a single URL if there are frames (i.e. <FRAME>, <IFRAME>).
                if (frame.IsMain)
                {
                    Console.WriteLine("START: {0}", browser.GetMainFrame().Url);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            if (frame.IsMain)
            {
                Console.WriteLine("END: {0}, {1}", browser.GetMainFrame().Url, httpStatusCode);
            }
            if (this.LoadEnd != null)
            {
                this.LoadEnd.Invoke(this, new LoadEndEventArgs(frame, httpStatusCode));

            }

        }
    }
}
