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

namespace ShoppingWebCrawler.Host.MessageConvert
{

    /// <summary>
    /// 默认的消息转换
    /// 统一为基本字符串消息
    /// </summary>
    public class DefaultMessageConvert : IMessageParse<string>
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
        /// <summary>
        /// 处理客户端的消息
        /// </summary>
        /// <param name="SCBID"></param>
        /// <param name="RemoteIPEndPoint"></param>
        /// <param name="Flag"></param>
        /// <param name="CableId"></param>
        /// <param name="Channel"></param>
        /// <param name="Event"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, string obj)
        {
            //Console.WriteLine(obj);
            //var result = new DataResultContainer<string>();
            //result.Result = string.Format("server time is:{0}", DateTime.Now.ToString());

            //return result;
            if (string.IsNullOrEmpty(obj))
            {
                return DataResultContainer<string>.CreateNullDataContainer();
            }

            IDataContainer result = null;
            //对消息命令内容进行分支处理
            switch (obj)
            {
                case "ping": result = this.PingCmdProcessor(); break;
                default:
                    result = DataResultContainer<string>.CreateNullDataContainer();
                    break;
            }

            return result;

        }


        /// <summary>
        /// 接受ping 命令 返回pong
        /// </summary>
        /// <returns></returns>
        private IDataContainer PingCmdProcessor()
        {
            var result = new DataResultContainer<string>();
            result.Result = "pong";

            return result;
        }



    }
}
