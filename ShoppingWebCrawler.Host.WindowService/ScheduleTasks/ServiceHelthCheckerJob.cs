
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using ShoppingWebCrawler.Host.WindowService.App_Start;

namespace ShoppingWebCrawler.Host.WindowService.ScheduleTasks
{
    /// <summary>
    /// 监视的host  进程的健康监视
    /// 用一定的频率设置，发送心跳包，一旦为能成功返回TCP结果 那么重启服务
    /// </summary>
    public sealed class ServiceHelthCheckerJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            WinServiceConfig.Logger.Info("ServiceHelthCheckerJob 被执行！");

            if (ShoppingWebCrawlerHostMonitor.IsServiceRunning==true)
            {
                var checkResult = RemoteTcpTestClient.TestPingSendMessage();
                if (false == checkResult)
                {
                    //一旦返回结果不对 那么重新启动host  进程
                    ShoppingWebCrawlerHostMonitor.StartWebCrawlerHostProcess();

                }
            }
           

        }


    }
}
