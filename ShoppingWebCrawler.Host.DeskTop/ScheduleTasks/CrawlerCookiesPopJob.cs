
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
        /// <summary>
        /// 发送cookie 触发的事件
        /// </summary>
        public static event EventHandler<string> OnSendCookiesToRemoteEvent;

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("CrawlerCookiesPopJob 被执行！");

            // 获取传递过来的参数            
            JobDataMap jobParasData = context.JobDetail.JobDataMap;

            SendCookiesToRemote();
        }

        /// <summary>
        /// 发送客户端搜集的平台的cookies 集合到远程
        /// </summary>
        public static void SendCookiesToRemote()
        {
            string msg = "SendCookiesToRemote 被执行！时间："+DateTime.Now.ToString();
            Logger.Info(msg);
            if (null!=OnSendCookiesToRemoteEvent)
            {
                OnSendCookiesToRemoteEvent.Invoke(null, msg);
            }

            //todo:实现拉取cookies 然后发送到远程server

        }
          
    }
}
