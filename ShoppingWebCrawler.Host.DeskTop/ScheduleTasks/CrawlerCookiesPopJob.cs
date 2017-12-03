
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Quartz;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Common;

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
        /// 发送内部消息到监听
        /// </summary>
        /// <param name="msg"></param>
        private static void SendMessageToListerner(string msg)
        {
            if (null != OnSendCookiesToRemoteEvent)
            {
                OnSendCookiesToRemoteEvent.Invoke(null, msg);
            }

        }
        /// <summary>
        /// 发送客户端搜集的平台的cookies 集合到远程
        /// </summary>
        public static void SendCookiesToRemote()
        {
            string msg = "SendCookiesToRemote 被执行！时间："+DateTime.Now.ToString();
            Logger.Info(msg);

            SendMessageToListerner(msg);

            //todo:实现拉取cookies 然后发送到远程server
            var allPlatforms = Enum.GetValues(typeof(SupportPlatformEnum));// SupportPlatformEnum.Alimama.get
            var cookieLoader = new LazyCookieVistor();
            foreach (SupportPlatformEnum platform in allPlatforms)
            {
                var siteObj = GlobalContext.SupportPlatforms.Find(x => x.Platform == platform);
                if (null == siteObj)
                {
                    string platformDescription = platform.GetEnumDescription();
                    string errMsg=string.Format($"CrawlerCookiesPopJob,未能正确从配置文件加载平台地址：{platformDescription ?? platform.ToString()}");
                    SendMessageToListerner(errMsg);
                    continue;
                }
                var domian = siteObj.SiteUrl;
                var cks = new LazyCookieVistor().LoadCookies(domian);

                 
                if (null != cks && cks.IsNotEmpty())
                {
                    GlobalContext.DeskPushToRedisCookies(platform, cks);
                }


            }
        }
          
    }
}
