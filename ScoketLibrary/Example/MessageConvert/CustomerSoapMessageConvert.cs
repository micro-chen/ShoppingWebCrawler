
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage;
using NTCPMessage.Serialize;
using System.Net;
using Newtonsoft.Json;

namespace Example2010.MessageConvert
{


    /// <summary>
    /// 自定义消息转换
    /// </summary>

     public  class CustomerSoapMessageConvert : MessageParse
    {
        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="dataSerializer">serializer for input data</param>
        /// <param name="returnSerializer">serializer for return data</param>
        public CustomerSoapMessageConvert()
            : base(new SoapMessageSerializer(), new JsonSerializer<IDataContainer>())
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
