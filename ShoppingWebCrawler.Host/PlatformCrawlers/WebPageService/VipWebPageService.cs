using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.Net.Http;
using ShoppingWebCrawler.Host.Common.Http;
using System.Net;
using ShoppingWebCrawler.Host.Headless;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common;
using NTCPMessage.EntityPackage.Arguments;

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
            private const string templateOfSearchUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getSearchList&f=&_=1513070519238";

            /// <summary>
            /// 检索当前关键词下的-【品类】
            /// </summary>
            private const string queryCategoryUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getCategoryTree&f=&_=1513070166483";
            /// <summary>
            /// 检索当前关键词下的-【查询品牌】
            /// </summary>
            private const string queryBrandUrl = "https://m.vip.com/server.html?rpc&method=SearchRpc.getBrandStoreList&f=&_=1513070611746";

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
                    if (queryParas.IsNeedResolveHeaderTags == true)
                    {
                        //1 查询品牌
                        var brandPara = new Dictionary<string, string>();
                        brandPara.Add("para", new VipSearchParaBrand(keyWord).ToJson());
                        brandTask = clientProxy.PostRequestTransferAsync(queryBrandUrl, PostDataContentType.Json, brandPara, null);
                        // 2 查询分类
                        var categoryTreePara = new Dictionary<string, string>();
                        categoryTreePara.Add("para", new VipSearchParaCategoryTree(keyWord).ToJson());
                        categoryTreeTask = clientProxy.PostRequestTransferAsync(queryCategoryUrl, PostDataContentType.Json, categoryTreePara, null);

                    }
                    else
                    {
                        brandTask = Task.FromResult<HttpResponseMessage>(null);
                        categoryTreeTask = Task.FromResult<HttpResponseMessage>(null);
                    }

                    //3检索内容
                    var searchListPara = new Dictionary<string, string>();
                    var paraDetais = new VipSearchParaSearchList(keyWord);
                    //分页
                    paraDetais.paramsDetails.np = queryParas.PageIndex + 1;
                    //排序
                    int tempSort = 0;
                    int.TryParse(queryParas.OrderFiled.FieldValue, out tempSort);
                    paraDetais.paramsDetails.sort = tempSort;
                    //品牌
                    if (null != queryParas.Brands && queryParas.Brands.IsNotEmpty())
                    {
                        paraDetais.paramsDetails.brand_store_sn = string.Join(",", queryParas.Brands.Select(x => x.BrandId));
                    }
                    //分类+规格
                    if (null != queryParas.TagGroup)
                    {
                        //分类
                        var category_id_1_5_show = queryParas.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_1_5_showTags");
                        paraDetais.paramsDetails.category_id_1_5_show = string.Join(",", category_id_1_5_show.Select(x => x.Value));
                        var category_id_1_show = queryParas.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_1_showTags");
                        paraDetais.paramsDetails.category_id_1_show = string.Join(",", category_id_1_show.Select(x => x.Value));
                        var category_id_2_show = queryParas.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_2_showTags");
                        paraDetais.paramsDetails.category_id_2_show = string.Join(",", category_id_2_show.Select(x => x.Value));
                        var category_id_3_show = queryParas.TagGroup.Tags.Where(x => x.FilterFiled == "category_id_3_showTags");
                        paraDetais.paramsDetails.category_id_3_show = string.Join(",", category_id_3_show.Select(x => x.Value));
                        //规格
                        var props = queryParas.TagGroup.Tags.Where(x => x.FilterFiled == "props");
                        paraDetais.paramsDetails.props = string.Join(";", props.Select(x => x.Value));
                    }
                    searchListPara.Add("para", paraDetais.ToJson());
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
