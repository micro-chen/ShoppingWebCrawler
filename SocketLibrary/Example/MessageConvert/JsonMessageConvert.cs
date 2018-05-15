
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

namespace Example2010.MessageConvert
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
            //Console.WriteLine(obj);
            var result = new DataContainer();
            result.Result = "1111111111";

            return result;
           
        }
    }


}
