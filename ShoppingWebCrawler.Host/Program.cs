using System;
using System.Collections.Generic;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.AppStart;
using System.Collections.Specialized;
using ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService;

namespace ShoppingWebCrawler.Host
{
    internal static class Program
    {


        [STAThread]
        private static void Main(string[] args)
        {





            //破解淘宝客加密的链接

            //string url = "http://s.click.taobao.com/t?spm=1002.8113010.1999451588.1.197829d2xOjGWY&e=m%3D2%26s%3DmP8QGUZCl18cQipKwQzePOeEDrYVVa64LKpWJ%2Bin0XK3bLqV5UHdqU7FFcTKJEXpBuky%2F0Sep%2BFpvEi8xmC0PQfgGrPFD%2FD7ItsJf7xhZUukOrdzMLy3g0C9MWo3ZAy5ZtvIAOb0yL8buZkKjgqa4LRqys2RxTiLmiP8wiUuCvFDEV8PXh1a5UciGQ2l2vvBJoe7ipwP0MtRLBgaW5udaw%3D%3D";

            //string content = new Http.HttpServerProxy().GetRequestTransfer(url,null);


            //var rl = TaobaoWebPageService.GetTaobaoUnionOfficalUrl(url);
            // return;

            try
            {
                //初始化CEF运行时 等操作
                InitApp.Init(args);
            }
            catch (Exception ex)
            {
                
                // Clean up CEF.
                CefRuntime.Shutdown();
                Logging.Logger.WriteException(new Exception("未能正确启动CEF爬行蜘蛛！异常信息如下："));
                Logging.Logger.WriteException(ex);
                return;
            }


            //var locker1 = RunningLocker.CreateNewLock();
            //locker1.CancelAfter(20000);


            //BaseWebPageService etaoWeb = new TaobaoWebPageService();

            //var paras = new NTCPMessage.EntityPackage.Arguments.TaobaoFetchWebPageArgument { KeyWord = "洗面奶男" };

            //var con = etaoWeb.QuerySearchContent(paras);

            //System.Diagnostics.Debug.WriteLine(con.Result);

            //etaoWeb = new JingdongWebPageService();

            // con = etaoWeb.QuerySearchContent(paras);



            Console.WriteLine("ShoppingWebCrawler.Host is started.....");
            var locker = RunningLocker.CreateNewLock();
            locker.Pause();


            // Clean up CEF.
            CefRuntime.Shutdown();
        }

    }
}
