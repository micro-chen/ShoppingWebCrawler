using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.AppStart
{
    public class AppBroker
    {

        private static Timer _timer_supervisor_main = new Timer(1000);
        private static Timer _timer_supervisor_renderGC = new Timer(ConfigHelper.GetConfigInt("RenderProcessGCLifeTime")*1000);


        public static void Start()
        {
            MonitorIsMasterCenterAlive();
        }


        #region 监视残留的render进程的broker



       
        private static object _locker_renderGC = new object();
        /// <summary>
        /// 定期回收清理残留的render 进程
        /// </summary>
        public static void MonitorClearRenderProcessByLifeTime()
        {

            lock (_locker_renderGC)
            {


                _timer_supervisor_renderGC.Elapsed += (s, e) =>
                {
                    try
                    {

                        Process mainProcess = Process.GetCurrentProcess();
                        var appName = Assembly.GetExecutingAssembly().GetName().Name;
                        var psArray = Process.GetProcessesByName(appName);
                        foreach (var ps in psArray)
                        {
                            var cmdInfo = ps.GetCommandLine();
                            if (!string.IsNullOrEmpty(cmdInfo)&&cmdInfo.Contains("--type=renderer"))
                            {
                                ps.Kill();//终止render进程
                            }
                           
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.Logging.Logger.Error(ex);

                    }

                };
                _timer_supervisor_renderGC.Start();


            


            }
        }

        #endregion

        #region 监视主进程的broker



        private static bool _isInit = false;
        private static object _locker = new object();
        /// <summary>
        /// 监视主节点是否存活
        /// 主节点退出  从节点全部结束进程。进程监视 broker
        /// </summary>
        public static void MonitorIsMasterCenterAlive()
        {

            //RunningLocker.CreateNewLock().CancelAfter(1000);
            lock (_locker)
            {
                if (_isInit == true)
                {
                    return;
                }

                _timer_supervisor_main.Elapsed += (s, e) =>
                {
                    try
                    {
                        var mainProcess = System.Diagnostics.Process.GetProcessById(GlobalContext.MainProcessId);
                        if (mainProcess == null)
                        {
                        //主进程都已经退出，那么残留的进程都没有意义
                        AppBroker.TerminalApplicationProcess();

                        }
                    }
                    catch (Exception ex)
                    {
                        AppBroker.TerminalApplicationProcess();

                        Common.Logging.Logger.Error(ex);

                    }

                };
                _timer_supervisor_main.Start();


                _isInit = true;


            }
        }

        #endregion

        /// <summary>
        /// 清理同名的当前程序的其它残留进程
        /// </summary>
        public static void ClearGarbageProcess()
        {
            Process mainProcess = Process.GetCurrentProcess();
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var psArray = Process.GetProcessesByName(appName);
            foreach (var ps in psArray)
            {
                if (ps.Id != mainProcess.Id)
                {
                    ps.Kill();//终止同名的其他进程
                }
            }


        }

        /// <summary>
        /// 终止当前程序的全部进程
        /// </summary>
        public static void TerminalApplicationProcess()
        {
            Process mainProcess = Process.GetCurrentProcess();
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var psArray = Process.GetProcessesByName(appName);
            foreach (var ps in psArray)
            {
                if (ps.Id != mainProcess.Id)
                {
                    try
                    {
                        ps.Kill();//终止同名的其他进程
                    }
                    catch { }
                    
                }
            }
            mainProcess.Kill();


        }
    }
}
