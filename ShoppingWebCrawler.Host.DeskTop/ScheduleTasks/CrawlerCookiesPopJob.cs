
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

        private static object _locker = new object();
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
            string msg = "SendCookiesToRemote 被执行！时间：" + DateTime.Now.ToString();
            Logger.Info(msg);

            lock (_locker)
            {


                SendMessageToListerner(msg);

                //todo:实现拉取cookies 然后发送到远程server
                var allPlatforms = Enum.GetValues(typeof(SupportPlatformEnum));// SupportPlatformEnum.Alimama.get
                                                                               //var cookieLoader = new LazyCookieVistor();
                foreach (SupportPlatformEnum platform in allPlatforms)
                {
                    var siteObj = GlobalContext.SupportPlatforms.Find(x => x.Platform == platform);
                    if (null == siteObj)
                    {
                        string platformDescription = platform.GetEnumDescription();
                        string errMsg = string.Format($"CrawlerCookiesPopJob,未能正确从配置文件加载平台地址：{platformDescription ?? platform.ToString()}");
                        SendMessageToListerner(errMsg);
                        continue;
                    }
                    var domian = siteObj.SiteUrl;
                    var cks = new LazyCookieVistor().LoadNativCookies(domian,true);


                    if (null != cks && cks.IsNotEmpty())
                    {
                        //淘宝的cookie 附加
                        if (platform == SupportPlatformEnum.Taobao)
                        {
                            //推送爱淘宝-券官网的cookie到淘宝
                            var cks_aiTaoBao = new LazyCookieVistor().LoadNativCookies(GlobalContext.AiTaobaoSiteURL, true);
                            if (null != cks_aiTaoBao && cks_aiTaoBao.IsNotEmpty())
                            {
                                for (int i = 0; i < cks_aiTaoBao.Count; i++)
                                {
                                    var item = cks_aiTaoBao.ElementAt(i);
                                    if (cks.FirstOrDefault(x => x.Name == item.Name) != null)
                                    {
                                        continue;//跳过重名的cookie
                                    }
                                    cks.Add(item);
                                }
                            }
                        }
                        GlobalContext.PushToRedisCookies(domian, cks);
                    }


                }

                //推送轻淘客cookies
                var cks_qingTaoke = new LazyCookieVistor().LoadNativCookies(GlobalContext.QingTaokeSiteURL,true);
                if (null != cks_qingTaoke && cks_qingTaoke.IsNotEmpty())
                {
                    GlobalContext.PushToRedisCookies(GlobalContext.QingTaokeSiteURL, cks_qingTaoke);
                }


            }
        }

    }
}
