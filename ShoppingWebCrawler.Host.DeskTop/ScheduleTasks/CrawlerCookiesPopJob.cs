
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using ShoppingWebCrawler.Host.Common.Logging;

namespace ShoppingWebCrawler.Host.DeskTop.ScheduleTasks
{
    public sealed class CrawlerCookiesPopJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("ReportManageJob 被执行！");

            // 获取传递过来的参数            
            JobDataMap jobParasData = context.JobDetail.JobDataMap;

        }

          
    }
}
