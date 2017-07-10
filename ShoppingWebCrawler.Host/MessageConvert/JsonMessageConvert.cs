
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using Newtonsoft.Json;

namespace ShoppingWebCrawler.Host.MessageConvert
{


    /// <summary>
    /// json 消息转换
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
            if (null==obj)
            {
                return DataResultContainer<string>.CreateNullDataContainer();
            }

            IDataContainer result = null;
            //对消息命令内容进行分支处理
            string actionName = obj.Head;
            switch (actionName)
            {
                case "GetAllSupportPlatforms": result = this.GetAllSupportPlatforms(); break;
                case "FetchPlatformSearchWebPage":
                    try
                    {
                        //从body 中获取参数 ，并传递到指定的Action
                        var args = JsonConvert.DeserializeObject<FetchWebPageArgument>(obj.Body);
                        result = this.FetchPlatformSearchWebPage(args);
                       
                    }
                    catch (Exception ex)
                    {

                        Logging.Logger.WriteException(ex);
                    }
                    break;

                default:
                    result = DataResultContainer<string>.CreateNullDataContainer();
                    break;
            }

            return result;

        }

        /// <summary>
        /// 获取所有支持的平台列表
        /// </summary>
        /// <returns></returns>
        private IDataContainer GetAllSupportPlatforms()
        {
            var result = new DataResultContainer<string>();

            var allPlatforms = GlobalContext.SupportPlatforms;

            result.Result = JsonConvert.SerializeObject(allPlatforms);

            return result;
        }


        /// <summary>
        /// 抓取指定平台的搜索结果网页
        /// </summary>
        /// <returns></returns>
        private IDataContainer FetchPlatformSearchWebPage(FetchWebPageArgument args)
        {
            var result = new DataResultContainer<string>();

            var allPlatforms = GlobalContext.SupportPlatforms;

            result.Result = JsonConvert.SerializeObject(allPlatforms);

            return result;
        }
    }


}
