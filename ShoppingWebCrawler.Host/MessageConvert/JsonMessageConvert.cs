
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using Newtonsoft.Json;
using NTCPMessage.EntityPackage.Arguments;
using ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Common;
using NTCPMessage;

namespace ShoppingWebCrawler.Host.MessageConvert
{


    /// <summary>
    /// json 接收处理消息转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonMessageConvert : MessageParse
    {

        public JsonMessageConvert()
            : base(new JsonSerializer<SoapMessage>(), new JsonSerializer<IDataContainer>())
        {

        }

        public override IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, SoapMessage obj)
        {
            if (null == obj)
            {
                return DataContainer.CreateNullDataContainer();
            }

            IDataContainer result = null;

            try
            {
                //对消息命令内容进行分支处理
                string actionName = obj.Head;
                switch (actionName)
                {
                    case CommandConstants.CMD_Platforms: result = this.GetAllSupportPlatforms(); break;
                    case CommandConstants.CMD_FetchPage:

                        //从body 中获取参数 ，并传递到指定的Action todo  :指定命令发到指定的平台action解析
                        var args_webpage = JsonConvert.DeserializeObject<BaseFetchWebPageArgument>(obj.Body);
                        result = this.FetchPlatformSearchWebPage(args_webpage);
                        break;
                    case CommandConstants.CMD_FetchquanExistsList:

                        //从body 中获取参数
                        var args_yuohuiquan = JsonConvert.DeserializeObject<YouhuiquanFetchWebPageArgument>(obj.Body);
                        result = this.FetchYouhuiquanExistsList(args_yuohuiquan);

                        break;
                    case CommandConstants.CMD_FetchquanDetails:

                        //从body 中获取参数
                        var args_yuohuiquanDetails = JsonConvert.DeserializeObject<YouhuiquanFetchWebPageArgument>(obj.Body);
                        result = this.FetchYouhuiquanDetails(args_yuohuiquanDetails);

                        break;
                    default:
                        result = DataContainer.CreateNullDataContainer();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;

        }
        

        /// <summary>
        /// 搜索指定的卖家的商品的优惠券是否存在优惠券信息
        /// </summary>
        /// <param name="args_yuohuiquan"></param>
        /// <returns></returns>
        private IDataContainer FetchYouhuiquanExistsList(YouhuiquanFetchWebPageArgument args_yuohuiquan)
        {
            IDataContainer result = DataContainer.CreateNullDataContainer();
            if (null == args_yuohuiquan)
            {
                return result;
            }

            //抓取优惠券信息-列表
            //解析参数的Web蜘蛛服务
            AlimamaWebPageService webPageService = new AlimamaWebPageService();

            try
            {
                
                result = webPageService.QueryYouhuiquanExistsList(args_yuohuiquan);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            return result;
        }

        /// <summary>
        /// 搜索指定的卖家的商品的优惠券详细
        /// </summary>
        /// <param name="args_yuohuiquan"></param>
        /// <returns></returns>
        private IDataContainer FetchYouhuiquanDetails(YouhuiquanFetchWebPageArgument args_yuohuiquan)
        {
            IDataContainer result = DataContainer.CreateNullDataContainer();
            if (null == args_yuohuiquan)
            {
                return result;
            }

            //抓取优惠券信息-列表
            //解析参数的Web蜘蛛服务
            AlimamaWebPageService webPageService = new AlimamaWebPageService();

            try
            {
                result = webPageService.QueryYouhuiquan(args_yuohuiquan);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            return result;
        }


        /// <summary>
        /// 获取所有支持的平台列表
        /// </summary>
        /// <returns></returns>
        private IDataContainer GetAllSupportPlatforms()
        {
            var result = new DataContainer();

            var allPlatforms = GlobalContext.SupportPlatforms;

            result.Result = JsonConvert.SerializeObject(allPlatforms);

            return result;
        }


        /// <summary>
        /// 抓取指定平台的搜索结果网页
        /// </summary>
        /// <returns></returns>
        private IDataContainer FetchPlatformSearchWebPage(IFetchWebPageArgument args)
        {
            IDataContainer result = DataContainer.CreateNullDataContainer();
            //解析参数的Web蜘蛛服务
            BaseWebPageService webPageService = BaseWebPageService.CreateWebPageService(args.Platform);


            if (null != webPageService)
            {
                try
                {
                    result = webPageService.QuerySearchContent(args);
                }
                catch (Exception ex)
                {

                    Logger.Error(ex);
                }
            }
            if (null == result)
            {
                result = DataContainer.CreateNullDataContainer();
            }


            return result;
        }



    }


}
