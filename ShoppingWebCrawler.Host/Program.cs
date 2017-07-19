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
                //1 初始化CEF运行时
                InitApp.Init(args);
            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);
            }


            //var locker1 = RunningLocker.CreateNewLock();
            //locker1.CancelAfter(20000);


            var etaoWeb = new TaobaoWebPageService();

            var paras = new NTCPMessage.EntityPackage.Arguments.TaobaoFetchWebPageArgument { KeyWord = "洗面奶男" };

            var con = etaoWeb.QuerySearchContent(paras);

            System.Diagnostics.Debug.WriteLine(con.Result);





            //3 开启总控TCP端口，用来接收站点的请求--开启后 会阻塞进程 防止结束
            // 总控端口 负责 1 收集请求 响应请求 2 收集分布的采集客户端 登记注册可用的端，用来做CDN 任务分发，做负载均衡
            //RemoteServer.Start();

            var locker = RunningLocker.CreateNewLock();
            locker.Pause();


            // Clean up CEF.
            CefRuntime.Shutdown();
        }

    }
}
