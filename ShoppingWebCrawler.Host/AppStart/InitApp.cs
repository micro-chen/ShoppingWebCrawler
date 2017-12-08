using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Reflection;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService;
using ShoppingWebCrawler.Host.Common.TypeFinder;

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
                SingleProcess = false,//开启多进程模型，tab  进程独立模式
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
            #region  阿里妈妈等平台网页初始化操作
       
            System.Threading.Tasks.Task.Factory.StartNew(() =>{


                //----------注意：由于开启了多进程模型，不同的网址在不同的tab,每个tab 在其独立的进程中-------
                //每次打开一个程序进程，让进程开启一个端口server,向总控端口，发送注册登记，用来接受请求转发，做负载均衡---

                //初始化平台网页进程
                //1 通过反射 获取所有的webpage service 

                var ass = Assembly.GetExecutingAssembly();
                var typeFinder = new AppDomainTypeFinder();
                var targetType = typeof(BaseWebPageService);
                var webPageServiceTypes = typeFinder.FindClassesOfType(targetType, new Assembly[] { ass }, true);
                if (webPageServiceTypes.IsNotEmpty())
                {
                    foreach (Type itemPageService in webPageServiceTypes)
                    {
                        BaseWebPageService servieInstance = Activator.CreateInstance(itemPageService) as BaseWebPageService;
                        //静态属性访问一次 即可触发打开页面
                        var loader=servieInstance.RequestLoader;
                    }
                }

            });

            #endregion
            return 0;
        }

    }
}
