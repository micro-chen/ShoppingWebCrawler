using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.AppStart
{
    public class AppBroker
    {



        /// <summary>
        /// 监视主节点是否存活
        /// 主节点退出  从节点全部结束进程。进程监视 broker
        /// </summary>
        public static void MonitorIsMasterCenterAlive()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    RunningLocker.CreateNewLock().CancelAfter(1000);

                    try
                    {
                        bool isBeUsed = MasterRemoteServer.IsMasterStarted();
                        if (isBeUsed == false)
                        {
                            //主进程都已经退出，那么残留的进程都没有意义
                            AppBroker.TerminalApplicationProcess();

                        }
                    }
                    catch (Exception ex)
                    {

                        Common.Logging.Logger.Error(ex);
                        break;
                    }

                }


            });

        }

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
                    ps.Kill();//终止同名的其他进程
                }
            }
            mainProcess.Kill();


        }
    }
}
