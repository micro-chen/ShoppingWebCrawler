
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Common.Ioc;
using NTCPMessage.EntityPackage;
using System.Net;
using ShoppingWebCrawler.Host.Common.Caching;
using System.IO;
using ShoppingWebCrawler.Cef.Framework;

namespace ShoppingWebCrawler.Host.Common
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
                var ckManager= CefCookieManager.GetGlobal(null);
              
                //if (null==ckManager)
                //{
                //    Console.WriteLine("麻痹 的的的 的的    空cookie 管理姻缘啊  啊啊");
                //}
                return ckManager;
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
        /// 远程服务主控Master套接字端口
        /// </summary>
        public static int MasterSocketPort
        {
            get
            {
                if (_DefaultSocketPort <= 0)
                {
                    lock (_locker_SocketPort)
                    {
                        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "TCPServerConf.config");
                        _DefaultSocketPort = ConfigHelper.GetConfigFromConfigFile(configPath, "Port").ToInt();
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

        /// <summary>
        /// 是否在从节点下运行
        /// </summary>
        public static bool IsInSlaveMode
        {
            get;set;
        }
        /// <summary>
        /// 在从节点进行运行的进程是在 cef  的rende 进程中；
        /// 在 render 进程创建browser 回调事件中，获取对应的绑定browser对象
        /// </summary>
        public static CefBrowser SlaveModeCefBrowserInRenderProcess
        {
            get; set;
        }
        /// <summary>
        /// 是否开启集群模式
        /// </summary>
        public static bool IsClusteringMode
        {
            get
            {
                return ConfigHelper.GetConfigBool("ClusteringMode");
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

        #region redis 相关

        private static RedisCacheManager _RedisClient;
        /// <summary>
        /// redis 客户端
        /// </summary>
        public static RedisCacheManager RedisClient
        {
            get
            {
                if (null == _RedisClient)
                {
                    _RedisClient = RedisCacheManager.Current;
                }
                return _RedisClient;
            }
            set
            {
                _RedisClient = value;
            }
        }

        #region cookies 跨机器


        private static readonly string PrefixDeskPushToRedisCookies = "Desk.Platform.Cookies.";
        public static EventHandler<PushToRedisCookiesEventArgs> HandlerPushToRedisCookies;

        /// <summary>
        /// 推送其他平台的cookie
        /// </summary>
        /// <param name="otherPlatform"></param>
        /// <param name="cookies"></param>
        public static void DeskPushToRedisCookies(string otherPlatform, IEnumerable<CefCookie> cookies)
        {

            //键
            var key = string.Concat(PrefixDeskPushToRedisCookies, otherPlatform);
            RedisClient.SetAsync(key, cookies);
        }
        /// <summary>
        /// 从桌面版发送cookie到redis
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="cookies"></param>
        public static void DeskPushToRedisCookies(SupportPlatformEnum platform, IEnumerable<CefCookie> cookies)
        {

            //键
            var key = string.Concat(PrefixDeskPushToRedisCookies, platform.ToString());
            RedisClient.SetAsync(key, cookies);
        }


        /// <summary>
        /// 拉取桌面版发送cookie到redis
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="cookies"></param>
        public static List<CefCookie> DeskPullFromRedisCookies(string otherPlatform)
        {

            //键
            var key = string.Concat(PrefixDeskPushToRedisCookies, otherPlatform);
            var cookies = RedisClient.Get<List<CefCookie>>(key);

            return cookies;
        }
        /// <summary>
        /// 拉取桌面版发送cookie到redis
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="cookies"></param>
        public static List<CefCookie> DeskPullFromRedisCookies(SupportPlatformEnum platform)
        {

            //键
            var key = string.Concat(PrefixDeskPushToRedisCookies, platform.ToString());
            var cookies = RedisClient.Get<List<CefCookie>>(key);

            return cookies;
        }
        #endregion

        #region 优惠券相关
        private static readonly string PrefixYouHuiQuanExistsRedis = "YouHuiQuan.Exists.{0}.{1}.{2}";
        private static readonly string PrefixYouHuiQuanDetailsListRedis = "YouHuiQuan.DetailsList.{0}.{1}.{2}";

        /// <summary>
        /// 保存优惠券是否已经存在到缓存
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="sellerId"></param>
        /// <param name="itemId"></param>
        /// <param name="timeOut"></param>
        public static void SetToRedisYouHuiQuanExists(string platform, long sellerId, long itemId, int? timeOut = null)
        {
            if (null == timeOut)
            {
                timeOut = ConfigHelper.GetConfigInt("QuanCacheableTime");
                if (timeOut <= 0)
                {
                    timeOut = 30;//默认为30秒
                }
            }
            //键
            var key = string.Format(PrefixYouHuiQuanExistsRedis, platform, sellerId, itemId);
            RedisClient.SetAsync(key, 1, timeOut.Value);
        }
        /// <summary>
        /// 从缓存中检索是否优惠券存在
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="sellerId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool CheckFromRedisYouHuiQuanExists(string platform, long sellerId, long itemId)
        {
            var result = false;
            //键
            var key = string.Format(PrefixYouHuiQuanExistsRedis, platform, sellerId, itemId);
            var value = RedisClient.Get<int>(key);
            if (value > 0)
            {
                result = true;
            }
            return result;
        }


        /// <summary>
        /// 保存优惠券列表 到缓存
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="sellerId"></param>
        /// <param name="itemId"></param>
        /// <param name="timeOut"></param>
        public static void SetToRedisYouHuiQuanDetailsList(string platform, long sellerId, long itemId, List<Youhuiquan> quanList, int? timeOut = null)
        {
            if (null == timeOut)
            {
                timeOut = ConfigHelper.GetConfigInt("QuanCacheableTime");
                if (timeOut <= 0)
                {
                    timeOut = 30;//默认为30秒
                }
            }
            //键
            var key = string.Format(PrefixYouHuiQuanDetailsListRedis, platform, sellerId, itemId);
            RedisClient.SetAsync(key, quanList, timeOut.Value);
        }
        /// <summary>
        /// 从缓存加载存在的券列表
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="sellerId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static List<Youhuiquan> GetFromRedisYouHuiQuanDetailsList(string platform, long sellerId, long itemId)
        {

            //键
            var key = string.Format(PrefixYouHuiQuanDetailsListRedis, platform, sellerId, itemId);
            var value = RedisClient.Get<List<Youhuiquan>>(key);

            return value;
        }

        #endregion
        #endregion
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
        /// 淘宝联盟推广站-阿里妈妈
        /// </summary>
        public const string AlimamaSiteURL = "https://pub.alimama.com/";
        public const string TaobaoSiteURL = "https://www.taobao.com/";
        /// <summary>
        /// 爱淘宝-（原来的优惠券官网）
        /// </summary>
        public const string AiTaobaoSiteURL = "https://uland.taobao.com/new_cat.htm";
        /// <summary>
        /// 轻淘客
        /// </summary>
        public const string QingTaokeSiteURL = "http://www.qingtaoke.com/";
        public const string QingTaokeSiteName = "QingTaoke";
        //public const string QingTaokeSiteLoginURL = "http://www.qingtaoke.com/login?ref=http%253A%252F%252Fwww.qingtaoke.com%252F";

        private static string _Pid = string.Empty;
        /// <summary>
        /// 阿里妈妈推广id
        /// </summary>
        public static string Pid
        {
            get
            {
                if (string.IsNullOrEmpty(_Pid))
                {
                    string uaConfig = ConfigHelper.GetConfig("Pid");
                    _Pid = uaConfig;
                }

                return _Pid;
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
