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
using System.Collections.Specialized;

namespace ShoppingWebCrawler.Host.MessageConvert
{
    /// <summary>
    /// 主节点的SOAP消息处理器
    /// TODO:未实现 从节点之间的健康监测，一旦ping 3次不通，那么需要移除 失效的从节点
    /// </summary>
    public class MasterJsonMessageConvert : JsonMessageConvert
    {
        /// <summary>
        /// 使用字典 进行连接池的创建
        /// 加速，减少tcp  连接 握手时间
        /// </summary>
        private Dictionary<string, SingleConnectionCable> _connectionPool = new Dictionary<string, SingleConnectionCable>();

        public MasterJsonMessageConvert()
        {

        }
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
                if (false == GlobalContext.IsClusteringMode//是否开启集群模式
                    || cmdHead.Equals(CommandConstants.CMD_RegisterSlavePort)//注册从节点端口
                    || MasterRemoteServer.GetOneSlavePort(out slavePort) == false//是否注册从节点端口)//注册从节点端口
                    || slavePort == GlobalContext.MasterSocketPort//分配到了主节点工作
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
        private IDataContainer TransferMsgToSlave(int slavePort, SoapMessage soaMsg)
        {
            IDataContainer result = null;
            SingleConnectionCable client = null;
            string connName = string.Concat("127.0.0.1:", slavePort);

            // 发送消息
            if (_connectionPool.Count > 0)
            {
                _connectionPool.TryGetValue(connName, out client);
            }
            if (null==client)
            {
                client= new SingleConnectionCable(new IPEndPoint(IPAddress.Parse("127.0.0.1"), slavePort), 7);
                _connectionPool.Add(connName, client);
            }
            
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
            ////finally
            ////{
            ////    client.Close();
            ////}



            return result;


        }


    }
}
