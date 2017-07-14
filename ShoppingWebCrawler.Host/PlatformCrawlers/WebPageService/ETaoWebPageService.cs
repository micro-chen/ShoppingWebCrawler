using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net;
using System.Web;
using ShoppingWebCrawler.Host.Http;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Host.Handlers;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{

    /*
    
测试代码：
            var etaoWeb = new ETaoWebPageService();
            for (int i = 0; i < 10; i++)
            {
                string con = etaoWeb.QuerySearchContent("大米-" + i.ToString() + DateTime.Now.Ticks.ToString());

                System.Diagnostics.Debug.WriteLine(con);

                //var locker1 = RunningLocker.CreateNewLock();
                //locker1.CancelAfter(2000);
            }

        
        */
    /// <summary>
    /// 一淘搜索页面抓取
    /// </summary>
    public class ETaoWebPageService : BaseWebPageService
    {



        /// <summary>
        /// 请求地址-根据方法传递的参数 动态格式化
        /// </summary>
        protected override string TargetUrl
        {
            get; set;
        }





        public ETaoWebPageService()
        {
        }


        /// <summary>
        /// 查询网页
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public override string QuerySearchContent(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                return null;
            }
            //格式化一个查询地址

            //this.TargetUrl = string.Format(templateOfSearchUrl, keyWord);



            //string respText = ETaoMixReuestLoader.Current.LoadUrlGetSearchApiContent(this.TargetUrl)
            //    .ConfigureAwait(false)
            //    .GetAwaiter()
            //    .GetResult();

            string respText = ETaoMixReuestLoader.Current.LoadUrlGetSearchApiContent(keyWord);
            return respText;
        }



        ///------------内部类-----------------

        /// <summary>
        /// 一淘的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class ETaoMixReuestLoader : BaseBrowserRequestLoader<ETaoMixReuestLoader>
        {
            private const string eTaoSiteUrl = "https://www.etao.com/";

            /// <summary>
            /// 一淘请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "https://www.etao.com/search.htm?nq={0}&spm=1002.8113010.2698880.6862";


            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient etaoHttpClient;

            


            /// <summary>
            /// 静态构造函数
            /// </summary>
            static ETaoMixReuestLoader()
            {
                //静态创建请求客户端
                etaoHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "apie.m.etao.com");
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                //requestHeaders.Add("Referer", eTaoSiteUrl);
                etaoHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(etaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public ETaoMixReuestLoader()
            {
                ///一淘刷新搜索页cookie的地址
                this.RefreshCookieUrl = string.Format("https://www.etao.com/search.htm?nq={0}&spm=1002.8113010.2698880.6862", "洗面奶男" + DateTime.Now.Ticks.ToString()); ;

                this.IntiCefWebBrowser();
            }

            public string LoadUrlGetSearchApiContent(string keyWord)
            {
                //生成时间戳
                string timestamp = JavascriptContext.getUnixTimestamp();

                //加载Cookie
                var ckVisitor = new LazyCookieVistor();
                var cks = ckVisitor.LoadCookies(eTaoSiteUrl);

                var _m_h5_tk_cookie = cks.FirstOrDefault(x => x.Name == "_m_h5_tk");
                if (null == _m_h5_tk_cookie)
                {
                    this.AutoRefeshCookie(this.RefreshCookieUrl);//从新刷新页面 获取 服务器颁发的私钥
                    cks = ckVisitor.LoadCookies(eTaoSiteUrl);
                    _m_h5_tk_cookie = cks.FirstOrDefault(x => x.Name == "_m_h5_tk");
                }
                if (null == _m_h5_tk_cookie || string.IsNullOrEmpty(_m_h5_tk_cookie.Value))
                {
                    throw new Exception("加载授权私钥失败！无法获取对应的cookie:_m_h5_tk ");
                }
                string _m_h5_tk_valueString = _m_h5_tk_cookie.Value.Split('_')[0];

                string etao_appkey = "12574478";

                string paras = string.Concat("{\"s\":0,\"n\":40,\"q\":\"", keyWord, "\",\"needEncode\":false,\"sort\":\"sales_desc\",\"maxPrice\":10000000,\"minPrice\":0,\"serviceList\":\"\",\"navigator\":\"all\",\"urlType\":2}");

                string sign = JavascriptContext.getEtaoJSSDKSign(_m_h5_tk_valueString, timestamp, etao_appkey, paras);

                string url = string.Format("https://apie.m.etao.com/h5/mtop.etao.fe.search/1.0/?type=jsonp&api=mtop.etao.fe.search&v=1.0&appKey=12574478&data={0}&t={1}&sign={2}&callback=jsonp28861232595120323", paras, timestamp, sign);


                string searchUrl = string.Format(templateOfSearchUrl, keyWord);
                var client = etaoHttpClient;

                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, eTaoSiteUrl);
                //修改client 的refer 头
                client.Client.DefaultRequestHeaders.Referrer = new Uri(searchUrl);
                // 4 发送请求
                var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };
                string content = clientProxy.GetRequestTransfer(url, null);

                return content;

            }





        }

    }




}
