using NTCPMessage.EntityPackage;
using NTCPMessage.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NTCPMessage.Serialize;

namespace NTCPMessage.MessageConvert
{

    /// <summary>
    /// 默认的消息
    /// 统一为基本字符串消息
    /// </summary>
    public class DefaultMessageConvert: IMessageParse<string>
    {
        public void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {

            string clientData = Encoding.UTF8.GetString(args.Data);
            object ret = ProcessMessage(args.SCBID, args.RemoteIPEndPoint, args.Flag, args.CableId, args.Channel, args.Event,
                clientData);

            if (ret != null)
            {
                string jsonMsg = JsonConvert.SerializeObject(ret);
                args.ReturnData = Encoding.UTF8.GetBytes(jsonMsg);
            }
            else
            {
                args.ReturnData = null;
            }
        }
        public IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, string obj)
        {
            //Console.WriteLine(obj);
            var result = new DataResultContainer<string>();
            result.Result = string.Format("server time is:{0}", DateTime.Now.ToString());

            return result;
         
        }
    }
}
