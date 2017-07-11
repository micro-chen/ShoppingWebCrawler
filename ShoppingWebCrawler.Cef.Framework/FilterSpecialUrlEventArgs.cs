using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Cef.Framework
{
    /// <summary>
    /// 拦截到指定的url 事件参数
    /// </summary>
    public class FilterSpecialUrlEventArgs:EventArgs
    {
        /// <summary>
        /// cef browser 实例
        /// </summary>
        public CefBrowser Browser { get; set; }

        /// <summary>
        /// 指定的请求地址
        /// </summary>
        public string Url { get; set; }
    }
}
