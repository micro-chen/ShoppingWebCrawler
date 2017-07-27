using ShoppingWebCrawler.Host.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingWebCrawler.Host.AppStart
{
    /// <summary>
    /// 定时将控制台中的字符清屏处理
    /// </summary>
    public static class ConsoleClean
    {
        /// <summary>
        /// 定时器 用来定时清理屏幕信息
        /// </summary>
        private static System.Timers.Timer _timer_monitor_console;

        /// <summary>
        /// 定时清理周期（默认5min）
        /// 配置文件 配置节：ConsoleCleanSpan
        /// </summary>
        private static int ConsoleCleanSpan
        {
            get
            {
                int configSpan = ConfigHelper.GetConfigInt("ConsoleCleanSpan");
                return configSpan > 0 ? configSpan : 5;
            }
        }

        /// <summary>
        /// 开启监听
        /// </summary>
        public static void Start()
        {
            if (null != _timer_monitor_console)
            {
                _timer_monitor_console.Stop();
                _timer_monitor_console.Dispose();
            }

            _timer_monitor_console = new System.Timers.Timer(1000 * 60 * ConsoleCleanSpan);
            _timer_monitor_console.Elapsed += (s, e) =>
            {
                Console.Clear();
            };

            _timer_monitor_console.Start();
        }

        /// <summary>
        /// 停止监视
        /// </summary>
        public static void Stop()
        {
            if (null!=_timer_monitor_console)
            {
                _timer_monitor_console.Stop();
                _timer_monitor_console.Dispose();
            }
        }
    }
}
