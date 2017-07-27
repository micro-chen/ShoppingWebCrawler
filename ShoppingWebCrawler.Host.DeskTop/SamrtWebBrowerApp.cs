namespace ShoppingWebCrawler.Host.DeskTop
{
    using Handlers;
    using ShoppingWebCrawler.Cef.Core;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 继承CefApp
    /// 实现一个  Chromium的 应用
    /// </summary>
    internal sealed class SamrtWebBrowerApp : CefApp, IDisposable
    {

        private CefBrowserProcessHandler _browserProcessHandler = new SmartBrowserProcessHandler();
        private CefRenderProcessHandler _renderProcessHandler = new SmartRenderProcessHandler();

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
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


        ~SamrtWebBrowerApp()
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
