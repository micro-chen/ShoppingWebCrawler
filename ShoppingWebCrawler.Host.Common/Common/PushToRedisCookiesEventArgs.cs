using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;

namespace ShoppingWebCrawler.Host.Common
{

    /// <summary>
    /// 推送到远程redis服务器cookies 事件参数
    /// </summary>
    public class PushToRedisCookiesEventArgs:EventArgs
    {
        /// <summary>
        /// 平台枚举
        /// </summary>
        public SupportPlatformEnum Platform
        {
            get; set;
        }
        /// <summary>
        /// 发送的cookie 内容
        /// </summary>
        public string CookiesContent { get; set; }
    }



}
