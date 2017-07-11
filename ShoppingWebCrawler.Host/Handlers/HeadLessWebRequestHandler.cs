
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;

namespace ShoppingWebCrawler.Host.Handlers
{


    sealed class HeadLessWebRequestHandler : CefRequestHandler
    {

        /// <summary>
        /// 当开始请求指定的url地址的时候触发事件
        /// </summary>
        public event EventHandler<FilterSpecialUrlEventArgs> OnRequestTheMoniterdUrl;


        public HeadLessWebRequestHandler()
        {

        }


        /// <summary>
        /// 请求资源的时候  Handler方法
        /// 如：js css 图片等资源
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected override CefReturnValue OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request, CefRequestCallback callback)
        {
            ///--------TODO:这些配置回头需要设置到配置文件中，暂时没时间实现 回头有时间再弄  ------------
            //if (request.Url.Contains("mtop.etao.fe.search/"))
            //{
            //    int x = 9;
            //    return true;
            //}

            //监视一淘的请求数据的接口地址
            if (request.Url.Contains("apie.m.etao.com/h5/mtop.etao.fe.search"))
            {
                if (null!=this.OnRequestTheMoniterdUrl)
                {
                    this.OnRequestTheMoniterdUrl(this, new FilterSpecialUrlEventArgs { Browser = browser, Url = request.Url });
                    //当有监听的时候 就不再请求此url了  让 监听去处理此url
                    return CefReturnValue.Cancel;
                }
                
            }

            var reg = new System.Text.RegularExpressions.Regex(@"(http:|https:).*\.(woff|css|jpg|jpeg|png|ttf|svg|json)\?{0,}.*", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (reg.IsMatch(request.Url))
            {
                return CefReturnValue.Cancel;
            }

            if (request.Url.Contains("img.alicdn.com"))
            {
                return CefReturnValue.Cancel;
            }
            if (request.Url.Contains("qrlogin.taobao.com"))
            {
                return CefReturnValue.Cancel;
            }
            if (request.Url.Contains("tanx.com"))
            {
                return CefReturnValue.Cancel;
            }
            if (request.Url.Contains("wwc.alicdn.com"))
            {
                return CefReturnValue.Cancel;
            }
            if (request.Url.Contains("wwc.alicdn.com"))
            {
                return CefReturnValue.Cancel;
            }

            return base.OnBeforeResourceLoad(browser, frame, request, callback);
        }

        /// <summary>
        /// Called on the IO thread before a resource request is loaded. The |request|
        /// object may be modified. To cancel the request return true otherwise return
        /// false.
        /// </summary>
        //protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        //{
        //    //if (request.Url.Contains("mtop.etao.fe.search/"))
        //    //{
        //    //    int x = 9;
        //    //    return true;
        //    //}



        //    var reg = new System.Text.RegularExpressions.Regex(@"(http:|https:).*\.(woff|css|jpg|jpeg|png|ttf|svg|json)\?{0,}.*", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        //    if (reg.IsMatch(request.Url))
        //    {
        //        return true;
        //    }

        //    if (request.Url.Contains("img.alicdn.com"))
        //    {
        //        return true;
        //    }
        //    if (request.Url.Contains("qrlogin.taobao.com"))
        //    {
        //        return true;
        //    }
        //    if (request.Url.Contains("tanx.com"))
        //    {
        //        return true;
        //    }
        //    if (request.Url.Contains("wwc.alicdn.com"))
        //    {
        //        return true;
        //    }
        //    if (request.Url.Contains("wwc.alicdn.com"))
        //    {
        //        return true;
        //    }




        //    return false;
        //}

        protected override CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            return null;
            //return base.GetResourceHandler(browser, frame, request);
        }

        protected override void OnPluginCrashed(CefBrowser browser, string pluginPath)
        {
            //_core.InvokeIfRequired(() => _core.OnPluginCrashed(new PluginCrashedEventArgs(pluginPath)));
        }

        protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
            //_core.InvokeIfRequired(() => _core.OnRenderProcessTerminated(new RenderProcessTerminatedEventArgs(status)));
        }
    }

}
