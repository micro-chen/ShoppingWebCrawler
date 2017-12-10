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

        #region 示范代码

        //    //向IPC  render  进程注册事件委托
        //    IPCCommand.OnGetCookieFromBrowserProcessHandler += handler;

        //            try
        //            {
        //                //从当前的render 绑定的browser对象，发送进程消息
        //                if (null == GlobalContext.SlaveModeCefBrowserInRenderProcess)
        //                {
        //                    string msg = "在 render 进程无对应的browser 对象！！";
        //    Logging.Logger.Info(msg);
        //                    Console.WriteLine(msg);
        //                }
        //var message = CefProcessMessage.Create(IPCCommand.CommandType.GET_COOKIE_FROM_BROWSER_PROCESS.ToString());
        //message.Arguments.SetString(0, domainName);
        //                var success = GlobalContext.SlaveModeCefBrowserInRenderProcess.SendProcessMessage(CefProcessId.Browser, message);
        //Console.WriteLine("Sending myMessage3 to browser process = {0}", success);

        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }

        #endregion
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
