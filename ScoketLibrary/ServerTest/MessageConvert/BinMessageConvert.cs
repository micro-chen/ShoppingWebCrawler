
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

namespace ServerTest.MessageConvert
{
  
    /// <summary>
    /// 二进制消息转换
    /// </summary>
   public class BinMessageConvert : MessageParse
    {
        public BinMessageConvert()
            : base(new BinSerializer<SoapMessage>(), new JsonSerializer<IDataContainer>())
        {

        }

        public  override IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, SoapMessage obj)
        {
            //Console.WriteLine(obj);
            var result = new DataResultContainer<string>();
            result.Result = "1111111111";

            return result;
           
        }
    }

}
