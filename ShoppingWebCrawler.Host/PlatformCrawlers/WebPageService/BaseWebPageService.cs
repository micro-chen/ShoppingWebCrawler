using System;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using NTCPMessage.Compress;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// web 页面请求服务的基类
    /// </summary>
    public abstract class BaseWebPageService : BaseRequest
    {

        /// <summary>
        /// 抽象属性-http请求加载器
        /// </summary>
        public abstract IBrowserRequestLoader RequestLoader { get;  }

        /// <summary>
        /// 根据传递的查询参数 进行网页抓取
        /// 返回抓取后的内容
        /// </summary>
        /// <param name="queryParas"></param>
        /// <returns></returns>
        public DataContainer QuerySearchContent(IFetchWebPageArgument queryParas)
        {

            if (null == queryParas)
            {
                return null;
            }
            var container = new DataContainer();

            string respText = RequestLoader.LoadUrlGetSearchApiContent(queryParas);
            //string compressedString = string.Empty;
            //if (!string.IsNullOrEmpty(respText))
            //{
            //    compressedString = LZString.CompressToBase64(respText);
            //}
            //container.Result = compressedString;
            container.Result = respText;
            return container;
        }

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
        /// 根据不同的平台类型 
        /// 创建对应平台的蜘蛛实例
        /// 工厂分支
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static BaseWebPageService CreateWebPageService(SupportPlatformEnum platform)
        {
            BaseWebPageService webPageService = null;
            switch (platform)
            {
                case SupportPlatformEnum.Tmall:
                    webPageService = new TmallWebPageService();
                    break;
                case SupportPlatformEnum.Taobao:
                    webPageService = new TaobaoWebPageService();
                    break;
                case SupportPlatformEnum.Jingdong:
                    webPageService = new JingdongWebPageService();
                    break;
                case SupportPlatformEnum.Pdd:
                    webPageService = new PddWebPageService();
                    break;
                case SupportPlatformEnum.Vip:
                    webPageService = new VipWebPageService();
                    break;
                case SupportPlatformEnum.Guomei:
                    webPageService = new GuomeiWebPageService();
                    break;
                case SupportPlatformEnum.Suning:
                    webPageService = new SuningWebPageService();
                    break;
                case SupportPlatformEnum.Dangdang:
                    webPageService = new DangdangWebPageService();
                    break;
                case SupportPlatformEnum.Yhd:
                    webPageService = new YhdWebPageService();
                    break;
                //case SupportPlatformEnum.Meilishuo:
                //    webPageService = new MeilishuoWebPageService();
                //    break;
                case SupportPlatformEnum.Mogujie:
                    webPageService = new MogujieWebPageService();
                    break;
                //case SupportPlatformEnum.Zhe800:
                //    webPageService = new Zhe800WebPageService();
                //    break;
                case SupportPlatformEnum.ETao:
                    webPageService = new ETaoWebPageService();
                    break;
                default:
                    break;
            }

            return webPageService;
        }

    }
}
