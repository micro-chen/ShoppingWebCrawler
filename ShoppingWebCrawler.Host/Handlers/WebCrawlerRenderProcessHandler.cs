using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Core.Wrapper;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Cef.Framework;
using System.Collections;
using Newtonsoft.Json;

namespace ShoppingWebCrawler.Host.Handlers
{
    /// <summary>
    /// 存活在 render 进程中
    /// </summary>
    internal sealed class WebCrawlerRenderProcessHandler : CefRenderProcessHandler
    {
 

       // public event EventHandler<CefBrowser> HandlerOfOnBrowserCreated;

        internal static bool DumpProcessMessages { get; private set; }

        public WebCrawlerRenderProcessHandler()
        {
            DumpProcessMessages = true;
            MessageRouter = new CefMessageRouterRendererSide(new CefMessageRouterConfig());
        }

        internal CefMessageRouterRendererSide MessageRouter { get; private set; }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextCreated(browser, frame, context);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextReleased(browser, frame, context);
        }
        /// <summary>
        /// 当CefBrowser 创建完毕后的事件
        /// </summary>
        /// <param name="browser"></param>
        protected override void OnBrowserCreated(CefBrowser browser)
        {
            //if (null != HandlerOfOnBrowserCreated)
            //{
            //    HandlerOfOnBrowserCreated.Invoke(this, browser);
            //}

            //var message3 = CefProcessMessage.Create("myMessage33");
            //message3.Arguments.SetString(0, "AAAAAAAAAAAA");
            //var success2 = browser.SendProcessMessage(CefProcessId.Browser, message3);
            //Console.WriteLine("Sending myMessage3 to browser process = {0}", success2);
            if (GlobalContext.IsInSlaveMode==true)
            {
                //当在从节点 工作的时候，其实是在 render 进程工作，此时记录下对应的cef browser 对象
                GlobalContext.SlaveModeCefBrowserInRenderProcess = browser;
            }
        }

        /// <summary>
        /// 从对应的browser 进程中消息过滤filter 处理
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="sourceProcess"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (DumpProcessMessages)
            {
                Console.WriteLine("Render::OnProcessMessageReceived: SourceProcess={0}", sourceProcess);
                Console.WriteLine("Message Name={0} IsValid={1} IsReadOnly={2}", message.Name, message.IsValid, message.IsReadOnly);
                var arguments = message.Arguments;
                for (var i = 0; i < arguments.Count; i++)
                {
                    var type = arguments.GetValueType(i);
                    object value;
                    switch (type)
                    {
                        case CefValueType.Null: value = null; break;
                        case CefValueType.String: value = arguments.GetString(i); break;
                        case CefValueType.Int: value = arguments.GetInt(i); break;
                        case CefValueType.Double: value = arguments.GetDouble(i); break;
                        case CefValueType.Bool: value = arguments.GetBool(i); break;
                        default: value = null; break;
                    }

                    Console.WriteLine("  [{0}] ({1}) = {2}", i, type, value);
                }
            }

            var handled = MessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
            if (handled) return true;

            //if (message.Name == "myMessage2") return true;

            //var message2 = CefProcessMessage.Create("myMessage2");
            //var success = browser.SendProcessMessage(CefProcessId.Renderer, message2);
            //Console.WriteLine("Sending myMessage2 to renderer process = {0}", success);

            //var message3 = CefProcessMessage.Create("myMessage3");
            //var success2 = browser.SendProcessMessage(CefProcessId.Browser, message3);
            //Console.WriteLine("Sending myMessage3 to browser process = {0}", success);
            if (message.Name.Equals(IPCCommand.CommandType.GET_COOKIE_FROM_BROWSER_PROCESS.ToString()))
            {
                var argumentsInMsg = message.Arguments;
                string domainName = argumentsInMsg.GetString(0);
                string message_cookies = argumentsInMsg.GetString(1);
                if (!string.IsNullOrEmpty(domainName))
                {
                    //todo call back
                    if (!string.IsNullOrEmpty(message_cookies))
                    {
                        var lstCookies = JsonConvert.DeserializeObject<List<CefCookie>>(message_cookies);
                        IPCCommand.OnGetCookieFromBrowserProcess(domainName, lstCookies);
                    }
                 
                 
                }
            }
            return true;
        }
    }

}
