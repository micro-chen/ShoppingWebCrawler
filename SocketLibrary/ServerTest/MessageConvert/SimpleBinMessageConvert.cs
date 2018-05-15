using NTCPMessage.EntityPackage;
using NTCPMessage.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest.MessageConvert
{


    /// <summary>
    /// 简单二进制消息转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleBinMessageConvert : MessageParse
    {

        public SimpleBinMessageConvert()
            : base(new SimpleBinSerializer<SoapMessage>(), new JsonSerializer<IDataContainer>())
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
