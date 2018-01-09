using System;
using System.Collections.Generic;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.AppStart;
using System.Collections.Specialized;
using ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Common;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShoppingWebCrawler.Host
{
    internal static class Program
    {

        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭  
                    break;
                case 2:
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭  
                    break;
            }
            //清理残留进程
            // Clean up CEF.
            //CefRuntime.Shutdown();
            //MasterRemoteServer.Stop();
            InitApp.TerminalApplicationProcess();


            //Console.ReadLine();
            return false;
        }


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

                // CefRuntime.PostTask(CefThreadId.IO, new MySomeTask());

                // Clean up CEF.
                CefRuntime.Shutdown();
                MasterRemoteServer.Stop();
                Logger.Error(new Exception("未能正确启动CEF爬行蜘蛛！异常信息如下："));
                Logger.Error(ex);
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

            //var headlessForm = new HeadLessMainForm();
            //headlessForm.NavigateToUrl("https://pub.alimama.com/myunion.htm?spm=a219t.7900221/1.1998910419.dbb742793.21214865YeCJuR#!/promo/self/items");



            Console.WriteLine("ShoppingWebCrawler.Host is started.....");

            if (null != args
               && string.Concat(args).Contains(GlobalContext.SlaveModelStartAgrs))
            {
                Console.WriteLine("SlaveRemoteServer is started.....");
            }

            //注册窗口关闭的时候 退出子进程
            // we have to keep the handler routine alive during the execution of the program,
            // because the garbage collector will destroy it after any CTRL event
            GC.KeepAlive(cancelHandler);
            SetConsoleCtrlHandler(cancelHandler, true);

            var locker = RunningLocker.CreateNewLock();
            locker.Pause();

            //6 主进程退出的事件

         



        }

    }
}
