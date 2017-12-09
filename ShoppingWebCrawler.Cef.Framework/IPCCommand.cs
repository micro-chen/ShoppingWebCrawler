using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Cef.Framework
{
    /// <summary>
    /// CEF3的进程之间可以通过IPC进行通信。Browser和Render进程可以通过发送异步消息进行双向通信。
    /// 甚至在Render进程可以注册在Browser进程响应的异步JavaScript API。 
    /// 更多细节，请参考Inter-Process Communication一节。
    /// </summary>
    public class IPCCommand
    {
        public enum CommandType
        {
            /// <summary>
            /// 从browser进程获取 cookie
            /// </summary>
            GET_COOKIE_FROM_BROWSER_PROCESS,

        }

        /// <summary>
        /// 当从browser 进程获取完毕cookie 触发的事件
        /// </summary>
        public static event EventHandler<CookieVistCompletedEventAgrs> OnGetCookieFromBrowserProcessHandler;
        public static void OnGetCookieFromBrowserProcess(string domainName, List<CefCookie> cookies)
        {
            if (null != OnGetCookieFromBrowserProcessHandler)
            {
                var args = new CookieVistCompletedEventAgrs { DomainName = domainName, Results = cookies };
                OnGetCookieFromBrowserProcessHandler.Invoke("ipc", args);
            }
        }
    }
}
