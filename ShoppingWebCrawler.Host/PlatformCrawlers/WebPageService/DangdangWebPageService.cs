﻿using System;
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
    /// 当当搜索页面榨取
    /// </summary>
    public class DangdangWebPageService : BaseWebPageService
    {

        private const string dangdangSiteUrl = "http://www.dangdang.com";

        private const string templateOfSearchUrl = "http://search.dangdang.com/?key={0}&act=input";
        /// <summary>
        /// 保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient dangdangHttpClient;


        /// <summary>
        /// 请求地址-根据方法传递的参数 动态格式化
        /// </summary>
        protected override string TargetUrl
        {
            get;set;
        }



        

        public DangdangWebPageService()
        {
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DangdangWebPageService()
        {
            //初始化头信息
            var requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", dangdangSiteUrl);
            dangdangHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(dangdangHttpClient.Client.DefaultRequestHeaders, requestHeaders);

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
            CookieContainer ckContainer = GlobalContext.SupportPlatformsCookiesContainer[dangdangSiteUrl];
            dangdangHttpClient.Cookies = ckContainer;

            string respText = this.QuerySearchContentResonseAsync(dangdangHttpClient.Client).Result;

            return respText;
        }


    }
}