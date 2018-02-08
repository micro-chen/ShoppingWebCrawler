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
using System.Diagnostics;

namespace ShoppingWebCrawler.Host.AppStart
{
    public class InitApp
    {
        public static int Init(string[] args)
        {
            //1 设定当前程序运行的主上下文
            GlobalContext.SyncContext = SynchronizationContext.Current;



            //1-1 集群节点的判定 一旦进程启动参数有此标识，那么表示是子节点进程需要启动
            //子节点仅仅用来做负载均衡的分流，不承载cef 运行时
            //"type=slavemode mainProcessId={0}";
            if (null != args
                && string.Concat(args).Contains("slavemode"))
            {
                GlobalContext.IsInSlaveMode = true;
                //从启动参数 获取主进程的id
                var paramProcess = args.FirstOrDefault(x => x.Contains("mainProcessId"));
                if (null != paramProcess)
                {
                    GlobalContext.MainProcessId = int.Parse(paramProcess.Split('=')[1]);
                    SlaveRemoteServer.Start();
                }

                return 0;//子节点的进程启动完毕后，返回
            }


            //2 初始化CEF运行时
            #region 初始化CEF运行时

            try
            {
                //加载CEF 运行时  lib cef
                CefRuntime.Load();

                var mainArgs = new CefMainArgs(args);
                var app = new HeadLessWebBrowerApp();


                //执行CEF启动进程，如果是二级进程 ，那么执行后返回exitCode，启动进程是 -1
                /*// CefExecuteProcess returns -1 for the host process
                 CefExecuteProcess()方法来检测是否要启动其它的子进程。此处的CefExecuteProcess是在libcef_dll_wrapper.cc中的，它内部又调用了cef_execute_process方法（libcef_dll.cc），cef_execute_process又调用了libcef/browser/context.cc文件内实现的CefExecuteProcess方法
                它分析了命令行参数，提取”type”参数，如果为空，说明是Browser进程，返回-1，这样一路回溯到wWinMain方法里，然后开始创建Browser进程相关的内容。
如果”type”参数不为空，做一些判断，最后调用了content::ContentMain方法，直到这个方法结束，子进程随之结束。
                 *  */

                var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
                if (exitCode != -1)
                {
                    return exitCode;
                }

                //一旦为 -1 ,那么表示为Host  进程，对cef  运行时进行初始化操作---

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
                    PersistSessionCookies = true,
                    UserAgent = GlobalContext.ChromeUserAgent,
                    UserDataPath = System.IO.Path.Combine(Environment.CurrentDirectory, "UserData"),
                    CachePath = System.IO.Path.Combine(Environment.CurrentDirectory, "LocalCache")
                };


                // DevTools doesn't seem to be working when this is enabled
                // http://magpcss.org/ceforum/viewtopic.php?f=6&t=14095



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
            MasterRemoteServer.Start();
            #region 开启集群模式
            if (GlobalContext.IsConfigClusteringMode)
            {
                var clusterNodeCount = ConfigHelper.GetConfigInt("ClusterNodeCount");
                if (clusterNodeCount <= 0)
                {
                    clusterNodeCount = 1;
                }

                try
                {
                    //清理残留进程
                    AppBroker.ClearGarbageProcess();

                    var mainProcess = Process.GetCurrentProcess();
                    string appName = Assembly.GetExecutingAssembly().GetName().Name;
                    //开启子节点进程
                    for (int i = 0; i < clusterNodeCount; i++)
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.exe", appName));
                        p.StartInfo.Arguments = string.Format(GlobalContext.SlaveModelStartAgrs, mainProcess.Id);//集群节点的启动参数标识
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.Start();
                        //延迟启动下一个从节点进程，防止并发分配端口资源导致失败
                        RunningLocker.CreateNewLock().CancelAfter(1000);

                    }
                }
                catch (Exception ex)
                {

                    Logger.Error(ex);
                }

            }
            #endregion


            //4 定时清理器
            #region 定时清理控制台
            //ConsoleClean.Start();
            #endregion

            //5 阿里妈妈等平台登录
            #region  阿里妈妈等平台网页初始化操作



            //----------注意：由于开启了多进程模型，不同的网址在不同的tab,每个tab 在其独立的进程中-------
            //每次打开一个程序进程，让进程开启一个端口server,向总控端口，发送注册登记，用来接受请求转发，做负载均衡---

            //初始化平台网页进程
            //1 通过反射 获取所有的webpage service --正式版

            var ass = Assembly.GetExecutingAssembly();
            var typeFinder = new AppDomainTypeFinder();
            var targetType = typeof(BaseWebPageService);
            var webPageServiceTypes = typeFinder.FindClassesOfType(targetType, new Assembly[] { ass }, true);
            if (webPageServiceTypes.IsNotEmpty())
            {
                foreach (Type itemPageService in webPageServiceTypes)
                {
                    #region 没用的服务 先不启用


                    if (typeof(VipWebPageService).Equals(itemPageService))
                    {
                        continue;
                    }
                    if (typeof(MogujieWebPageService).Equals(itemPageService))
                    {
                        continue;
                    }
                    if (typeof(YhdWebPageService).Equals(itemPageService))
                    {
                        continue;
                    }

                    #endregion

                    try
                    {

                        BaseWebPageService servieInstance = Activator.CreateInstance(itemPageService) as BaseWebPageService;
                        //静态属性访问一次 即可触发打开页面
                        var loader = servieInstance.RequestLoader;

                    }
                    catch (Exception ex)
                    {

                        Logger.Error(ex);
                    }
                 

                }
            }

            // --------------测试环境begin 不建议打开多个tabpage，影响测试加载-------------
            //var tmallService = new TmallWebPageService();
            //var loader = tmallService.RequestLoader;

            //var loader_taobao = new TaobaoWebPageService().RequestLoader;
            //var loader_jd = new JingdongWebPageService().RequestLoader;
            //var loader_dangdang = new DangdangWebPageService().RequestLoader;
            //--------------测试环境end-------使用一个tabpage ,即可测试是否正确加载----------


            #endregion



            return 0;
        }




    }
}
