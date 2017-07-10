using System;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// web 页面请求服务的基类
    /// </summary>
    public abstract class BaseWebPageService: BaseRequest
    {
        public abstract string QuerySearchContent(string keyWord);
        /// <summary>
        /// 格式化 字符串 并且过滤
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string FormatAndFilterString(string input)
        {
            return input;
                //.Replace("<b>", "")
                //.Replace("</b>", "")
                //.Replace("<\\/b>", "");
        }

        /// <summary>
        /// 查询关键词对应的网页内容异步支持
        /// 返回的是 -响应的文本
        /// </summary>
        /// <returns></returns>
        protected Task<string> QuerySearchContentResonseAsync(HttpClient client)
        {
            return this.SendRequesntAsync(client);

        }
    }
}
