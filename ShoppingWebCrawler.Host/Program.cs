using System;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.AppStart;


namespace ShoppingWebCrawler.Host
{ 
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            //return InitApp.Init(args);

            try
            {
                //1 初始化CEF运行时
                InitApp.Init(args);
                var appForm = new HeadLessMainForm();
                appForm.Start();
            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);
            }
            

            Console.ReadKey();

            // Clean up CEF.
            CefRuntime.Shutdown();
        }
    }
}
