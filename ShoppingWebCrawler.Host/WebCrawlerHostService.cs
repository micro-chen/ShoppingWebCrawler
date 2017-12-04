using ShoppingWebCrawler.Host.AppStart;
using ShoppingWebCrawler.Host.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ShoppingWebCrawler.Host
{
    public sealed class WebCrawlerHostService : ServiceControl, ServiceSuspend
    {


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
                InitCefApp.Start(null);
                result = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "开启 WebCrawlerHostService 失败！");

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
                InitCefApp.Stop(null);
                result = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "关闭 WebCrawlerHostService 失败！");

            }

            return result;
        }

        public bool Pause(HostControl hostControl)
        {
            Logger.Info("WebCrawlerHostService is Paused!");
            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            Logger.Info("WebCrawlerHostService is Continue......");
            return true;
        }
    }
}
