
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
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.DeskTop.MessageConvert
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
                    case "platforms": result = this.GetAllSupportPlatforms(); break;
                    case "alimamatoken":
                        //获取阿里妈妈登录后的 Cookie json结构
                        result = this.FetchALimamaCookie();
                        break;

                    default:
                        result = DataContainer.CreateNullDataContainer();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }
            return result;

        }

        /// <summary>
        /// 获取阿里妈妈登录后的 Cookie json结构
        /// </summary>
        /// <returns></returns>
        private IDataContainer FetchALimamaCookie()
        {
            var result = DataContainer.CreateNullDataContainer();
            try
            {
                result = Services.AlimamaService.GetLoginTokenCookies();
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
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




    }


}
