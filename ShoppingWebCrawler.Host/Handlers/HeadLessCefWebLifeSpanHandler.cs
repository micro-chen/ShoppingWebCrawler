using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host.Handlers
{


 
    public sealed class HeadLessCefWebLifeSpanHandler : CefLifeSpanHandler
    {
        /// <summary>
        /// cef browser 对象
        /// </summary>
        public CefBrowser Browser { get; private set; }

        //public event EventHandler<CefBrowser> HandlerOfOnBrowserCreated;

        public HeadLessCefWebLifeSpanHandler()
        {
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            this.Browser = browser;

            base.OnAfterCreated(browser);

            Console.WriteLine("CefBrowser has been created .and in lifespan.");
            //if (null != HandlerOfOnBrowserCreated)
            //{
            //    HandlerOfOnBrowserCreated.Invoke(this, browser);
            //}
            //_core.InvokeIfRequired(() => _core.OnBrowserAfterCreated(browser));
        }

        //protected override bool DoClose(CefBrowser browser)
        //{
        //    // TODO: ... dispose core
        //    return false;
        //}

        //protected override void OnBeforeClose(CefBrowser browser)
        //{

        //}

    }

}
