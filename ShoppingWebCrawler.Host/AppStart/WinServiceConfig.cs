using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ServiceProcess;

using Topshelf;
using Topshelf.Hosts;
using log4net.Config;
using ShoppingWebCrawler.Host.Common.Logging;

namespace ShoppingWebCrawler.Host.AppStart
{
    public class WinServiceConfig
    {

        public const string Service_Name = "WebCrawlerHostService";


        /// <summary>
        /// 标识是否正在基于 topshelf 进行的windows 服务承载
        /// </summary>
        public static bool IsTopshelfWinServiceInit = false;
        /// <summary>
        /// 初始化 当前WinService
        /// </summary>
        public static void Init(string[] args)
        {

            IsTopshelfWinServiceInit = true;

            HostFactory.Run(x =>
            {
                x.Service<WebCrawlerHostService>(s =>
                {

                    s.ConstructUsing(name => new WebCrawlerHostService());
                    s.WhenStarted(tc =>
                    {
                     
                        Logger.Info("服务 WebCrawlerHostService 开启");
                    });
                    s.WhenStopped(tc =>
                    {
                       
                        Logger.Info("服务 WebCrawlerHostService 停止");
                    }
                    );

                    //s.WhenPaused(tc =>
                    //{
                    //    //ScheduleTaskRunner.Instance.Pause();
                    //   Logger.Info("服务 WebCrawlerHostService 暂停");
                    //});
                    //s.WhenContinued(tc =>
                    //{
                    //    //ScheduleTaskRunner.Instance.Continue();
                    //    Logger.Info("服务 WebCrawlerHostService 停止");
                    //});

                });


                //以网络服务承载
                x.RunAsNetworkService();



                //使用log4进行日志记录
                var log4ConfigFile = Common.Logging.Logger.ConfigFilePath;
                x.UseLog4Net(log4ConfigFile);
                //配置服务显示
                x.SetDescription("本项目用来做 服务端的Chrome集成！实现代理解析电商平台的数据！");
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
