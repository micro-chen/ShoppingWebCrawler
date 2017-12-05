using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NTCPMessage.EntityPackage.Arguments;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.CookiePender;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Common.Http;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Cef.Core;
using System.Threading;
using ShoppingWebCrawler.Host.Model;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 
    /// todo:代码封存：先忙工作
    /// todo:查询单个产品的券 需要的时间不理想
    /// 阿里妈妈 网页授权自动刷新管理类
    /// 远程监听 UI登录程序端口，获取登录的Cookie授权
    /// 刷新到无头浏览器中进行定时刷新保持登录状态
    /// </summary>
    public class AlimamaWebPageService : BaseWebPageService
    {

        /// 阿里妈妈主站地址
        private const string AlimamaSiteUrl = GlobalContext.AlimamaSiteURL;



        private static AlimamaCookiePenderClient cookiePender_alimama;


        private static System.Timers.Timer _timer_refresh_login_cookie;

        /// <summary>
        /// 是否已经登录阿里妈妈
        /// </summary>
        public static bool IsHasLoginAlimama = false;
        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return AlimamaMixReuestLoader.Current;
            }
        }


        /// <summary>
        /// 静态构造函数
        /// 在构建的时候 进行一次登录初始化
        /// </summary>
        static AlimamaWebPageService()
        {
            BeginTryToLogin();
        }


        /// <summary>
        /// 尝试登录
        /// </summary>
        private static void BeginTryToLogin()
        {
            if (null != _timer_refresh_login_cookie)
            {
                //有定时任务进行监听的时候 不要重复定时监听
                return;
            }
            cookiePender_alimama = new AlimamaCookiePenderClient();
            //-----------首先尝试登录一次，登录不成功，那么进入定时任务中----------
            //表示已经登录 那么刷新登录Cookie
            var cks_alimama = cookiePender_alimama.GetCookiesFromRemoteServer();
            if (null != cks_alimama && cks_alimama.FirstOrDefault(x => x.Name == "login") != null)
            {
                //表示已经登录 那么刷新登录Cookie
                SetLogin(cks_alimama);

            }
            else
            {

                //开启定时任务刷新登录阿里妈妈Cookie
                _timer_refresh_login_cookie = new System.Timers.Timer(5000);
                _timer_refresh_login_cookie.Elapsed += (s, e) =>
                {
                    cks_alimama = cookiePender_alimama.GetCookiesFromRemoteServer();
                    if (null != cks_alimama && cks_alimama.FirstOrDefault(x => x.Name == "login") != null)
                    {
                        //表示已经登录 那么刷新登录Cookie
                        SetLogin(cks_alimama);
                        //一旦登录成功不再定时从远程获取，后续让自身无头浏览器 刷新登录Cookie
                        _timer_refresh_login_cookie.Stop();
                        _timer_refresh_login_cookie.Dispose();
                        _timer_refresh_login_cookie = null;
                    }
                };
                _timer_refresh_login_cookie.Start();
            }
        }


        /// <summary>
        /// 根据传递的查询参数 {卖家id  商品id}列表 批量查询优惠券是否存在信息
        /// 返回抓取后的内容
        /// </summary>
        /// <param name="queryParas"></param>
        /// <returns></returns>
        public DataContainer QueryYouhuiquanExistsList(YouhuiquanFetchWebPageArgument queryParas)
        {
            DataContainer container = new DataContainer();

            if (null == queryParas)
            {
                return container;
            }
            if (IsHasLoginAlimama == false)
            {
                string msg = "未能正确登录阿里妈妈，不能查询优惠券！" + DateTime.Now.ToString();
                container.Result = msg;
                Logger.Info(msg);
                return container;
            }
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //查询所有券集合
            var allQuanList = AlimamaMixReuestLoader.Current.GetAllCouponExistsList(queryParas).Result;

            sw.Stop();
            Console.WriteLine("GetAllCoupon Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);
            container.Result = JsonConvert.SerializeObject(allQuanList);
            return container;
        }

        /// <summary>
        /// 根据传递的查询参数 {卖家id  商品id}列表 批量查询优惠券信息
        /// 返回抓取后的内容
        /// </summary>
        /// <param name="queryParas"></param>
        /// <returns></returns>
        public DataContainer QueryYouhuiquan(YouhuiquanFetchWebPageArgument queryParas)
        {
            DataContainer container = new DataContainer();

            if (null == queryParas)
            {
                return container;
            }
            if (IsHasLoginAlimama == false)
            {
                string msg = "未能正确登录阿里妈妈，不能查询优惠券！" + DateTime.Now.ToString();
                container.Result = msg;
                Logger.Info(msg);
                return container;
            }
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //查询所有券集合
            var allQuanList = AlimamaMixReuestLoader.Current.GetAllCoupon(queryParas).Result;

            sw.Stop();
            Console.WriteLine("GetAllCoupon Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);
            container.Result = JsonConvert.SerializeObject(allQuanList);
            return container;
        }


        /// <summary>
        /// 静态登录Cookie注册
        /// </summary>
        /// <param name="loginCookieCollection">需要提供的已经登录的Cookie集合</param>
        private static void SetLogin(List<CefCookie> loginCookieCollection)
        {
            if (null != loginCookieCollection)
            {
                //注册cookie集合到全局Cookie容器内
                new LazyCookieVistor().SetCookieToCookieManager(AlimamaSiteUrl, loginCookieCollection);


                IsHasLoginAlimama = true;
            }




        }

        /// <summary>
        /// 强制从新登录
        /// </summary>
        public static bool ForceLogin()
        {
            bool success = false;
            var cks = cookiePender_alimama.GetCookiesFromRemoteServer();
            if (null != cks && cks.FirstOrDefault(x => x.Name == "login") != null)
            {
                //表示已经登录 那么刷新登录Cookie
                SetLogin(cks);

                success = true;
            }

            return success;
        }






        ///------------内部类-----------------

        /// <summary>
        /// 阿里妈妈的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class AlimamaMixReuestLoader : BaseBrowserRequestLoader<AlimamaMixReuestLoader>
        {


            private bool isQuanCacheable = ConfigHelper.GetConfigBool("QuanCacheable");


            /// <summary>
            /// 阿里妈妈请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "http://pub.alimama.com/items/search.json?q={0}&_t={1}&toPage=1&perPageSize=40&auctionTag=&shopTag=yxjh&t={1}&_tb_token_={2}&pvid=10_123.127.46.142_367_1501208864509";

            /// <summary>
            /// 隐藏 券API
            /// </summary>
            private const string hidenQuanAPI = "http://www.qingtaoke.com/api/UserPlan/UserCouponList?sid={0}&gid={1}";

            /// <summary>
            /// 查询商家价格阶梯优惠活动
            /// </summary>
            private const string getPriceVolumeAPI = "https://cart.taobao.com/json/GetPriceVolume.do?sellerId={0}";


            /// <summary>
            /// 淘宝券详细页面
            /// demo:https://uland.taobao.com/coupon/edetail?itemId=528837368980&activityId=343ad6fd06a1421dbe65e67752f12824
            /// </summary>
            private const string taobaoQuanDetailPageUrl = "https://uland.taobao.com/coupon/edetail?itemId={0}&activityId={1}";

            /// <summary>
            /// 淘宝券详细json数据url
            /// demo:https://uland.taobao.com/cp/coupon?ctoken=1bLiFi7H4Radgy7V8Usbiceland&itemId=537099364188&activityId=f2113053fa0a4fe98f5faee6584426cc
            /// </summary>
            private const string taobaoQuanDetailJsonUrl = "https://uland.taobao.com/cp/coupon?ctoken={0}&itemId={1}&activityId={2}";


            /// <summary>
            /// 淘宝券隐藏券领取地址
            /// </summary>
            //private const string taobaoQuanHidenQuanGetToClickUrl = "https://taoquan.taobao.com/coupon/unify_apply.htm?activityId={0}&sellerId={1}";
            private const string taobaoQuanLingQuanGetToClickUrl = "https://uland.taobao.com/coupon/edetail?activityId={0}&itemId={1}&pid={2}&src=huidangso";

            /// <summary>
            /// 淘鹊桥活动券查询
            /// 参数是itemId
            /// </summary>

            private const string taoqueqiaoQuanGetUrl = "http://vip.taoqueqiao.com/?mod=inc&act=plugin&do=quan&iid={0}";

            /// <summary>
            /// 阿里妈妈查询券1
            /// 根据商品链接查询--不管是淘宝还是天猫 阿里妈妈会自动转为对应的平台 根据itemid
            /// var url='https://detail.tmall.com/item.htm?id='+item_id;
            /// </summary>
            private const string alimamaQuanGetUrl = "http://pub.alimama.com/items/search.json?q={0}&_t={1}&perPageSize=40&shopTag=&t={1}&_tb_token_={2}&pvid=10_180.77.135.254_9822_1501394606709&yxjh=-1";

            /// <summary>
            /// 获取我的商品定向推广链接
            /// </summary>
            private const string alimamaMyTuiGuangQuanUrl = "http://pub.alimama.com/common/code/getAuctionCode.json?auctionid={0}&adzoneid=21932422&siteid=6394774&scenes=1&t={1}&_tb_token_={2}&pvid={3}";
            /// <summary>
            /// 阿里妈妈-请求客户端
            /// </summary>
            private static CookedHttpClient alimamaHttpClient;

            /// <summary>
            /// 轻淘客-请求客户端
            /// </summary>
            private static CookedHttpClient qingTaoKeHttpClient;


            /// <summary>
            /// 淘宝券-请求客户端
            /// </summary>
            private static CookedHttpClient taoquanHttpClient;

            /// <summary>
            /// 淘宝-请求客户端
            /// </summary>
            private static CookedHttpClient taobaoHttpClient;

            /// <summary>
            /// 淘鹊桥-请求客户端
            /// </summary>
            private static CookedHttpClient taoqueqiaoHttpClient;


            /// <summary>
            /// 静态构造函数
            /// </summary>
            static AlimamaMixReuestLoader()
            {
                //静态创建请求客户端
                //alimamaHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化-阿里妈妈头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "pub.alimama.com");
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                //requestHeaders.Add("Referer", alimamaSiteUrl);
                alimamaHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(alimamaHttpClient.Client.DefaultRequestHeaders, requestHeaders);

                //初始化-轻淘客头信息
                requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "www.qingtaoke.com");
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                //requestHeaders.Add("versionCode", "42");
                qingTaoKeHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(qingTaoKeHttpClient.Client.DefaultRequestHeaders, requestHeaders);

                //初始化淘宝券头信息
                requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "uland.taobao.com");
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                requestHeaders.Add("pragma", "no-cache");
                taoquanHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(taoquanHttpClient.Client.DefaultRequestHeaders, requestHeaders);

                //初始化淘宝-价格阶梯券头信息
                requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                //requestHeaders.Add("Referer", TaobaoWebPageService.TaobaoMixReuestLoader.TaobaoSiteUrl);
                requestHeaders.Add("Host", "cart.taobao.com");
                taobaoHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(taobaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);


                //初始化淘鹊桥-价格阶梯券头信息
                requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("upgrade-insecure-requests", "1");
                requestHeaders.Add("Host", "vip.taoqueqiao.com");
                taoqueqiaoHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(taoqueqiaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);

            }

            /// <summary>
            /// 默认构造无登录Cookie的构造函数
            /// 会跳转到登录页面
            /// </summary>
            public AlimamaMixReuestLoader()
            {

                ///阿里妈妈刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate = "http://pub.alimama.com/myunion.htm?spm=a219t.7900221/10.a214tr8.2.77522c2apY61Kb";

                this.IntiCefWebBrowser();

            }

            ///// <summary>
            ///// 登录Cookie注册
            ///// </summary>
            ///// <param name="loginCookieCollection">需要提供的已经登录的Cookie集合</param>
            //public override void SetLogin(List<CefCookie> loginCookieCollection)
            //{
            //    if (null != loginCookieCollection)
            //    {
            //        //注册cookie集合到全局Cookie容器内
            //        new LazyCookieVistor().RegisterCookieToCookieManager(AlimamaSiteUrl, loginCookieCollection);
            //    }

            //    this.AutoRefeshCookie(this.RefreshCookieUrlTemplate);
            //}



            #region 优惠券存在列表


            /// <summary>
            /// 查询商品的优惠券是否存在列表信息
            /// </summary>
            /// <param name="queryParas"></param>
            /// <returns></returns>
            public async Task<IEnumerable<YouhuiquanExistsModel>> GetAllCouponExistsList(YouhuiquanFetchWebPageArgument queryParas)
            {

                string ctoken = string.Empty;
                var cks = new LazyCookieVistor().LoadCookies(TaoUlandWebPageServic.TaobaoQuanDomain);

                //加载完毕后 刷新下 token 
                //模拟导航到券列表页面 - 用来获取优惠券的Cookie - ctoken
                if (null == cks)
                {
                    string msg = "未能正获取阿里妈妈券的cookie 集合，不能查询优惠券！" + DateTime.Now.ToString();
                    Logger.Info(msg);
                    return null;

                }
                else
                {
                    var ctokenCookie = cks.FirstOrDefault(x => x.Name == "ctoken");
                    if (null == ctokenCookie || string.IsNullOrEmpty(ctokenCookie.Value))
                    {
                        string msg = "未能正获取阿里妈妈券的ctoken，不能查询优惠券！" + DateTime.Now.ToString();
                        Logger.Info(msg);
                        return null;
                    }

                    ctoken = ctokenCookie.Value;
                }


                //-----------让两个任务并行，等待并行运算结果--------------
                var tskAllQuanExistsList = await Task.Factory.StartNew(() =>
                {
                    var dataHashTable = new System.Collections.Concurrent.ConcurrentBag<YouhuiquanExistsTaskBuffer>();

                    foreach (var paraItem in queryParas.ArgumentsForExistsList)
                    {

                        var taskModel = new YouhuiquanExistsTaskBuffer(paraItem.SellerId, paraItem.ItemId);
                        bool isHasExistAtCache = false;
                        if (isQuanCacheable)
                        { //如果开启了缓存 那么先去缓存中查询指定键
                            isHasExistAtCache = GlobalContext.CheckFromRedisYouHuiQuanExists(SupportPlatformEnum.Alimama.ToString(), paraItem.SellerId, paraItem.ItemId);
                            if (isHasExistAtCache)
                            {
                                taskModel.ResultModel = new YouhuiquanExistsModel { SellerId = paraItem.SellerId, ItemId = paraItem.ItemId, IsExistsQuan = true };
                                dataHashTable.Add(taskModel);
                                continue;//从缓存中的 不用再后面动态查询
                            }
                        }



                        //1 隐藏券查询
                        // var tskAlimamaHiddenQuanActivity = this.QueryHideQuanActivitysExistsListAsync(paraItem.SellerId, paraItem.ItemId);
                        taskModel.TaskBufferQueue.Enqueue(this.QueryHideQuanActivitysExistsListAsync);

                        //2 首先查询商家设置的价格阶梯 如：https://cart.taobao.com/json/GetPriceVolume.do?sellerId=2649797694
                        /*
                        {"priceVolumes":[{"condition":"满488减30","id":"e9c303182542418589c1a3ead872acd1","price":"30","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满488领劵立减30","type":"youhuijuan"},{"condition":"满188减20","id":"d9060db139fd4c9bac375e474632e485","price":"20","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满188领劵立减20","type":"youhuijuan"},{"condition":"满88减10","id":"2f9cc940cc0f4f8197e7e0f6dee45087","price":"10","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满88领劵立减10元","type":"youhuijuan"},{"condition":"满45减5","id":"fbe4efe09a824b7dbc4b83e00d6adb77","price":"5","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.21-2017.12.31","title":"买1立减5元","type":"youhuijuan"}],"receivedCount":0,"unreceivedCount":4}
                        */

                        taskModel.TaskBufferQueue.Enqueue(this.QueryPriceVolumeQuanActivitysExistsListAsync);
                        //  3 查询商家在合作平台-淘鹊桥-上发起的活动
                        taskModel.TaskBufferQueue.Enqueue(this.QueryTaoQueQiaoQuanActivitysExistListAsync);
                        //4 查询商家在淘宝联盟上的活动-没有在淘鹊桥上活动
                        taskModel.TaskBufferQueue.Enqueue(this.QueryMamaQuanActivitysExistListAsync);

                        //开始发起查询任务
                        taskModel.BeginQueryTaskQueue();

                        dataHashTable.Add(taskModel);
                    }

                    //需要等待任务并行完毕 
                    try
                    {
                        var allDynamicTasks = dataHashTable.Where(x => x.QueryTaskEndPoint != null);
                        if (allDynamicTasks.IsNotEmpty())
                        {
                            var executeTasks = allDynamicTasks.Select(x => x.QueryTaskEndPoint);
                            Task.WaitAll(executeTasks.ToArray());//一旦有执行动态查询的task 那么等待动态查询完毕
                            //完毕后 将执行动态查询的对象  进行插入到缓存
                            foreach (var item in allDynamicTasks)
                            {
                                if (item.ResultModel != null && item.ResultModel.IsExistsQuan == true)
                                {
                                    GlobalContext.SetToRedisYouHuiQuanExists(SupportPlatformEnum.Alimama.ToString(), item.ResultModel.SellerId, item.ResultModel.ItemId);
                                }
                            }
                        }


                    }
                    catch (AggregateException aex)
                    {
                        //取消任务不做处理
                    }




                    return dataHashTable.Select(x => x.ResultModel);
                });

                return tskAllQuanExistsList;
            }



            #endregion

            #region 优惠券详细

            /// <summary>
            /// 获取商品的所有优惠券集合
            /// </summary>
            /// <param name="queryParas"></param>
            /// <returns></returns>
            public async Task<List<Youhuiquan>> GetAllCoupon(YouhuiquanFetchWebPageArgument queryParas)
            {

                string ctoken = string.Empty;
                var cks = new LazyCookieVistor().LoadCookies(TaoUlandWebPageServic.TaobaoQuanDomain);

                //加载完毕后 刷新下 token 
                //模拟导航到券列表页面 - 用来获取优惠券的Cookie - ctoken
                if (null == cks)
                {
                    string msg = "未能正获取阿里妈妈券的cookie 集合，不能查询优惠券！" + DateTime.Now.ToString();
                    Logger.Info(msg);
                    return null;

                }
                else
                {
                    var ctokenCookie = cks.FirstOrDefault(x => x.Name == "ctoken");
                    if (null == ctokenCookie || string.IsNullOrEmpty(ctokenCookie.Value))
                    {
                        string msg = "未能正获取阿里妈妈券的ctoken，不能查询优惠券！" + DateTime.Now.ToString();
                        Logger.Info(msg);
                        return null;
                    }

                    ctoken = ctokenCookie.Value;
                }

                bool isHasExistAtCache = false;
                if (isQuanCacheable)
                { //如果开启了缓存 那么先去缓存中查询指定键 是否存在
                    var lstAtCache = GlobalContext.GetFromRedisYouHuiQuanDetailsList(SupportPlatformEnum.Alimama.ToString(), queryParas.ArgumentsForQuanDetails.SellerId, queryParas.ArgumentsForQuanDetails.ItemId);
                    if (null != lstAtCache)
                    {
                        isHasExistAtCache = true;
                        return lstAtCache;
                    }
                }

                //-----------如果未存在缓存中，让两个任务并行，等待并行运算结果--------------

                var tskAllQuan = await Task.Factory.StartNew(() =>
                  {
                      var dataList = new List<Youhuiquan>();

                      //普通券
                      var tskCommonQuan = this.GetCommonCoupon(queryParas, ctoken);

                      //隐藏券
                      var tskHiddenQuan = this.GetHiddenCouponAsync(queryParas, ctoken);


                      if (null != tskCommonQuan.Result)
                      {
                          dataList.AddRange(tskCommonQuan.Result);
                      }
                      if (null != tskHiddenQuan.Result)
                      {
                          dataList.AddRange(tskHiddenQuan.Result);
                      }
                      if (isQuanCacheable == true)
                      {
                          //插入到缓存中
                          GlobalContext.SetToRedisYouHuiQuanDetailsList(SupportPlatformEnum.Alimama.ToString(), queryParas.ArgumentsForQuanDetails.SellerId, queryParas.ArgumentsForQuanDetails.ItemId, dataList);

                      }
                      return dataList;
                  });

                return tskAllQuan;
            }

            #region 查询普通优惠券

            /// <summary>
            /// 根据查询券参数 
            /// 查询普通优惠券集合
            /// 1 促销券--价格活动阶梯-满减
            /// 2 商家设置的一般优惠券-推广券-明券-在淘鹊桥上的活动
            /// 3 没有在淘鹊桥上 ，但是是阿里妈妈的券，妈妈券
            /// 上家推广券一般只有一个，是淘宝联盟官方活动券，可以从淘鹊桥拿下来 活动的id
            /// </summary>
            /// <param name="queryParas"></param>
            /// <param name="ctoken"></param>
            /// <returns></returns>
            public Task<IEnumerable<Youhuiquan>> GetCommonCoupon(YouhuiquanFetchWebPageArgument queryParas, string ctoken)
            {
                if (null == queryParas || queryParas.ArgumentsForQuanDetails == null)
                {
                    return null;
                }
                var paraItem = queryParas.ArgumentsForQuanDetails;
                //查询出所有的商品的非隐藏-（非特定网站合作的优惠活动）优惠券
                ConcurrentBag<Youhuiquan> dataListHashTable = new ConcurrentBag<Youhuiquan>();

                var tskQueryAllCommonQuanList = Task.Factory.StartNew<IEnumerable<Youhuiquan>>(() =>
                {


                    //1 首先查询商家设置的价格阶梯 如：https://cart.taobao.com/json/GetPriceVolume.do?sellerId=2649797694
                    /*
                    {"priceVolumes":[{"condition":"满488减30","id":"e9c303182542418589c1a3ead872acd1","price":"30","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满488领劵立减30","type":"youhuijuan"},{"condition":"满188减20","id":"d9060db139fd4c9bac375e474632e485","price":"20","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满188领劵立减20","type":"youhuijuan"},{"condition":"满88减10","id":"2f9cc940cc0f4f8197e7e0f6dee45087","price":"10","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满88领劵立减10元","type":"youhuijuan"},{"condition":"满45减5","id":"fbe4efe09a824b7dbc4b83e00d6adb77","price":"5","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.21-2017.12.31","title":"买1立减5元","type":"youhuijuan"}],"receivedCount":0,"unreceivedCount":4}
                    */
                    var tskVolumeQuanActivity = this.QueryPriceVolumeQuanActivitysAsync(paraItem.SellerId, paraItem.ItemId);

                    // 2 查询商家在合作平台-淘鹊桥-上发起的活动
                    var tskTaoqueqiaoQuanActivity = this.QueryTaoQueQiaoQuanActivitysAsync(paraItem.SellerId, paraItem.ItemId);

                    //3 查询商家在淘宝联盟上的活动-没有在淘鹊桥上活动
                    var tskAlimamaQuanActivity = this.QueryMamaQuanActivitysAsync(paraItem.SellerId, paraItem.ItemId);

                    //等待并行完毕
                    //Task.WaitAll(tskVolumeQuanActivity, tskTaoqueqiaoQuanActivity, tskAlimamaQuanActivity);
                    if (null != tskVolumeQuanActivity.Result)
                    {
                        var cursor = tskVolumeQuanActivity.Result.GetEnumerator();
                        while (cursor.MoveNext())
                        {
                            dataListHashTable.Add(cursor.Current);
                        }
                    }
                    if (null != tskTaoqueqiaoQuanActivity.Result)
                    {
                        dataListHashTable.Add(tskTaoqueqiaoQuanActivity.Result);
                    }
                    if (null != tskAlimamaQuanActivity.Result)
                    {
                        dataListHashTable.Add(tskAlimamaQuanActivity.Result);
                    }

                    return dataListHashTable;
                });



                return tskQueryAllCommonQuanList;

            }

            /// <summary>
            /// 查询阶梯价格活动优惠券是否存在
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <param name="funHandler"></param>
            /// <returns></returns>
            private Task<bool> QueryPriceVolumeQuanActivitysExistsListAsync(long sellerId, long itemId, QueryQuanCompleteTaskHandler funHandler)
            {
                var taskQuery = Task.Factory.StartNew<bool>(() =>
                {
                    bool result = false;
                    if (sellerId <= 0 || itemId <= 0)
                    {
                        return result;
                    }
                    //查询隐藏券的地址
                    string queryAddress = string.Format(getPriceVolumeAPI, sellerId);

                    try
                    {


                        var cks = new LazyCookieVistor().LoadCookies(TaobaoWebPageService.TaobaoMixReuestLoader.TaobaoSiteUrl);
                        taobaoHttpClient.ChangeGlobleCookies(cks, TaobaoWebPageService.TaobaoMixReuestLoader.TaobaoSiteUrl);

                        var clientProxy = new HttpServerProxy() { Client = taobaoHttpClient.Client, KeepAlive = true };
                        var resp = clientProxy.GetResponseTransferAsync(queryAddress, null).Result;
                        if (null == resp || resp.Content == null)
                        {
                            return result;
                        }
                        //异步读取内容字符串
                        //demo:{"priceVolumes":[{"condition":"满488减30","id":"e9c303182542418589c1a3ead872acd1","price":"30","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满488领劵立减30","type":"youhuijuan"},{"condition":"满188减20","id":"d9060db139fd4c9bac375e474632e485","price":"20","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满188领劵立减20","type":"youhuijuan"},{"condition":"满88减10","id":"2f9cc940cc0f4f8197e7e0f6dee45087","price":"10","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满88领劵立减10元","type":"youhuijuan"},{"condition":"满45减5","id":"fbe4efe09a824b7dbc4b83e00d6adb77","price":"5","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.21-2017.12.31","title":"买1立减5元","type":"youhuijuan"}],"receivedCount":0,"unreceivedCount":4}
                        string respContent = resp.Content.ReadAsStringAsync().Result;
                        if (string.IsNullOrEmpty(respContent))
                        {
                            return result;
                        }


                        if (respContent.IndexOf("<html>") > 0)
                        {
                            //TODO:发送邮件通知 登录淘宝失效了 ，因为正常返回的是价格阶梯的json
                            return result;
                        }
                        PriceVolumesResult dataList = JsonConvert.DeserializeObject<PriceVolumesResult>(respContent);
                        if (null == dataList || dataList.priceVolumes == null || dataList.priceVolumes.Count <= 0)
                        {
                            result = false;
                        }
                        else
                        {
                            result = true;
                        }

                    }
                    catch (AggregateException aex)
                    {
                        //任务取消不做处理
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("-------查询价格阶梯失败！----原因异常如下：-----");
                        Logger.Info(string.Format("sellerId:{0},itemId:{1}", sellerId, itemId));
                        Logger.Error(ex);
                    }
                    return result;

                });

                //任务完毕后注册的子任务；用来注册任务完毕后的后续事件
                taskQuery.ContinueWith((tsk) =>
                {
                    if (null != funHandler)
                    {
                        funHandler(tsk, taskQuery);
                    }
                });

                return taskQuery;//将父亲源任务弹出


            }

            /// <summary>
            /// 查询价格阶梯的优惠活动
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <returns></returns>
            private async Task<IEnumerable<Youhuiquan>> QueryPriceVolumeQuanActivitysAsync(long sellerId, long itemId)
            {

                if (sellerId <= 0 || itemId <= 0)
                {
                    return null;
                }
                //查询隐藏券的地址
                string queryAddress = string.Format(getPriceVolumeAPI, sellerId);

                try
                {


                    var cks = new LazyCookieVistor().LoadCookies(TaobaoWebPageService.TaobaoMixReuestLoader.TaobaoSiteUrl);
                    taobaoHttpClient.ChangeGlobleCookies(cks, TaobaoWebPageService.TaobaoMixReuestLoader.TaobaoSiteUrl);

                    var clientProxy = new HttpServerProxy() { Client = taobaoHttpClient.Client, KeepAlive = true };
                    var resp = await clientProxy.GetResponseTransferAsync(queryAddress, null);
                    if (null == resp || resp.Content == null)
                    {
                        return null;
                    }
                    //异步读取内容字符串
                    //demo:{"priceVolumes":[{"condition":"满488减30","id":"e9c303182542418589c1a3ead872acd1","price":"30","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满488领劵立减30","type":"youhuijuan"},{"condition":"满188减20","id":"d9060db139fd4c9bac375e474632e485","price":"20","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满188领劵立减20","type":"youhuijuan"},{"condition":"满88减10","id":"2f9cc940cc0f4f8197e7e0f6dee45087","price":"10","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满88领劵立减10元","type":"youhuijuan"},{"condition":"满45减5","id":"fbe4efe09a824b7dbc4b83e00d6adb77","price":"5","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.21-2017.12.31","title":"买1立减5元","type":"youhuijuan"}],"receivedCount":0,"unreceivedCount":4}
                    string respContent = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(respContent))
                    {
                        return null;
                    }

                    //异步任务字符串数据返回
                    var tskResult = Task.Factory.StartNew(() =>
                    {
                        if (respContent.IndexOf("<html>") > 0)
                        {
                            //TODO:发送邮件通知 登录淘宝失效了 ，因为正常返回的是价格阶梯的json
                            return null;
                        }
                        PriceVolumesResult dataList = JsonConvert.DeserializeObject<PriceVolumesResult>(respContent);
                        if (null == dataList || dataList.priceVolumes == null || dataList.priceVolumes.Count <= 0)
                        {
                            return null;
                        }

                        //解析价格阶梯中的内容 直接转为优惠券信息
                        List<Youhuiquan> lstCommonQuanListPriceVolume = new List<Youhuiquan>();
                        foreach (PriceVolumesResult.PriceVolumeItem volumeItem in dataList.priceVolumes)
                        {
                            if (string.IsNullOrEmpty(volumeItem.price))
                            {
                                continue;
                            }


                            var modelQuan = this.ConvertPriceVolumeInfoToYouhuiquan(volumeItem, itemId);
                            if (null != modelQuan)
                            {
                                lstCommonQuanListPriceVolume.Add(modelQuan);
                            }
                        }

                        return lstCommonQuanListPriceVolume;
                    });

                    return tskResult.Result;
                }
                catch (Exception ex)
                {
                    Logger.Info("-------查询价格阶梯失败！----原因异常如下：-----");
                    Logger.Info(string.Format("sellerId:{0},itemId:{1}", sellerId, itemId));
                    Logger.Error(ex);
                }

                return null;
            }

            /// <summary>
            /// 查询是否在淘鹊桥有隐藏的活动优惠券
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <param name="funHandler"></param>
            /// <returns></returns>
            private Task<bool> QueryTaoQueQiaoQuanActivitysExistListAsync(long sellerId, long itemId, QueryQuanCompleteTaskHandler funHandler)
            {
                var taskQuery = Task.Factory.StartNew<bool>(() =>
                {
                    bool result = false;
                    if (sellerId <= 0 || itemId <= 0)
                    {
                        return result;
                    }
                    //查询隐藏券的地址
                    string queryAddress = string.Format(taoqueqiaoQuanGetUrl, itemId);


                    try
                    {


                        var clientProxy = new HttpServerProxy() { Client = taoqueqiaoHttpClient.Client, KeepAlive = true };
                        var resp = clientProxy.GetResponseTransferAsync(queryAddress, null).Result;
                        if (null == resp || resp.Content == null)
                        {
                            return result;
                        }
                        //异步读取内容字符串
                        //demo:{"s":1,"r":"788d960caead44998d852e251f6e934f"}
                        string respContent = resp.Content.ReadAsStringAsync().Result;
                        if (string.IsNullOrEmpty(respContent))
                        {
                            return result;
                        }

                        //异步任务字符串数据返回

                        Dictionary<string, string> queQiaoResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(respContent);
                        if (null == queQiaoResult || !queQiaoResult.ContainsKey("s"))
                        {
                            return result;
                        }
                        string activityId = string.Empty;
                        queQiaoResult.TryGetValue("r", out activityId);
                        if (string.IsNullOrEmpty(activityId))
                        {
                            return result;
                        }

                        /*
                        demo {"s":1,"r":"521a48814b9b40d9a11df136c01e29c1"}
                        */
                        string sValue = string.Empty;
                        string rValue = string.Empty;
                        if (queQiaoResult.TryGetValue("s", out sValue) && sValue.ToString().Equals("1"))
                        {
                            if (queQiaoResult.TryGetValue("r", out rValue) && rValue.Length == 32)
                            {
                                result = true;//32位字符串哈希编码
                            }
                        }


                    }
                    catch (AggregateException aex)
                    {
                        //任务取消不做处理
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("-------查询淘鹊桥活动券失败！----原因异常如下：-----");
                        Logger.Info(string.Format("sellerId:{0},itemId:{1}", sellerId, itemId));
                        Logger.Error(ex);
                    }

                    return result;

                });

                //任务完毕后注册的子任务；用来注册任务完毕后的后续事件
                taskQuery.ContinueWith((tsk) =>
                {
                    if (null != funHandler)
                    {
                        funHandler(tsk, taskQuery);
                    }
                });

                return taskQuery;//将父亲源任务弹出
            }

            /// <summary>
            /// 查询淘鹊桥的活动券
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <returns></returns>
            private async Task<Youhuiquan> QueryTaoQueQiaoQuanActivitysAsync(long sellerId, long itemId)
            {

                if (sellerId <= 0 || itemId <= 0)
                {
                    return null;
                }
                //查询隐藏券的地址
                string queryAddress = string.Format(taoqueqiaoQuanGetUrl, itemId);


                try
                {


                    var clientProxy = new HttpServerProxy() { Client = taoqueqiaoHttpClient.Client, KeepAlive = true };
                    var resp = await clientProxy.GetResponseTransferAsync(queryAddress, null);
                    if (null == resp || resp.Content == null)
                    {
                        return null;
                    }
                    //异步读取内容字符串
                    //demo:{"s":1,"r":"788d960caead44998d852e251f6e934f"}
                    string respContent = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(respContent))
                    {
                        return null;
                    }

                    //异步任务字符串数据返回
                    var tskResult = Task.Factory.StartNew(() =>
                    {
                        Dictionary<string, string> queQiaoResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(respContent);
                        if (null == queQiaoResult || !queQiaoResult.ContainsKey("s"))
                        {
                            return null;
                        }
                        string activityId = string.Empty;
                        queQiaoResult.TryGetValue("r", out activityId);
                        if (string.IsNullOrEmpty(activityId))
                        {
                            return null;
                        }

                        //获取淘宝券的Cookie集合
                        var cacheCookies = new LazyCookieVistor().LoadCookies(TaoUlandWebPageServic.TaobaoQuanDomain);
                        Cookie[] arry_cookies = null;
                        var ctokenCookie = cacheCookies.FirstOrDefault(x => x.Name == "ctoken");
                        string ctoken = string.Empty;
                        //刷新最新token
                        if (null != cacheCookies)
                        {
                            arry_cookies = cacheCookies.ToArray();
                            if (!string.IsNullOrEmpty(ctokenCookie.Value))
                            {
                                ctoken = ctokenCookie.Value;
                            }

                        }
                        ////加载cookies
                        ////获取当前站点的Cookie
                        taoquanHttpClient.ChangeGlobleCookies(cacheCookies, TaoUlandWebPageServic.TaobaoQuanDomain);

                        //解析价格阶梯中的内容 直接转为优惠券信息
                        var modelQuan = this.QueryTaoQuanDetailsAsync(ctoken, sellerId, itemId, activityId);

                        return modelQuan.Result;
                    });

                    return tskResult.Result;
                }
                catch (Exception ex)
                {
                    Logger.Info("-------查询淘鹊桥活动券失败！----原因异常如下：-----");
                    Logger.Info(string.Format("sellerId:{0},itemId:{1}", sellerId, itemId));
                    Logger.Error(ex);
                }

                return null;
            }

            /// <summary>
            /// 查询阿里妈妈上是否存在活动券
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <param name="funHandler"></param>
            /// <returns></returns>
            private Task<bool> QueryMamaQuanActivitysExistListAsync(long sellerId, long itemId, QueryQuanCompleteTaskHandler funHandler)
            {
                var taskQuery = Task.Factory.StartNew<bool>(() =>
                {

                    bool result = false;
                    if (sellerId <= 0 || itemId <= 0)
                    {
                        return result;
                    }
                    try
                    {


                        //查询隐藏券的地址
                        var url = string.Concat("https://detail.tmall.com/item.htm?id=", itemId);
                        var timestamp = JavascriptContext.getUnixTimestamp();
                        var ckVisitor = new LazyCookieVistor();
                        var cks = ckVisitor.LoadCookies(AlimamaSiteUrl);

                        //在查询字符串中的Cookie
                        var queryCookie = cks.FirstOrDefault(x => x.Name == "_tb_token_");
                        string queryCookieValue = string.Empty;
                        if (null != queryCookie)
                        {
                            queryCookieValue = queryCookie.Value;
                        }
                        //查询是否有上阿里妈妈推广活动
                        //demo：http://pub.alimama.com/items/search.json?q=http://item.tmall.com/item.htm?id=537099364188&perPageSize=10
                        string queryAddress = string.Format(alimamaQuanGetUrl, url, timestamp, queryCookieValue);
                        ////加载cookies
                        ////获取当前站点的Cookie
                        alimamaHttpClient.ChangeGlobleCookies(cks, AlimamaSiteUrl);

                        // 4 发送请求
                        var clientProxy = new HttpServerProxy() { Client = alimamaHttpClient.Client, KeepAlive = true };

                        var resp = clientProxy.GetResponseTransferAsync(queryAddress, null).Result;
                        if (null == resp || resp.Content == null)
                        {
                            return result;
                        }
                        //异步读取内容字符串
                        //{"data":{"head":{"version":"1.0","status":"OK","pageSize":10,"pageNo":1,"searchUrl":null,"pvid":"100_10.103.34.47_236203_2131501424498625508","errmsg":null,"fromcache":null,"processtime":3829,"ha3time":3503,"docsfound":1,"docsreturn":1,"responseTxt":null},"condition":{"userType":null,"queryType":null,"sortType":null,"loc":null,"includeDxjh":null,"auctionTag":null,"startDsr":null,"hasUmpBonus":null,"isBizActivity":null,"freeShipment":null,"startTkRate":null,"endTkRate":null,"startTkTotalSales":null,"startPrice":null,"endPrice":null,"startRatesum":null,"endRatesum":null,"startQuantity":null,"startBiz30day":null,"startPayUv30":null,"hPayRate30":null,"hGoodRate":null,"jhs":null,"lRfdRate":null,"startSpay30":null,"hSellerGoodrat":null,"hSpayRate30":null,"subOeRule":null,"auctionTagRaw":null,"startRlRate":null,"shopTag":null,"npxType":null,"picQuality":null,"selectedNavigator":null,"typeTagName":null},"paginator":{"length":1,"offset":0,"page":1,"beginIndex":1,"endIndex":1,"items":1,"lastPage":1,"itemsPerPage":10,"previousPage":1,"nextPage":1,"pages":1,"firstPage":1,"slider":[1]},"pageList":[{"tkSpecialCampaignIdRateMap":{"63960348":"50.00","45332944":"25.00","28623940":"20.00","28929421":"10.00","28929471":"50.00"},"rootCatId":0,"leafCatId":50008612,"eventCreatorId":117644290,"debugInfo":null,"rootCatScore":0,"sellerId":2649797694,"userType":0,"shopTitle":"SHIYUAN－石岩村生态庄园官方店","pictUrl":"//img.alicdn.com/bao/uploaded/i2/2649797694/TB2xOquXd3nyKJjSZFjXXcdBXXa_!!2649797694.jpg","title":"石岩村茶园丁香茶正品养胃长白山野生丁香叶茶特紫丁香花茶级包邮","auctionId":537099364188,"tkMktStatus":null,"biz30day":12626,"tkRate":2.80,"nick":"全球茶叶公司","includeDxjh":1,"reservePrice":68.00,"tkCommFee":1.90,"totalFee":14111.30,"totalNum":2943,"zkPrice":68.00,"auctionTag":"258 385 587 651 843 1163 1483 2059 4491 4550 5831 6215 6603 7110 11083 12491 13707 13771 16395 24002 25282 27137 28353 30977 34305 36161 40513 40897 48578 49089 49601 53185 54209 54337 59073 59137 59970 67521 70465 119234 119298 138178 140098 140674 140738 140866 143234 143746 151362 172866","auctionUrl":"http://item.taobao.com/item.htm?id=537099364188","rlRate":0.00,"hasRecommended":0,"hasSame":0,"tk3rdRate":null,"sameItemPid":"9223372036854775807","couponActivityId":null,"couponTotalCount":50000,"couponLeftCount":48205,"couponLink":"","couponLinkTaoToken":"","couponAmount":5,"dayLeft":2,"couponShortLink":null,"couponInfo":"满45元减5元","couponStartFee":45,"couponEffectiveStartTime":"2017-07-21","couponEffectiveEndTime":"2017-12-31","eventRate":30.00,"hasUmpBonus":null,"isBizActivity":null,"umpBonus":null,"rootCategoryName":null,"couponOriLink":null,"userTypeName":null}],"navigator":null,"extraInfo":null},"info":{"message":null,"pvid":"10_180.77.135.254_330_1501424498623","ok":true},"ok":true,"invalidKey":null}
                        string respContent = resp.Content.ReadAsStringAsync().Result;
                        if (string.IsNullOrEmpty(respContent))
                        {
                            return result;
                        }


                        MamaQuanAuctioin mamaQuanResult = JsonConvert.DeserializeObject<MamaQuanAuctioin>(respContent);
                        if (null == mamaQuanResult || mamaQuanResult.ok == false || mamaQuanResult.data == null || mamaQuanResult.data.pageList == null)
                        {
                            return result;
                        }
                        //因为我们是按照商品链接搜索 所以只能有一个
                        var productAuction = mamaQuanResult.data.pageList.FirstOrDefault();
                        if (null == productAuction)
                        {
                            return result;
                        }
                        //过期失效的券活动
                        if (productAuction.couponAmount <= 0 || productAuction.couponLeftCount <= 0)
                        {
                            return result;
                        }

                        //转换查询推广链接
                        timestamp = JavascriptContext.getUnixTimestamp();
                        string tuiguangApiUrl = string.Format(alimamaMyTuiGuangQuanUrl,
                            productAuction.auctionId,
                            timestamp,
                            queryCookieValue,
                            mamaQuanResult.info.pvid
                            );
                        var respOfTuiGuang = clientProxy.GetResponseTransferAsync(tuiguangApiUrl, null).Result;
                        if (null == respOfTuiGuang || respOfTuiGuang.Content == null)
                        {
                            return result;
                        }
                        //异步读取内容字符串
                        //{"data":{"taoToken":"￥uL1c0Yz0Zkk￥","couponShortLinkUrl":"https://s.click.taobao.com/OtZMMew","qrCodeUrl":"//gqrcode.alicdn.com/img?type=hv&text=https%3A%2F%2Fs.click.taobao.com%2FeLVMMew%3Faf%3D3&h=300&w=300","clickUrl":"https://s.click.taobao.com/t?e=m%3D2%26s%3DRDah1ToHrrccQipKwQzePOeEDrYVVa64LKpWJ%2Bin0XLjf2vlNIV67lUAiMP49uDbJdux29CqXYk0gkVGIth8kaXPTjhnp4ESePszmPR8eOguIUNSY24v1C8DJwbWoWv1lrfKbc84rlcbsWZwF8LEJObKIHyGA6hixiXvDf8DaRs%3D&pvid=10_180.77.135.254_527_1501426771407","couponLinkTaoToken":"￥oa700Yz0ZsR￥","couponLink":"https://uland.taobao.com/coupon/edetail?e=97LO7bg8LSibhUsf2ayXDE5NBfsTCtsHMw03sPQ2yo1%2FbtwLdhUU4bcZelJt%2BzjyVfB1Gny0JAla6Dsdc8AKTNzNwQTGaE3k14t9QUPD0GYZqvzm5Xubt7AJ69hyUes5miwKVpSjoJXOaNlgU0mIWqXvhrUNlNUy&pid=mm_31965263_6394774_21932422&af=1","type":"auction","shortLinkUrl":"https://s.click.taobao.com/eLVMMew"},"info":{"message":null,"ok":true},"ok":true,"invalidKey":null}
                        var respContentOfTuiGuang = respOfTuiGuang.Content.ReadAsStringAsync().Result;
                        if (string.IsNullOrEmpty(respContentOfTuiGuang))
                        {
                            return result;
                        }
                        //从阿里妈妈推广结果 获取推广链接
                        if (respContentOfTuiGuang.IndexOf("login.taobao.com/member/login") > -1)
                        {
                            //TODO:登录失效过期 发送邮件到管理员
                            return result;
                        }
                        MamaQuanOrProductTuiGuangResult tuiGuangResult = JsonConvert.DeserializeObject<MamaQuanOrProductTuiGuangResult>(respContentOfTuiGuang);
                        if (null == tuiGuangResult || tuiGuangResult.ok == false || tuiGuangResult.data == null || string.IsNullOrEmpty(tuiGuangResult.data.couponLink))
                        {
                            return result;
                        }

                        result = true;


                    }
                    catch (AggregateException aex)
                    {
                        //任务取消不做处理
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("-------查询阿里妈妈官方活动券失败！----原因异常如下：-----");
                        Logger.Info(string.Format("sellerId:{0},itemId:{1}", sellerId, itemId));
                        Logger.Error(ex);
                    }
                    return result;

                });

                //任务完毕后注册的子任务；用来注册任务完毕后的后续事件
                taskQuery.ContinueWith((tsk) =>
                {
                    if (null != funHandler)
                    {
                        funHandler(tsk, taskQuery);
                    }
                });

                return taskQuery;//将父亲源任务弹出

            }


            /// <summary>
            /// 查询阿里妈妈的活动券-不一定在淘鹊桥上了这个活动
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <returns></returns>
            private async Task<Youhuiquan> QueryMamaQuanActivitysAsync(long sellerId, long itemId)
            {

                if (sellerId <= 0 || itemId <= 0)
                {
                    return null;
                }
                try
                {


                    //查询隐藏券的地址
                    var url = string.Concat("https://detail.tmall.com/item.htm?id=", itemId);
                    var timestamp = JavascriptContext.getUnixTimestamp();
                    var ckVisitor = new LazyCookieVistor();
                    var cks = ckVisitor.LoadCookies(AlimamaSiteUrl);

                    //在查询字符串中的Cookie
                    var queryCookie = cks.FirstOrDefault(x => x.Name == "_tb_token_");
                    string queryCookieValue = string.Empty;
                    if (null != queryCookie)
                    {
                        queryCookieValue = queryCookie.Value;
                    }
                    //查询是否有上阿里妈妈推广活动
                    //demo：http://pub.alimama.com/items/search.json?q=http://item.tmall.com/item.htm?id=537099364188&perPageSize=10
                    string queryAddress = string.Format(alimamaQuanGetUrl, url, timestamp, queryCookieValue);
                    ////加载cookies
                    ////获取当前站点的Cookie
                    alimamaHttpClient.ChangeGlobleCookies(cks, AlimamaSiteUrl);

                    // 4 发送请求
                    var clientProxy = new HttpServerProxy() { Client = alimamaHttpClient.Client, KeepAlive = true };

                    var resp = await clientProxy.GetResponseTransferAsync(queryAddress, null);
                    if (null == resp || resp.Content == null)
                    {
                        return null;
                    }
                    //异步读取内容字符串
                    //{"data":{"head":{"version":"1.0","status":"OK","pageSize":10,"pageNo":1,"searchUrl":null,"pvid":"100_10.103.34.47_236203_2131501424498625508","errmsg":null,"fromcache":null,"processtime":3829,"ha3time":3503,"docsfound":1,"docsreturn":1,"responseTxt":null},"condition":{"userType":null,"queryType":null,"sortType":null,"loc":null,"includeDxjh":null,"auctionTag":null,"startDsr":null,"hasUmpBonus":null,"isBizActivity":null,"freeShipment":null,"startTkRate":null,"endTkRate":null,"startTkTotalSales":null,"startPrice":null,"endPrice":null,"startRatesum":null,"endRatesum":null,"startQuantity":null,"startBiz30day":null,"startPayUv30":null,"hPayRate30":null,"hGoodRate":null,"jhs":null,"lRfdRate":null,"startSpay30":null,"hSellerGoodrat":null,"hSpayRate30":null,"subOeRule":null,"auctionTagRaw":null,"startRlRate":null,"shopTag":null,"npxType":null,"picQuality":null,"selectedNavigator":null,"typeTagName":null},"paginator":{"length":1,"offset":0,"page":1,"beginIndex":1,"endIndex":1,"items":1,"lastPage":1,"itemsPerPage":10,"previousPage":1,"nextPage":1,"pages":1,"firstPage":1,"slider":[1]},"pageList":[{"tkSpecialCampaignIdRateMap":{"63960348":"50.00","45332944":"25.00","28623940":"20.00","28929421":"10.00","28929471":"50.00"},"rootCatId":0,"leafCatId":50008612,"eventCreatorId":117644290,"debugInfo":null,"rootCatScore":0,"sellerId":2649797694,"userType":0,"shopTitle":"SHIYUAN－石岩村生态庄园官方店","pictUrl":"//img.alicdn.com/bao/uploaded/i2/2649797694/TB2xOquXd3nyKJjSZFjXXcdBXXa_!!2649797694.jpg","title":"石岩村茶园丁香茶正品养胃长白山野生丁香叶茶特紫丁香花茶级包邮","auctionId":537099364188,"tkMktStatus":null,"biz30day":12626,"tkRate":2.80,"nick":"全球茶叶公司","includeDxjh":1,"reservePrice":68.00,"tkCommFee":1.90,"totalFee":14111.30,"totalNum":2943,"zkPrice":68.00,"auctionTag":"258 385 587 651 843 1163 1483 2059 4491 4550 5831 6215 6603 7110 11083 12491 13707 13771 16395 24002 25282 27137 28353 30977 34305 36161 40513 40897 48578 49089 49601 53185 54209 54337 59073 59137 59970 67521 70465 119234 119298 138178 140098 140674 140738 140866 143234 143746 151362 172866","auctionUrl":"http://item.taobao.com/item.htm?id=537099364188","rlRate":0.00,"hasRecommended":0,"hasSame":0,"tk3rdRate":null,"sameItemPid":"9223372036854775807","couponActivityId":null,"couponTotalCount":50000,"couponLeftCount":48205,"couponLink":"","couponLinkTaoToken":"","couponAmount":5,"dayLeft":2,"couponShortLink":null,"couponInfo":"满45元减5元","couponStartFee":45,"couponEffectiveStartTime":"2017-07-21","couponEffectiveEndTime":"2017-12-31","eventRate":30.00,"hasUmpBonus":null,"isBizActivity":null,"umpBonus":null,"rootCategoryName":null,"couponOriLink":null,"userTypeName":null}],"navigator":null,"extraInfo":null},"info":{"message":null,"pvid":"10_180.77.135.254_330_1501424498623","ok":true},"ok":true,"invalidKey":null}
                    string respContent = await resp.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(respContent))
                    {
                        return null;
                    }


                    MamaQuanAuctioin mamaQuanResult = JsonConvert.DeserializeObject<MamaQuanAuctioin>(respContent);
                    if (null == mamaQuanResult || mamaQuanResult.ok == false || mamaQuanResult.data == null || mamaQuanResult.data.pageList == null)
                    {
                        return null;
                    }
                    //因为我们是按照商品链接搜索 所以只能有一个
                    var productAuction = mamaQuanResult.data.pageList.FirstOrDefault();
                    if (null == productAuction)
                    {
                        return null;
                    }
                    //过期失效的券活动
                    if (productAuction.couponAmount <= 0 || productAuction.couponLeftCount <= 0)
                    {
                        return null;
                    }

                    //转换查询推广链接
                    timestamp = JavascriptContext.getUnixTimestamp();
                    string tuiguangApiUrl = string.Format(alimamaMyTuiGuangQuanUrl,
                        productAuction.auctionId,
                        timestamp,
                        queryCookieValue,
                        mamaQuanResult.info.pvid
                        );
                    var respOfTuiGuang = await clientProxy.GetResponseTransferAsync(tuiguangApiUrl, null);
                    if (null == respOfTuiGuang || respOfTuiGuang.Content == null)
                    {
                        return null;
                    }
                    //异步读取内容字符串
                    //{"data":{"taoToken":"￥uL1c0Yz0Zkk￥","couponShortLinkUrl":"https://s.click.taobao.com/OtZMMew","qrCodeUrl":"//gqrcode.alicdn.com/img?type=hv&text=https%3A%2F%2Fs.click.taobao.com%2FeLVMMew%3Faf%3D3&h=300&w=300","clickUrl":"https://s.click.taobao.com/t?e=m%3D2%26s%3DRDah1ToHrrccQipKwQzePOeEDrYVVa64LKpWJ%2Bin0XLjf2vlNIV67lUAiMP49uDbJdux29CqXYk0gkVGIth8kaXPTjhnp4ESePszmPR8eOguIUNSY24v1C8DJwbWoWv1lrfKbc84rlcbsWZwF8LEJObKIHyGA6hixiXvDf8DaRs%3D&pvid=10_180.77.135.254_527_1501426771407","couponLinkTaoToken":"￥oa700Yz0ZsR￥","couponLink":"https://uland.taobao.com/coupon/edetail?e=97LO7bg8LSibhUsf2ayXDE5NBfsTCtsHMw03sPQ2yo1%2FbtwLdhUU4bcZelJt%2BzjyVfB1Gny0JAla6Dsdc8AKTNzNwQTGaE3k14t9QUPD0GYZqvzm5Xubt7AJ69hyUes5miwKVpSjoJXOaNlgU0mIWqXvhrUNlNUy&pid=mm_31965263_6394774_21932422&af=1","type":"auction","shortLinkUrl":"https://s.click.taobao.com/eLVMMew"},"info":{"message":null,"ok":true},"ok":true,"invalidKey":null}
                    var respContentOfTuiGuang = await respOfTuiGuang.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(respContentOfTuiGuang))
                    {
                        return null;
                    }
                    //从阿里妈妈推广结果 获取推广链接
                    if (respContentOfTuiGuang.IndexOf("login.taobao.com/member/login") > -1)
                    {
                        //TODO:登录失效过期 发送邮件到管理员
                        return null;
                    }
                    MamaQuanOrProductTuiGuangResult tuiGuangResult = JsonConvert.DeserializeObject<MamaQuanOrProductTuiGuangResult>(respContentOfTuiGuang);
                    if (null == tuiGuangResult || tuiGuangResult.ok == false || tuiGuangResult.data == null)
                    {
                        return null;
                    }

                    //注意：官方不将活动id发送回来 无法知道活动id
                    Youhuiquan quanModel = new Youhuiquan()
                    {
                        itemId = itemId,
                        amount = productAuction.couponAmount.Value,
                        effectiveStartTime = productAuction.couponEffectiveStartTime.Value,
                        effectiveEndTime = productAuction.couponEffectiveEndTime.Value,
                        quanUrl = tuiGuangResult.data.couponShortLinkUrl,//使用的是官方转换后的短连接
                        isHiddenType = false
                    };

                    return quanModel;
                }
                catch (Exception ex)
                {
                    Logger.Info("-------查询阿里妈妈官方活动券失败！----原因异常如下：-----");
                    Logger.Info(string.Format("sellerId:{0},itemId:{1}", sellerId, itemId));
                    Logger.Error(ex);
                }

                //一般不会执行这句代码  除非异常了
                return null;

            }



            /// <summary>
            /// 根据价格阶梯item 转换为优惠券信息
            /// </summary>
            /// <param name="priceVolumeObj"></param>
            /// <param name="itemId"></param>
            private Youhuiquan ConvertPriceVolumeInfoToYouhuiquan(PriceVolumesResult.PriceVolumeItem priceVolumeObj, long itemId)
            {

                if (null == priceVolumeObj)
                {
                    return null;
                }
                var modelQuan = new Youhuiquan();
                //1 起始金额
                //2 优惠金额
                // 3 有效期 开始-结束时间
                // 4 券地址
                string activityId = priceVolumeObj.id;
                modelQuan.activityId = activityId;

                //condition : "满188减20"
                if (!string.IsNullOrEmpty(priceVolumeObj.condition))
                {
                    string[] youhuiArray = null;
                    youhuiArray = priceVolumeObj.condition.Substring(1).Split('减');


                    if (youhuiArray.Length == 2)
                    {
                        //1 起始金额
                        decimal startFee = 0m;
                        decimal.TryParse(youhuiArray[0].Trim(), out startFee);
                        modelQuan.startFee = startFee;
                        //2 优惠金额
                        decimal amount = 0m;
                        decimal.TryParse(youhuiArray[1].Trim(), out amount);
                        modelQuan.amount = amount;
                    }

                }

                if (!string.IsNullOrEmpty(priceVolumeObj.timeRange))
                {
                    string[] timeArray = priceVolumeObj.timeRange.Split('-');
                    if (timeArray.Length == 2)
                    {
                        //3 有效期 开始-结束时间
                        modelQuan.effectiveStartTime = DateTime.Parse(timeArray[0].Trim());
                        modelQuan.effectiveEndTime = DateTime.Parse(timeArray[1].Trim());
                    }
                }


                if (modelQuan.amount > 0)
                {

                    modelQuan.quanUrl = string.Format(taobaoQuanLingQuanGetToClickUrl, activityId, itemId, GlobalContext.Pid);
                }
                modelQuan.itemId = itemId;
                modelQuan.isHiddenType = false;

                return modelQuan;

            }

            #endregion

            #region 查询隐藏券


            /// <summary>
            /// 隐藏券 调用 轻淘客 的API  查询隐藏券
            /// 隐藏券是商家与平台合作设置的活动
            /// </summary>
            /// <param name="queryParas"></param>
            /// <param name="ctoken"></param>
            /// <returns></returns>

            public async Task<List<Youhuiquan>> GetHiddenCouponAsync(YouhuiquanFetchWebPageArgument queryParas, string ctoken)
            {
                if (null == queryParas || queryParas.ArgumentsForQuanDetails == null)
                {
                    return null;
                }
                var para = queryParas.ArgumentsForQuanDetails;
                //获取淘宝券的Cookie集合
                var cacheCookies = new LazyCookieVistor().LoadCookies(TaoUlandWebPageServic.TaobaoQuanDomain);
                Cookie[] arry_cookies = null;
                var ctokenCookie = cacheCookies.FirstOrDefault(x => x.Name == "ctoken");
                //刷新最新token
                if (null != cacheCookies)
                {
                    arry_cookies = cacheCookies.ToArray();
                    if (!string.IsNullOrEmpty(ctokenCookie.Value))
                    {
                        ctoken = ctokenCookie.Value;
                    }

                }

                ////加载cookies
                ////获取当前站点的Cookie
                taoquanHttpClient.ChangeGlobleCookies(cacheCookies, TaoUlandWebPageServic.TaobaoQuanDomain);


                var lstHideQuan = new List<Youhuiquan>();

                var currentHideQuanActivityList = await this.QueryHideQuanActivitysAsync(para.SellerId, para.ItemId);
                if (null == currentHideQuanActivityList || currentHideQuanActivityList.IsEmpty())
                {
                    return null;
                }

                //模拟多任务并行
                int tskLen = currentHideQuanActivityList.Length;
                Task<Youhuiquan>[] array_FetQuanTasks = new Task<Youhuiquan>[tskLen];

                //先刷新下 淘宝 h5 sdk 的cookie
                var taoBaoLoader = new TaobaoWebPageService().RequestLoader as TaobaoWebPageService.TaobaoMixReuestLoader;
                taoBaoLoader.RefreshH5Api_Cookies();

                for (int i = 0; i < currentHideQuanActivityList.Length; i++)
                {
                    var itemActivity = currentHideQuanActivityList.ElementAt(i);
                    //获取活动后 去查询优惠券信息
                    var tskYouhuiquan = this.QueryTaoQuanDetailsAsync(ctoken, para.SellerId, para.ItemId, itemActivity);

                    array_FetQuanTasks[i] = tskYouhuiquan;
                }
                //等待全部任务并行完毕
                //Task.WaitAll(array_FetQuanTasks);
                foreach (var itemTsk in array_FetQuanTasks)
                {
                    var modelQuan = itemTsk.Result;
                    if (null != modelQuan)
                    {
                        lstHideQuan.Add(modelQuan);
                    }
                }





                return lstHideQuan;

            }

            /// <summary>
            /// 查询是否有隐藏优惠券
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <param name="funHandler"></param>
            /// <returns></returns>
            private Task<bool> QueryHideQuanActivitysExistsListAsync(long sellerId, long itemId, QueryQuanCompleteTaskHandler funHandler)
            {
                var taskQuery = Task.Factory.StartNew<bool>(() =>
                 {
                     bool result = false;

                     if (sellerId <= 0 || itemId <= 0)
                     {
                         return result;
                     }

                     try
                     {


                         //查询隐藏券的地址
                         string queryAddress = string.Format(hidenQuanAPI, sellerId, itemId);

                         var clientProxy = new HttpServerProxy() { Client = qingTaoKeHttpClient.Client, KeepAlive = true };
                         var resp = clientProxy.GetResponseTransferAsync(queryAddress, null).Result;
                         if (null == resp || resp.Content == null)
                         {
                             return result;
                         }
                         //异步读取内容字符串
                         //demo:{"status":0,"data":[{"sellerId":"38365748","activityId":"98aef717f29241b2a8209f2f3b832300","amount":10,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":0,"useAble":false},{"sellerId":"38365748","activityId":"954784a166d447ac9dda51b6775eb46a","amount":10,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":0,"useAble":false},{"sellerId":"38365748","activityId":"343ad6fd06a1421dbe65e67752f12824","amount":10,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":"1","useAble":false},{"sellerId":"38365748","activityId":"dcbe30752d474411a93256f39fa9402f","amount":20,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":0,"useAble":false},{"sellerId":"38365748","activityId":"65c9d767cb6242238bc0b89a74f8edd1","amount":0,"applyAmount":0,"endDate":0,"remain":0,"requisitioned":0,"total":0,"startDate":"","quan_class":"1","useAble":false},{"sellerId":"38365748","activityId":"788d960caead44998d852e251f6e934f","amount":0,"applyAmount":0,"endDate":0,"remain":0,"requisitioned":0,"total":0,"startDate":"","quan_class":"1","useAble":true}]}
                         string respContent = resp.Content.ReadAsStringAsync().Result;
                         if (string.IsNullOrEmpty(respContent))
                         {
                             return result;
                         }


                         QingTaoKeHideQuanResult dataList = JsonConvert.DeserializeObject<QingTaoKeHideQuanResult>(respContent);
                         if (null == dataList || dataList.status != 0 || dataList.data == null || dataList.data.Count <= 0)
                         {
                             result = false;
                         }
                         else
                         {
                             result = true;
                             //只要能查出优惠券活动 那么去调用淘宝webapi 进行查询是否有合法的券
                             // 1 
                             //if (dataList.data.Exists(x=>x.useAble==true))
                             //{
                             //result = true;
                             //}
                         }
                     }
                     catch (AggregateException aex)
                     {
                         //任务取消不做处理
                     }
                     catch (Exception ex)
                     {
                         Logger.Error(ex);
                     }
                     return result;
                 });

                //任务完毕后注册的子任务；用来注册任务完毕后的后续事件
                taskQuery.ContinueWith((tsk) =>
                {
                    if (null != funHandler)
                    {
                        funHandler(tsk, taskQuery);
                    }
                });

                return taskQuery;//将父亲源任务弹出

            }
            /// <summary>
            /// 查询隐藏的优惠券-异步
            /// </summary>
            /// <param name="sellerId"></param>
            /// <param name="itemId"></param>
            /// <returns></returns>
            private async Task<string[]> QueryHideQuanActivitysAsync(long sellerId, long itemId)
            {

                if (sellerId <= 0 || itemId <= 0)
                {
                    return null;
                }
                //查询隐藏券的地址
                string queryAddress = string.Format(hidenQuanAPI, sellerId, itemId);

                var qingTaokeCookies = new LazyCookieVistor().LoadCookies(GlobalContext.QingTaokeSiteURL);
                if (null == qingTaokeCookies || qingTaokeCookies.IsEmpty())
                {
                    return null;//没有cookie 不能查询
                }

                ////获取当前站点的Cookie
                qingTaoKeHttpClient.ChangeGlobleCookies(qingTaokeCookies, GlobalContext.QingTaokeSiteURL);

                var clientProxy = new HttpServerProxy() { Client = qingTaoKeHttpClient.Client, KeepAlive = true };


                var resp = await clientProxy.GetResponseTransferAsync(queryAddress, null);
                if (null == resp || resp.Content == null)
                {
                    return null;
                }
                //异步读取内容字符串
                //demo:{"status":0,"data":[{"sellerId":"38365748","activityId":"98aef717f29241b2a8209f2f3b832300","amount":10,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":0,"useAble":false},{"sellerId":"38365748","activityId":"954784a166d447ac9dda51b6775eb46a","amount":10,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":0,"useAble":false},{"sellerId":"38365748","activityId":"343ad6fd06a1421dbe65e67752f12824","amount":10,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":"1","useAble":false},{"sellerId":"38365748","activityId":"dcbe30752d474411a93256f39fa9402f","amount":20,"applyAmount":0,"endDate":"&nbsp;&nbsp;\u5269\u4f59:0\/0","remain":"0","requisitioned":"0","total":0,"startDate":"","quan_class":0,"useAble":false},{"sellerId":"38365748","activityId":"65c9d767cb6242238bc0b89a74f8edd1","amount":0,"applyAmount":0,"endDate":0,"remain":0,"requisitioned":0,"total":0,"startDate":"","quan_class":"1","useAble":false},{"sellerId":"38365748","activityId":"788d960caead44998d852e251f6e934f","amount":0,"applyAmount":0,"endDate":0,"remain":0,"requisitioned":0,"total":0,"startDate":"","quan_class":"1","useAble":true}]}
                string respContent = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(respContent))
                {
                    return null;
                }

                //异步任务字符串数据返回

                QingTaoKeHideQuanResult dataList = JsonConvert.DeserializeObject<QingTaoKeHideQuanResult>(respContent);
                if (null == dataList || dataList.status != 0 || dataList.data == null || dataList.data.Count <= 0)
                {
                    return null;
                }

                return dataList.data.Select(x => x.activityId).ToArray();



            }

            /// <summary>
            /// 异步查询淘宝优惠券详情
            /// {"success":true,"message":"","result":{"retStatus":0,"startFee":9.0,"amount":3.0,"shopLogo":"//img.alicdn.com/bao/uploaded//d1/2c/TB1EbF0KFXXXXbzXXXXSutbFXXX.jpg","shopName":"SHIYUAN－石岩村生态庄园官方店","couponFlowLimit":false,"effectiveStartTime":"2017-07-29 00:00:00","effectiveEndTime":"2017-08-04 23:59:59","couponKey":"65hRBDiplQbiRzMiytwfIWWCMgE60FrJf%2BDeUg6MuerimchIO0RwUkf8vBSTQl%2Bfh%2BrNzX83mLU%3D","pid":null,"item":{"clickUrl":"//s.click.taobao.com/t?e=m%3D2%26s%3Dz7CbmOCCriJw4vFB6t2Z2ueEDrYVVa64LKpWJ%2Bin0XK3bLqV5UHdqfnMg2oL%2BJQ9tTN3K9waqqgUH%2BxaebRc9WL9obvJl7wO3r0WryJqDdTP7y%2Bzd%2FEOS30x7qnPuALfPVG0pCv7imWOR9%2B8W6fQrC8DJwbWoWv1jfmE2SW4AgNo%2FhgJ1Ekc4weG05KKpLRuqEoU2U2DPUX%2FkYUt1u0uX2QxeCONBP61P2l%2FcEwVQXPmbUgWg%2F6qhc0I%2FviJ36sUUSwYGlubnWs%3D","picUrl":"//gaitaobao1.alicdn.com/tfscom/i2/2649797694/TB2xOquXd3nyKJjSZFjXXcdBXXa_!!2649797694.jpg","title":"石岩村茶园丁香茶正品养胃长白山野生丁香叶茶特紫丁香花茶级包邮","reservePrice":68.0,"discountPrice":68.0,"biz30Day":12626,"tmall":"0","postFree":"1","itemId":537099364188,"commission":null,"shareUrl":"//uland.taobao.com/coupon/edetail?e=pfnf%2FKMW8LQGQASttHIRqeevCxuoq5zdyZdWxzED87iAnVXUwZx8ItKDJeYZOx7TZf1qnzJNP4XT1JpVn%2BUO1PbxKU7D3jwVm4VLH9mslwzey%2BRI7cidB8Yz6%2BiTbYgBk%2BYH%2Bw2c2MOD7ovclAc1qw%3D%3D"}}}
            /// </summary>
            /// <param name="ctoken"></param>
            /// <param name="itemId"></param>
            /// <param name="activityId"></param>
            /// <returns></returns>
            private async Task<Youhuiquan> QueryTaoQuanDetailsAsync(string ctoken, long sellerId, long itemId, string activityId)
            {

                if (string.IsNullOrEmpty(ctoken) || itemId <= 0 || string.IsNullOrEmpty(activityId))
                {
                    return null;
                }

                //查询券数据json
                string searchUrl = string.Format(taobaoQuanDetailJsonUrl, ctoken, itemId, activityId);


                // 发送请求
                var clientProxy = new HttpServerProxy() { Client = taoquanHttpClient.Client, KeepAlive = true };

                var resp = await clientProxy.GetResponseTransferAsync(searchUrl, null);
                if (null == resp || resp.Content == null)
                {
                    return null;
                }
                //异步读取内容字符串
                //demo:{"success":true,"message":"","result":{"retStatus":0,"startFee":9.0,"amount":3.0,"shopLogo":"//img.alicdn.com/bao/uploaded//d1/2c/TB1EbF0KFXXXXbzXXXXSutbFXXX.jpg","shopName":"SHIYUAN－石岩村生态庄园官方店","couponFlowLimit":false,"effectiveStartTime":"2017-07-29 00:00:00","effectiveEndTime":"2017-08-04 23:59:59","couponKey":"65hRBDiplQbiRzMiytwfIWWCMgE60FrJf%2BDeUg6MuerimchIO0RwUkf8vBSTQl%2Bfh%2BrNzX83mLU%3D","pid":null,"item":{"clickUrl":"//s.click.taobao.com/t?e=m%3D2%26s%3D4hJECNNm%2FkFw4vFB6t2Z2ueEDrYVVa64LKpWJ%2Bin0XK3bLqV5UHdqR9nw0n9FaFSn7yqOUL3SI0UH%2BxaebRc9WL9obvJl7wO3r0WryJqDdTP7y%2Bzd%2FEOS30x7qnPuALfPVG0pCv7imWOR9%2B8W6fQrC8DJwbWoWv1jfmE2SW4AgNo%2FhgJ1Ekc4weG05KKpLRuqEoU2U2DPUX%2FkYUt1u0uX2QxeCONBP61P2l%2FcEwVQXPmbUgWg%2F6qhc0I%2FviJ36sUUSwYGlubnWs%3D","picUrl":"//gaitaobao1.alicdn.com/tfscom/i2/2649797694/TB2xOquXd3nyKJjSZFjXXcdBXXa_!!2649797694.jpg","title":"石岩村茶园丁香茶正品养胃长白山野生丁香叶茶特紫丁香花茶级包邮","reservePrice":68.0,"discountPrice":68.0,"biz30Day":12626,"tmall":"0","postFree":"1","itemId":537099364188,"commission":null,"shareUrl":"//uland.taobao.com/coupon/edetail?e=pfnf%2FKMW8LQGQASttHIRqeevCxuoq5zdyZdWxzED87iAnVXUwZx8ItKDJeYZOx7TZf1qnzJNP4XT1JpVn%2BUO1PbxKU7D3jwVm4VLH9mslwzey%2BRI7cidB8Yz6%2BiTbYgBk%2BYH%2Bw2c2MOD7ovclAc1qw%3D%3D"}}}
                string respContent = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(respContent))
                {
                    return null;
                }

                //异步任务字符串数据返回

                TaobaoQuanDetailJsonResult dataJsonObj = JsonConvert.DeserializeObject<TaobaoQuanDetailJsonResult>(respContent);
                //对于无效的券 返回空值
                if (null == dataJsonObj || dataJsonObj.success == false || dataJsonObj.result == null)
                {
                    return null;
                }
                if (dataJsonObj.result.IsValidQuan() == false)
                {
                    return null;
                }

                //构建优惠券信息
                var modelQuan = new Youhuiquan
                {
                    activityId = activityId,
                    amount = dataJsonObj.result.amount.Value,
                    startFee = dataJsonObj.result.startFee.Value,
                    effectiveStartTime = dataJsonObj.result.effectiveStartTime.Value,
                    effectiveEndTime = dataJsonObj.result.effectiveEndTime.Value,
                    isHiddenType = true,
                    itemId = itemId,
                    quanUrl = string.Format(taobaoQuanLingQuanGetToClickUrl, activityId, itemId, GlobalContext.Pid)

                };

                return modelQuan;



                //--------------注意：下面调用web h5 api  先不放开了，并发执行会被屏蔽-----------------
                //if (string.IsNullOrEmpty(ctoken) || itemId <= 0 || string.IsNullOrEmpty(activityId))
                //{
                //    return null;
                //}

                //var taoBaoH5Client = new TaobaoWebPageService().RequestLoader as TaobaoWebPageService.TaobaoMixReuestLoader;

                //var respContent = await taoBaoH5Client.LoadH5Api_YouhuiquanDetailAsync(sellerId, activityId);

                ////对于空白的响应或者 失败的
                ////失败的结果： mtopjsonp2({"api":"mtop.taobao.couponMtopReadService.findShopBonusActivitys","v":"2.0","ret":["FAIL_SYS_SESSION_EXPIRED::SESSION失效"],"data":{}})
                //// mtopjsonp1({"api":"mtop.user.getUserSimple","v":"1.0","ret":["FAIL_SYS_ILLEGAL_ACCESS::非法请求"],"data":{}}) FAIL_SYS_TOKEN_EMPTY::令牌为空
                //if (string.IsNullOrEmpty(respContent) || respContent.IndexOf("FAIL_SYS_") != -1)
                //{
                //    return null;
                //}
                ////取出中间的json body
                //int startPos = "mtopjsonp2(".Length;
                //int endPos = respContent.Length - 1;
                //respContent = respContent.Substring(startPos, endPos - startPos);

                //TaobaoQuanDetailJsonResult dataJsonObj = JsonConvert.DeserializeObject<TaobaoQuanDetailJsonResult>(respContent);
                ////对于无效的券 返回空值
                //if (null == dataJsonObj || dataJsonObj.data == null || dataJsonObj.data.module == null || dataJsonObj.data.module.IsEmpty())
                //{
                //    return null;
                //}
                //var jsonEntity = dataJsonObj.data.module.First();//mtopjsonp2({"api":"mtop.taobao.couponMtopReadService.findShopBonusActivitys","data":{"error":"false","haveNextPage":"false","module":[{"activityId":"1550472474","couponId":"973329075","couponType":"0","currencyUnit":"￥","defaultValidityCopywriter":"2017.11.29前有效","description":"使用说明","discount":"7000","endTime":"2017-11-29 23:59:59","intervalDays":"0","intervalHours":"0","poiShop":"false","sellerId":"1690420968","shopNick":"伊芳妮旗舰店","startFee":"8900","startTime":"2017-11-27 00:00:00","status":"1","transfer":"false","useIntervalMode":"false","uuid":"da4216cd2d714ddbbe5a4eca3aea2c34"}],"needInterrupt":"false","totalCount":"0"},"ret":["SUCCESS::调用成功"],"v":"2.0"})
                //if (jsonEntity.IsValidQuan() == false)
                //{
                //    return null;//不有效的优惠券
                //}
                ////构建优惠券信息
                //var modelQuan = new Youhuiquan
                //{
                //    activityId = activityId,
                //    amount = jsonEntity.discount.Value / 100.0m,
                //    startFee = jsonEntity.startFee.Value / 100.0m,
                //    effectiveStartTime = jsonEntity.startTime.Value,
                //    effectiveEndTime = jsonEntity.endTime.Value,
                //    isHiddenType = true,
                //    itemId = itemId,
                //    quanUrl = string.Format(taobaoQuanLingQuanGetToClickUrl, activityId, itemId, GlobalContext.Pid)

                //};

                //return modelQuan;



            }

            #endregion


            /// <summary>
            /// 阿里妈妈商品搜索API
            /// </summary>
            /// <param name="queryParas"></param>
            /// <returns></returns>
            public override string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas)
            {

                string keyWord = queryParas.KeyWord;
                if (string.IsNullOrEmpty(keyWord))
                {
                    return string.Empty;
                }
                //生成时间戳
                string timestamp = JavascriptContext.getUnixTimestamp();

                //加载Cookie
                var ckVisitor = new LazyCookieVistor();
                var cks = ckVisitor.LoadCookies(AlimamaSiteUrl);

                //在查询字符串中的Cookie
                var queryCookie = cks.FirstOrDefault(x => x.Name == "_tb_token_");
                string queryCookieValue = string.Empty;
                if (null != queryCookie)
                {
                    queryCookieValue = queryCookie.Value;
                }

                string searchUrl = string.Format(templateOfSearchUrl, keyWord, timestamp, queryCookieValue);
                var client = alimamaHttpClient;

                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, AlimamaSiteUrl);

                // 4 发送请求
                var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };
                string content = clientProxy.GetRequestTransfer(searchUrl, null);

                return content;

            }

            #endregion


        }

    }


}

