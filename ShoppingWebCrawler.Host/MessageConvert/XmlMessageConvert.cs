using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

namespace ShoppingWebCrawler.Host.MessageConvert
{


    /// <summary>
    /// XML  消息转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlMessageConvert : MessageParse
    { 

        public XmlMessageConvert()
            : base(new XMLSerializer<SoapMessage>(), new JsonSerializer<IDataContainer>())
        {

        }

        public override IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, SoapMessage obj)
        {
            //Console.WriteLine(obj);

            var result = new DataResultContainer<string>();
            result.Result = "1111111111";

            return result;
           
        }
    }
}
