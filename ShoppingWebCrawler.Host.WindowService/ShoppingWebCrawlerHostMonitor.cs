using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.WindowService.App_Start;
using ShoppingWebCrawler.Host.WindowService.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ShoppingWebCrawler.Host.WindowService
{
    public class ShoppingWebCrawlerHostMonitor : ServiceControl
    {
        /// <summary>
        /// 监视的进程名称
        /// </summary>
        public const string ToMonitAppProcessName = "ShoppingWebCrawler.Host";
        /// <summary>
        /// 服务是否正在运行中
        /// </summary>
        public static bool IsServiceRunning = false;
        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {
            var result = false;
            try
            {
                StartWebCrawlerHostProcess();
                IsServiceRunning = true;
                ScheduleTaskRunner.Instance.Start();
               
                result = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

            }


            return result;
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            var result = false;
            try
            {
                ScheduleTaskRunner.Instance.Stop();
                StopWebCrawlerHostProcess();
                IsServiceRunning = false;
                result = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

            }

            return result;
        }


        /// <summary>
        /// 开启蜘蛛服务进程
        /// </summary>
        public static void StartWebCrawlerHostProcess() {
            
            try
            {
                //先杀死其他残留的进程 相当于重启
                StopWebCrawlerHostProcess();

                Process p = new Process(); //实例一个Process类，启动一个独立进程

                p.StartInfo.FileName = $"{ToMonitAppProcessName}.exe";  //设定程序名
               // p.StartInfo.Arguments = "/c " + command;  //设定程式执行参数   
                p.StartInfo.UseShellExecute = false;    //关闭Shell的使用
                //p.StartInfo.RedirectStandardInput = true;  //重定向标准输入
                //p.StartInfo.RedirectStandardOutput = true; //重定向标准输出  
                //p.StartInfo.RedirectStandardError = true;   //重定向错误输出         
                p.StartInfo.CreateNoWindow = true;  //设置不显示窗口
                p.Start();    //启动

                Logger.Info("已经开启 蜘蛛进程！");

                //return p.StandardOutput.ReadToEnd(); //从输出流取得命令执行结果
            }
            catch (Exception ex)
            {

                Logger.Error(ex);
            }
        }
        /// <summary>
        /// 终止蜘蛛服务进程
        /// </summary>
        public static void StopWebCrawlerHostProcess()
        {
            Logger.Info("清理关闭 蜘蛛进程！");
            try
            {
                ////根据进程命获得指定的进程
                Process[] ps = Process.GetProcessesByName(ToMonitAppProcessName);
                if (ps.Any())
                {
                    foreach (var item in ps)
                    {
                        item.Kill(); //杀死进程
                    }
                }
               
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


    }
}
