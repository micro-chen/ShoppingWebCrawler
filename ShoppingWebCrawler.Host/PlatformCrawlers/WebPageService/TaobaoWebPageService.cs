using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.Net.Http;
using ShoppingWebCrawler.Host.Http;
using System.Net;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{

    /// <summary>
    /// 淘宝网页搜索结果抓取
    /// </summary>
    public class TaobaoWebPageService : BaseWebPageService
    {
        private const string taobaoSiteUrl = "https://www.taobao.com/";
        /// <summary>
        /// 淘宝淘宝网页搜索获取地址
        /// </summary>
        private const string templateOfSearchUrl = "https://s.taobao.com/search?data-key=sort&data-value=default&ajax=true&_ksTS=1500016697605_872&callback=jsonp873&ie=utf8&initiative_id=staobaoz_20170714&stats_click=search_radio_all%3A1&js=1&imgfile=&q={0}&source=suggest";

        /// <summary>
        /// 淘宝请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient taobaoHttpClient;

        protected override string TargetUrl
        {
            get; set;
        }


        public TaobaoWebPageService()
        {
        }


        /// <summary>
        /// 静态构造函数
        /// </summary>
        static TaobaoWebPageService()
        {
            //初始化头信息
            var requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", taobaoSiteUrl);
            taobaoHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(taobaoHttpClient.Client.DefaultRequestHeaders, requestHeaders);

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

            this.TargetUrl = string.Format(templateOfSearchUrl, keyWord);

            //获取当前站点的Cookie
            List<Cookie> cks = null;
            GlobalContext.SupportPlatformsCookiesContainer.TryGetValue(taobaoSiteUrl, out cks);
            taobaoHttpClient.ChangeGlobleCookies(cks, taobaoSiteUrl);

            string respText = this.QuerySearchContentResonseAsync(taobaoHttpClient.Client).Result;

            return respText;
        }




        /// <summary>
        /// 爆淘宝联盟的链接-根据淘宝官方跳转js解析算法
        /// 使用postman  发送淘宝客链接可以得到这个算法
        /// </summary>
        /// <param name="encryUrl"></param>
        public static string GetTaobaoUnionOfficalUrl(string encryUrl)
        {
            //示范：下面为淘宝客加密的链接--从一淘过来的
            //string url = "http://s.click.taobao.com/t?spm=1002.8113010.1999451596.1.197829d2jvKq9J&e=m%3D2%26s%3Dxx7h3yvW%2FlwcQipKwQzePOeEDrYVVa64szgHCoaJEBXomhrxaV0k4ZAA5CqNKnVlalBUWfSYtdXqadVuhJq1oW37Sy0WpaHc0S8eIUiNHrwNztF5RF%2BnklwTri0BQMnX1tZRX7Kk0roGkzEdSUwZLhvt%2FrpwP7nD09XRW5e8YPIgsgo%2FaWiDiMYl7w3%2FA2kb";

            var httpHelper = new HttpRequestHelper();
            var requestHeaders = new NameValueCollection();
            requestHeaders.Add("Host", "s.click.taobao.com");
            requestHeaders.Add("Upgrade-Insecure-Requests", "1");
            var resp = httpHelper.CreateGetHttpResponse(encryUrl, requestHeaders);//.// new Http.CookedHttpClient().Client.GetStringAsync(url).Result;

            string tuUrl = resp.ResponseUri.AbsoluteUri;

            string realUrl = TaobaoWebPageService.ConvertTaobaoKeUrlToRealUrl(tuUrl);


            requestHeaders.Add("Referer", tuUrl);
            var resp2 = httpHelper.CreateGetHttpResponse(realUrl, requestHeaders, 50000);

            string carshedUrl = resp2.ResponseUri.AbsoluteUri;

            return carshedUrl;
        }


        /// <summary>
        /// 获取二级tu跳转 tuUrl
        /// </summary>
        /// <param name="tuUrl"></param>
        /// <returns></returns>
        private static string ConvertTaobaoKeUrlToRealUrl(string tuUrl)
        {

            var schema = string.Empty;
            if (tuUrl.IndexOf("https://") == 0)
            {
                schema = "https";
            }
            else
            {
                schema = "http";
            }

            var qs = tuUrl.Split('?')[tuUrl.Split('?').Length - 1].Split('&');
            var qso = new Dictionary<string, string>();
            for (var i = 0; i < qs.Length; i++)
            {
                if (qs[i] != "")
                {
                    var tmpa = qs[i].Split('=');
                    qso[tmpa[0]] = !string.IsNullOrEmpty(tmpa[1]) ? tmpa[1] : "";
                }
            }

            if (!qso.ContainsKey("tu"))
            {
                throw new Exception(string.Concat("未能转换此加密淘宝客链接：", tuUrl));

            }

            string jump_url = string.Empty;
            if (qso["tu"].IndexOf("https") == 0)
            {
                jump_url = qso["tu"].Substring(5);
            }
            else if (qso["tu"].IndexOf("http") == 0)
            {
                jump_url = qso["tu"].Substring(4);
            }


            var jump_address = schema + jump_url;

            var real_jump_address = Microsoft.JScript.GlobalObject.unescape(jump_address);

            return real_jump_address;

        }



    }
}
