namespace Xilium.CefGlue.Client
{
    using ShoppingWebCrawler.Cef.Core;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class DemoApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
        }
    }
}
