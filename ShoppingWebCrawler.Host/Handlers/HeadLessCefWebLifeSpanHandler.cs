using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;

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

        /// <summary>
        /// 弹出窗口事件
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="targetUrl"></param>
        /// <param name="targetFrameName"></param>
        /// <param name="targetDisposition"></param>
        /// <param name="userGesture"></param>
        /// <param name="popupFeatures"></param>
        /// <param name="windowInfo"></param>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="noJavascriptAccess"></param>
        /// <returns></returns>
        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref bool noJavascriptAccess)
        {

            //无窗口模式 禁止打开弹窗
            return true;
            //var e = new BeforePopupEventArgs(frame, targetUrl, targetFrameName, popupFeatures, windowInfo, client, settings,
            //                     noJavascriptAccess);

            /////*_core.InvokeIfRequired(() => _core.OnBeforePopup(e));*/

            ////client = e.Client;
            ////noJavascriptAccess = e.NoJavascriptAccess;
            ////无窗口模式 禁止打开弹窗
            ////直接在当前页面进行跳转 
            //e.Frame.Browser.GetMainFrame().LoadUrl(e.TargetUrl);

            //e.Handled = true;
            //return e.Handled;
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
