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
    /// 客户端发送过来的字符串命令进行相应，如：ping
    /// 统一为基本字符串消息
    /// </summary>
    public class DefaultMessageConvert: IMessageParse<string, string>
    {
        public void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {

            string clientData = Encoding.UTF8.GetString(args.Data);
            string ret = ProcessMessage(args.SCBID, args.RemoteIPEndPoint, args.Flag, args.CableId, args.Channel, args.Event,
                clientData);

            if (ret != null)
            {
                args.ReturnData = Encoding.UTF8.GetBytes(ret);
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
        public virtual string ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return string.Empty;
            }

            string result = null;
            //对消息命令内容进行分支处理
            switch (obj)
            {
                case "ping": result = this.PingCmdProcessor(); break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;

        }


        /// <summary>
        /// 接受ping 命令 返回pong
        /// </summary>
        /// <returns></returns>
        private string PingCmdProcessor()
        {
            return  "pong";
        }



    }
}
