using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService;

namespace ShoppingWebCrawler.Host.AppStart
{
    public class InitApp
    {
        public static int Init(string[] args)
        {
            //1 设定当前程序运行的主上下文
            GlobalContext.SyncContext = SynchronizationContext.Current;

       

            //2 初始化CEF运行时
            #region 初始化CEF运行时

            try
            {
                //加载CEF 运行时  lib cef
                CefRuntime.Load();
           


          

            var mainArgs = new CefMainArgs(args);
            var app = new HeadLessWebBrowerApp();


            //执行CEF启动进程
            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
            if (exitCode != -1)
                return exitCode;

            // CEF的配置参数，有很多参数，我们这里挑几个解释一下： 
            //SingleProcess = false：此处目的是使用多进程。 
            //注意： 强烈不建议使用单进程，单进程不稳定，而且Chromium内核不支持
            //MultiThreadedMessageLoop = true：此处的目的是让浏览器的消息循环在一个单独的线程中执行
            //注意： 强烈建议设置成true,要不然你得在你的程序中自己处理消息循环；自己调用CefDoMessageLoopWork()
            //Locale = "zh-CN"：webkit用到的语言资源，如果不设置，默认将为en - US
            //注意： 可执行文件所在的目录一定要有locals目录，而且这个目录下要有相应的资源文件


            var settings = new CefSettings
            {
                // BrowserSubprocessPath = @"D:\fddima\Projects\Xilium\Xilium.CefGlue\CefGlue.Demo\bin\Release\Xilium.CefGlue.Demo.exe",
                SingleProcess = false,
                MultiThreadedMessageLoop = true,//开启多线程任务
                WindowlessRenderingEnabled = true,//开启无头模式
                //Locale = "en-US",
                LogSeverity = CefLogSeverity.Disable,
                //LogFile = "CefGlue.log",
                NoSandbox = true,
                UserAgent=GlobalContext.ChromeUserAgent,
                UserDataPath =System.IO.Path.Combine(Environment.CurrentDirectory,"UserData"),
                CachePath = System.IO.Path.Combine(Environment.CurrentDirectory, "LocalCache")
            };


            // DevTools doesn't seem to be working when this is enabled
            // http://magpcss.org/ceforum/viewtopic.php?f=6&t=14095
            //settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");

            // Disable GPU in WPF and Offscreen examples until #1634 has been resolved
            //settings.CefCommandLineArgs.Add("disable-gpu", "1");

            //初始化  CEF进程参数设置
            CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return 3;
            }

            #endregion

            //3 开启总控TCP端口，用来接收站点的请求--开启后 会阻塞进程 防止结束
            // 总控端口 负责 1 收集请求 响应请求 2 收集分布的采集客户端 登记注册可用的端，用来做CDN 任务分发，做负载均衡
            RemoteServer.Start();

            //4 定时清理器
            #region 定时清理控制台
            ConsoleClean.Start();
            #endregion

            //5 阿里妈妈等平台登录
            #region  阿里妈妈等平台登录
            //------测试客户端获取 远程Cookie
       
            System.Threading.Tasks.Task.Factory.StartNew(() =>{



                //var ckpender = new CookiePender.AlimamaCookiePenderClient();
                //var cks = ckpender.GetCookiesFromRemoteServer();
                //初始化 轻淘客站的登录模拟
                var qingTaokeService = new QingTaokeWebPageService();
                var loader_qingTaoke = qingTaokeService.RequestLoader;

                //初始化 阿里妈妈站的登录模拟
                var alimamaService = new AlimamaWebPageService();
                var loader_alimama = alimamaService.RequestLoader;


                //模拟跳转到淘宝 进行跨站身份登录
                var taobaoService = new TaobaoWebPageService();
                taobaoService.RequestLoader.NavigateUrlByCefBrowser(TaobaoWebPageService.TaobaoMixReuestLoader.TaobaoSiteUrl);

                //初始化淘宝券站
                var ulandTaoService = new TaoUlandWebPageServic();
                var loader_uland = ulandTaoService.RequestLoader;


            });
         
            #endregion
            return 0;
        }

    }
}
