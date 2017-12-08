using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NTCPMessage;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.AppStart;
using ShoppingWebCrawler.Host.Common.Common;
using NTCPMessage.Client;
using NTCPMessage.Serialize;

namespace ShoppingWebCrawler.Host.MessageConvert
{
    /// <summary>
    /// 主节点的SOAP消息处理器
    /// </summary>
    public class MasterJsonMessageConvert: JsonMessageConvert
    {
        public override IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, NTCPMessage.MessageFlag Flag, ushort CableId, uint Channel, uint Event, SoapMessage objMsg)
        {
            if (null == objMsg)
            {
                return DataContainer.CreateNullDataContainer();
            }

            IDataContainer result = null;

            try
            {
                //如果不是集群，那么主节点处理消息
                int slavePort = -1;
                var cmdHead = objMsg.Head;
                if (false==GlobalContext.IsClusteringMode//是否开启集群模式
                    ||cmdHead.Equals(CommandConstants.CMD_RegisterSlavePort)
                    || MasterRemoteServer.GetOneSlavePort(out slavePort) == false//是否注册从节点端口)//注册从节点端口
                    )
                {
                    result = base.ProcessMessage(SCBID, RemoteIPEndPoint, Flag, CableId, Channel, Event, objMsg);
                }
                else
                {
                    //转发到从节点
                    result = this.TransferMsgToSlave(slavePort, objMsg);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;

        }
        /// <summary>
        /// 转发消息到从节点
        /// </summary>
        /// <param name="slavePort"></param>
        /// <param name="soaMsg"></param>
        /// <returns></returns>
        private IDataContainer TransferMsgToSlave(int slavePort,SoapMessage soaMsg)
        {
            IDataContainer result = null;
            bool isBeUsed = SocketHelper.IsUsedIPEndPoint(slavePort);
            if (isBeUsed == true)
            {
                // 发送消息
                SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse("127.0.0.1"), slavePort), 7);
                ISerialize<SoapMessage> iSendMessageSerializer = new NTCPMessage.Serialize.JsonSerializer<SoapMessage>(); ;

                try
                {
                    int timeOut = 20 * 1000;
                    //可以使用重载 设置连接超时时间
                    client.Connect(timeOut);
     
                    result = client.SyncSend((UInt32)MessageType.Json,
                    soaMsg, timeOut,
                   iSendMessageSerializer);


                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    client.Close();
                }


            }
            return result;


        }


    }
}
