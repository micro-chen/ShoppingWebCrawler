
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;

using ShoppingWebCrawler.Host.Handlers;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.Headless
{
    /// <summary>
    /// 这里存活在 browser 进程中
    /// </summary>
    internal class HeadLessCefClient : CefClient
    {
        private readonly HeadLessCefLoadHandler _loadHandler;
        private readonly HeadLessCefRenderHandler _renderHandler;
        private readonly HeadLessCefWebLifeSpanHandler _lifeSpanHandler;
        private readonly HeadLessWebRequestHandler _requestHandler;



        public HeadLessCefClient(int windowWidth, int windowHeight)
        {
            _loadHandler = new HeadLessCefLoadHandler();
            _renderHandler = new HeadLessCefRenderHandler(windowWidth, windowHeight);
            _lifeSpanHandler = new HeadLessCefWebLifeSpanHandler();
            _requestHandler = new HeadLessWebRequestHandler();
        }

        /// <summary>
        /// 获取当前client承载的 LoadHandler 对象
        /// </summary>
        public HeadLessCefLoadHandler GetCurrentLoadHandler()
        {
            HeadLessCefLoadHandler loader = this.GetLoadHandler() as HeadLessCefLoadHandler ;


            return loader;
        }

        /// <summary>
        /// 从其对应的render 进程中接收过来的消息过滤处理
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="sourceProcess"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
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
            //filters 
            if (message.Name.Equals(IPCCommand.CommandType.GET_COOKIE_FROM_BROWSER_PROCESS.ToString()))
            {
                var argumentsInMsg = message.Arguments;
                string domainName = argumentsInMsg.GetString(0);
                if (!string.IsNullOrEmpty(domainName))
                {
                    //获取指定域名的cookie 
                    var cks = new LazyCookieVistor().LoadNativCookies(domainName);
                    if (cks.IsNotEmpty())
                    {
                        var cookieString = cks.ToJson();
                        var message_cookies = CefProcessMessage.Create(IPCCommand.CommandType.GET_COOKIE_FROM_BROWSER_PROCESS.ToString());
                        message_cookies.Arguments.SetString(0, domainName);
                        message_cookies.Arguments.SetString(1, cookieString);
                        var success = browser.SendProcessMessage(CefProcessId.Renderer, message_cookies);
                        Console.WriteLine("Sending myMessage3 to browser process = {0}", success);
                    }
                }
            }
            return true;
            //return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }
        /// <summary>
        /// 暴露此client 的请求处理实例
        /// </summary>
        /// <returns></returns>
        public HeadLessWebRequestHandler GetRequestHandlerInstance() {
            return _requestHandler;
        }

        protected override CefRequestHandler GetRequestHandler()
        {
            return _requestHandler;
        }
        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }
    }
}
