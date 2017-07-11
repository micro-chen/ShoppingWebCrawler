
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;

using ShoppingWebCrawler.Host.Handlers;

namespace ShoppingWebCrawler.Host.Headless
{
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
