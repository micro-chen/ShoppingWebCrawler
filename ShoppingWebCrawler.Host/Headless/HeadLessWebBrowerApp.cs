using System;
using System.Collections.Generic;
using System.Text;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Handlers;

namespace ShoppingWebCrawler.Host.Headless
{


    /// <summary>
    /// 继承CefApp
    /// 实现一个  Chromium的 应用
    /// </summary>
    internal sealed class HeadLessWebBrowerApp : CefApp, IDisposable
    {

        private WebCrawlerBrowserProcessHandler _browserProcessHandler = new WebCrawlerBrowserProcessHandler();
        private WebCrawlerRenderProcessHandler _renderProcessHandler = new WebCrawlerRenderProcessHandler();

        //public event EventHandler<CefBrowser> HandlerOfOnBrowserCreated;

        public HeadLessWebBrowerApp()
        {
            //_renderProcessHandler.HandlerOfOnBrowserCreated += (s, e) =>
            //{
            //    if (null != this.HandlerOfOnBrowserCreated)
            //    {
            //        this.HandlerOfOnBrowserCreated(s, e);
            //    }
            //};
        }


          
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            //禁止使用gpu 加速 在headless 模式下  gpu 有问题
            //参考 http://www.cnblogs.com/koangel/p/5396975.html 
            //https://bitbucket.org/xilium/xilium.cefglue/commits/4146c2b46923593f55d28c7435f017631f86dca0
            //commandLine.AppendSwitch("renderer-process-limit", "1");//限制render process 的数目
            //commandLine.AppendSwitch("process-per-tab");
            commandLine.AppendSwitch("disable-gpu");
            commandLine.AppendSwitch("disable-gpu-compositing");
            commandLine.AppendSwitch("enable-begin-frame-scheduling");
            commandLine.AppendSwitch("disable-smooth-scrolling");
             

        }

        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            return _browserProcessHandler;
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        

        #region IDisposable


        ~HeadLessWebBrowerApp()
        {
            Dispose(false);
        }



        // Flag: Has Dispose already been called?
        bool disposed = false;


        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                this._browserProcessHandler = null;
                this._renderProcessHandler = null;

             
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        #endregion
    }
}
