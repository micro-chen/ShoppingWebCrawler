
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Models;
using ShoppingWebCrawler.Host.Ioc;
using NTCPMessage.EntityPackage;
using System.Net;

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

        private static string _ChromeUserAgent = string.Empty;
        /// <summary>
        /// 浏览器的UA标识
        /// </summary>
        public static string ChromeUserAgent
        {
            get
            {
                if (string.IsNullOrEmpty(_ChromeUserAgent))
                {
                    string uaConfig = ConfigHelper.GetConfig("UserAgent");
                    _ChromeUserAgent = uaConfig;
                }

                return _ChromeUserAgent;
            }
        }


        private static object _locker_SocketPort = new object();
        static int _DefaultSocketPort = 0;
        /// <summary>
        /// 远程服务套接字端口
        /// </summary>
        public static int SocketPort
        {
            get
            {
                if (_DefaultSocketPort <= 0)
                {
                    lock (_locker_SocketPort)
                    {

                        _DefaultSocketPort = ConfigHelper.GetConfigInt("Port");
                        if (_DefaultSocketPort <= 0)
                        {
                            _DefaultSocketPort = 10086;
                        }

                    }
                }

                return _DefaultSocketPort;
            }
        }

        private static List<string> _HotWords;
        /// <summary>
        /// 热搜词集合
        /// </summary>
        public static List<string> HotWords
        {
            get
            {
                if (null == _HotWords || _HotWords.Count <= 0)
                {
                    _HotWords = HotWordsLoader.LoadConfig();
                }
                return _HotWords;
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

                        var allPlatforms = SupportPlatformLoader.LoadConfig();
                        _SupportPlatforms = allPlatforms;
                        //文件变更后 通知的事件委托
                        EventHandler<SupportPlatformsChangedEventArgs> hander = null;

                        hander = (s, e) =>
                          {
                              if (null == e)
                              {
                                  return;
                              }
                              //不管有没有 都要清除掉
                              if (null != _SupportPlatforms)
                              {
                                  _SupportPlatforms.Clear();
                                  _SupportPlatforms = null;
                              }

                              //if (null == e.CurrentSupportPlatforms || e.CurrentSupportPlatforms.Count <= 0)
                              //{
                              //    return;
                              //}
                              //_SupportPlatforms = e.CurrentSupportPlatforms;

                              ////刷新完毕后 从新进入下个监听
                              //SupportPlatformLoader.MonitorConfigFile(hander);
                          };
                        SupportPlatformLoader.MonitorConfigFile(hander);

                    }
                }

                return _SupportPlatforms;

            }
        }

        /// <summary>
        /// 所有平台的 cookie 字典容器，按照网址对Cookie进行了key区分
        /// </summary>
        public static IDictionary<string, List<Cookie>> SupportPlatformsCookiesContainer
        {
            get
            {
                return SingletonDictionary<string, List<Cookie>>.Instance;
            }
        }


        /// <summary>
        /// 处理域名下的Cookies
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="callBackHandler"></param>
        [Obsolete("this method has been obsolete. and recomend use LazyCookieVisitor.LoadCookies()")]
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
