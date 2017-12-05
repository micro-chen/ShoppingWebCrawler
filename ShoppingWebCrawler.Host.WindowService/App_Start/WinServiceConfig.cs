using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Topshelf;
using Topshelf.Hosts;
using System.ServiceProcess;
using log4net.Config;
using Topshelf.Logging;
using ShoppingWebCrawler.Host.Common.Logging;

namespace ShoppingWebCrawler.Host.WindowService.App_Start
{
    public class WinServiceConfig
    {

        public const string Service_Name = "WebCrawler.Host.WindowService";

        /// <summary>
        /// 标识是否正在基于 topshelf 进行的windows 服务承载
        /// </summary>
        public static bool IsTopshelfWinServiceInit { get; set; }

         /// <summary>
        /// 初始化 当前WinService
        /// </summary>
        public static void Init()
        {
          

            if (IsTopshelfWinServiceInit==true)
            {
                return;
            }
            IsTopshelfWinServiceInit = true;

            HostFactory.Run(x =>
            {
                x.Service<ShoppingWebCrawlerHostMonitor>();


                //下面是通过编程的方式 自定义服务行为
               //x.Service<ShoppingWebCrawlerHostMonitor>(
               //     s =>
               // {

               //     s.ConstructUsing(name => new ShoppingWebCrawlerHostMonitor());
               //     //s.WhenStarted(tc =>
               //     //{
               //     //    Logger.Info($"服务 {Service_Name} 开启");
               //     //});
               //     //s.WhenStopped(tc =>
               //     //{
               //     //    Logger.Info($"服务 {Service_Name} 停止");
               //     //}
               //     );

               //     //s.WhenPaused(tc =>
               //     //{
               //     //    ScheduleTaskRunner.Instance.Pause();
               //     //    Logging.Logger.Info("服务 ReportManageService 暂停");
               //     //});
               //     //s.WhenContinued(tc =>
               //     //{
               //     //    ScheduleTaskRunner.Instance.Continue();
               //     //    Logging.Logger.Info("服务 ReportManageService 停止");
               //     //});

               // });


                //以网络服务承载
                x.RunAsNetworkService();



                //使用log4进行日志记录
                var log4ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "log4net.config");
                if (!File.Exists(log4ConfigFile))
                {
                    throw new Exception("未能找到 log4net 的配置文件！在路径：" + log4ConfigFile);
                }
                x.UseLog4Net(log4ConfigFile);
                //配置服务显示
                x.SetDescription($"本项目用来监视 进程{ShoppingWebCrawlerHostMonitor.ToMonitAppProcessName} ,监视运行健康情况！");
                x.SetDisplayName(Service_Name);
                x.SetServiceName(Service_Name);

                //配置进程服务恢复
                x.EnableServiceRecovery(rc =>
                {
                    //当服务崩溃的时候 ，自动重启服务
                    rc.OnCrashOnly();
                    rc.RestartService(1); // restart the service after 1 minute
                    rc.SetResetPeriod(1); // set the reset interval to one day
                });



                //配置安装完毕后自动启动
                x.AfterInstall((settings) =>
                {


                    try
                    {
                        ServiceController sc = new ServiceController(Service_Name);

                        if (sc.Status.Equals(ServiceControllerStatus.Stopped))
                        {
                            sc.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                });


                //配置服务自动启动
                x.StartAutomatically();

                //Logging.Logger.Info(DateTime.Now.ToString());


            });


        }
    }
}
