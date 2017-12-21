using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;

using System.Collections.Specialized;

using ShoppingWebCrawler.Host.Common.Http;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 唯品会搜索页面抓取
    /// 未完成--再具体做这家的时候 再做详细的地址解析参数
    /// </summary>
    public class VipWebPageService : BaseWebPageService
    {




        public VipWebPageService()
        {
        }


        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return VipMixReuestLoader.Current;
            }
        }







        ///------------内部类-----------------

        /// <summary>
        /// 唯品会的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class VipMixReuestLoader : BaseBrowserRequestLoader<VipMixReuestLoader>
        {
            public static readonly string VipSiteUrl = GlobalContext.SupportPlatforms.Find(x => x.Platform == SupportPlatformEnum.Vip).SiteUrl;

            /// <summary>
            /// 唯品会请求 搜索地址页面
            /// </summary>
            private  string templateOfSearchUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getSearchList&f=&_="+JavascriptContext.getUnixTimestamp();

            /// <summary>
            /// 检索当前关键词下的-【品类】
            /// </summary>
            private  string queryCategoryUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getCategoryTree&f=&_=" + JavascriptContext.getUnixTimestamp();
            /// <summary>
            /// 检索当前关键词下的-【查询品牌】
            /// </summary>
            private  string queryBrandUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getBrandStoreList&f=&_="+JavascriptContext.getUnixTimestamp();

            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient VipHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static VipMixReuestLoader()
            {

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders(true);
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "m.vip.com");
                requestHeaders.Add("Referer", VipSiteUrl);


                VipHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(VipHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public VipMixReuestLoader()
            {
                ///唯品会刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate = VipSiteUrl;

                this.IntiCefWebBrowser();
            }

            public override string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas)
            {

                VipSearchResultBag resultBag = new VipSearchResultBag();

                string keyWord = queryParas.KeyWord;
                if (string.IsNullOrEmpty(keyWord))
                {
                    return string.Empty;
                }

                try
                {



                    //加载Cookie
                    var ckVisitor = new LazyCookieVistor();
                    var cks = ckVisitor.LoadCookies(VipSiteUrl);




                    string searchUrl = string.Format(templateOfSearchUrl, keyWord);

                    var client = VipHttpClient;
                    client.Client.DefaultRequestHeaders.Referrer = new Uri(string.Format("https://m.vip.com/searchlist.html?q={0}&channel_id=", keyWord));
                    ////加载cookies
                    ////获取当前站点的Cookie
                    client.ChangeGlobleCookies(cks, VipSiteUrl);

                    // 4 发送请求
                    var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };

                    //注意：对于响应的内容 不要使用内置的文本 工具打开，这个工具有bug.看到的文本不全面
                    //使用json格式打开即可看到里面所有的字符串
                    Task<HttpResponseMessage> brandTask;
                    Task<HttpResponseMessage> categoryTreeTask;
                    Task<HttpResponseMessage> searchListTask;
                    string para_brandJson = "";
                    string para_categoryTreeJson = "";
                    string para_searchListJson = "";
                    if (null!=queryParas.ResolvedUrl&&null!=queryParas.ResolvedUrl.ParasPost)
                    {
                        para_brandJson = queryParas.ResolvedUrl.ParasPost["para_brand"].ToString();
                        para_categoryTreeJson = queryParas.ResolvedUrl.ParasPost["para_categoryTree"].ToString();
                        para_searchListJson = queryParas.ResolvedUrl.ParasPost["para_searchList"].ToString();
                    }
                    else
                    {
                         para_brandJson = new VipSearchParaBrand(keyWord).ToJson();
                         para_categoryTreeJson = new VipSearchParaCategoryTree(keyWord).ToJson();
                       
                        //插件不解析的话，那么使用最简单的基本关键词过滤分页，不进行复杂过滤，复杂过滤筛选应该在插件实现
                        var paraDetais = new VipSearchParaSearchList(keyWord);
                        //分页
                        paraDetais.paramsDetails.np = queryParas.PageIndex + 1;
                       
                        para_searchListJson = paraDetais.ToJson();
                         
                    }
                    if (queryParas.IsNeedResolveHeaderTags == true)
                    {
                        //1 查询品牌
                        var brandPara = new Dictionary<string, string>();
                        brandPara.Add("para_brand", para_brandJson);
                        brandTask = clientProxy.PostRequestTransferAsync(queryBrandUrl, PostDataContentType.Json, brandPara, null);
                        // 2 查询分类
                        var categoryTreePara = new Dictionary<string, string>();
                        categoryTreePara.Add("para_categoryTree", para_categoryTreeJson);
                        categoryTreeTask = clientProxy.PostRequestTransferAsync(queryCategoryUrl, PostDataContentType.Json, categoryTreePara, null);

                    }
                    else
                    {
                        brandTask = Task.FromResult<HttpResponseMessage>(null);
                        categoryTreeTask = Task.FromResult<HttpResponseMessage>(null);
                    }

                    //3检索内容
                    var searchListPara = new Dictionary<string, string>();
                    searchListPara.Add("para_searchList", para_searchListJson);
                    searchListTask = clientProxy.PostRequestTransferAsync(templateOfSearchUrl, PostDataContentType.Json, searchListPara, null);

                    //等待任务完毕
                    Task.WaitAll(new Task[] { brandTask, categoryTreeTask, searchListTask });
                    if (brandTask.Result != null)
                    {
                        resultBag.BrandStoreList = brandTask.Result.Content.ReadAsStringAsync().Result;
                    }
                    if (categoryTreeTask.Result != null)
                    {
                        resultBag.CategoryTree = categoryTreeTask.Result.Content.ReadAsStringAsync().Result;
                    }
                    if (searchListTask.Result != null)
                    {
                        resultBag.SearchList = searchListTask.Result.Content.ReadAsStringAsync().Result;
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return resultBag.ToJson();

            }





        }


    }
}
