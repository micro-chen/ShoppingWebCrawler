using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Headless;

namespace ShoppingWebCrawler.Host.AppStart
{
    public class InitApp
    {
        public static int Init(string[] args)
        {
            //设定当前程序运行的主上下文
            GlobalContext.SyncContext = SynchronizationContext.Current;


            try
            {
                //加载CEF 运行时  lib cef
                CefRuntime.Load();
            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);
                return 3;
            }


          

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

            #region winform 有窗口的加载渲染方式


            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //if (!multiThreadedMessageLoop)
            //{
            //    Application.Idle += (sender, e) => { CefRuntime.DoMessageLoopWork(); };
            //}

            //Application.Run(new MainForm());//--调试程序窗口页面
            //Application.Run(new HeadLessMainForm());


            ////注销CEF app
            //app.Dispose();

            /////终止CEF 线程
            //CefRuntime.Shutdown();
            #endregion

            return 0;
        }

    }
}
