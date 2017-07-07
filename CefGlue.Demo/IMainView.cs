namespace Xilium.CefGlue.Demo
{
    using ShoppingWebCrawler.Cef.Core;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IMainView : IDisposable
    {
        void NewTab(string url);
        void Close();

        void NavigateTo(string url);

        CefBrowser CurrentBrowser { get; }

        void NewWebView(string url, bool transparent);
    }
}
