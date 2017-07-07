
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Models;

namespace ShoppingWebCrawler.Host
{
    public static class GlobalContext
    {
        /// <summary>
        /// UI线程的同步上下文
        /// </summary>
        public static SynchronizationContext SyncContext;


        //public static IEnumerable<CefCookie> MainPageCooies;
        /// <summary>
        /// 默认的cef全局cookie 容器
        /// </summary>
        public static CefCookieManager DefaultCEFGlobalCookieManager
        {
            get
            {
                return CefCookieManager.GetGlobal(null);
            }
        }

        private static object _locker_SupportPlatform = new object();
        private static List<SupportPlatform> _SupportPlatforms;
        /// <summary>
        /// 支持的电商平台
        /// </summary>
        public static List<SupportPlatform> SupportPlatforms
        {
            get
            {


                //加载并返回支持平台的配置  并监视配置文件
                if (null == _SupportPlatforms || _SupportPlatforms.Count <= 0)
                {
                    lock (_locker_SupportPlatform)
                    {

                        var allPlatforms=SupportPlatform.LoadConfig();
                        _SupportPlatforms = allPlatforms;
                        SupportPlatform.MonitorConfigFile((s, e) =>
                        {
                            if (null == e)
                            {
                                return;
                            }
                            //不管有没有 都要清除掉
                            _SupportPlatforms.Clear();
                            _SupportPlatforms = null;
                            if (null == e.CurrentSupportPlatforms || e.CurrentSupportPlatforms.Count <= 0)
                            {
                                return;
                            }
                            _SupportPlatforms=e.CurrentSupportPlatforms;
                        });

                    }
                }

                return _SupportPlatforms;

            }
        }


        /// <summary>
        /// 处理域名下的Cookies
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="callBackHandler"></param>
        public static void OnInvokeProcessDomainCookies(string domainName, Action<IEnumerable<CefCookie>> callBackHandler)
        {


            //设定其存储Cookie的路径
            var ckManager = DefaultCEFGlobalCookieManager;
            var vistor = new LazyCookieVistor();
            vistor.VistCookiesCompleted += (object sender, CookieVistCompletedEventAgrs e) =>
            {



                //使用基于 程序UI线程同步上下文的 ，进行消息同步
                GlobalContext.SyncContext.Post((object agrs) =>
                {

                    ////获取当前域名下的Cookies 
                    var currentDomainCookies = (agrs as CookieVistCompletedEventAgrs).Results;

                    //设定全局的Cookie集合
                    ///GlobalContext.MainPageCooies = currentDomainCookies;



                    if (null != callBackHandler)
                    {
                        callBackHandler(currentDomainCookies);
                    }

                }, e);


            };
            //使用基于Vistor模式  进行当前域的Cookie遍历
            ckManager.VisitUrlCookies(domainName, true, vistor);




        }


    }
}
