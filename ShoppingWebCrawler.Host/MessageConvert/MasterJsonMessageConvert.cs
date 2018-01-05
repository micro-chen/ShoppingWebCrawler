using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NTCPMessage;
using NTCPMessage.Client;
using NTCPMessage.EntityPackage;
using NTCPMessage.EntityPackage.Arguments;
using ShoppingWebCrawler.Host.Common.Logging;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.AppStart;
using ShoppingWebCrawler.Host.Common.Common;
using NTCPMessage.Serialize;
using System.Collections.Specialized;
using ShoppingWebCrawler.Host.Model;

namespace ShoppingWebCrawler.Host.MessageConvert
{
    /// <summary>
    /// 主节点的SOAP消息处理器
    /// </summary>
    public class MasterJsonMessageConvert : JsonMessageConvert
    {
        /// <summary>
        /// 使用字典 进行连接池的创建
        /// 加速，减少tcp  连接 握手时间
        /// </summary>
        //private Dictionary<string, SingleConnectionCable> _connectionPool = new Dictionary<string, SingleConnectionCable>();

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
                PeekerClusterNode slaveNode = null;
                var cmdHead = objMsg.Head;
                if (false == GlobalContext.IsClusteringMode//是否开启集群模式
                    || cmdHead.Equals(CommandConstants.CMD_RegisterSlavePort)//注册从节点端口
                    || MasterRemoteServer.GetOneSlavePort(out slaveNode) == false//是否注册从节点端口)//注册从节点端口
                    || slaveNode.Port == GlobalContext.MasterSocketPort//分配到了主节点工作
                    )
                {
                    result = base.ProcessMessage(SCBID, RemoteIPEndPoint, Flag, CableId, Channel, Event, objMsg);
                }
                else
                {
                    //转发到从节点
                    result = this.TransferMsgToSlave(slaveNode, objMsg);
                
                }

                if (null!= slaveNode)
                {
                    slaveNode.ConnectedCount -= 1;//设置连接 减量
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
        /// <param name="slaveNode"></param>
        /// <param name="soaMsg"></param>
        /// <returns></returns>
        private IDataContainer TransferMsgToSlave(PeekerClusterNode slaveNode, SoapMessage soaMsg)
        {
            IDataContainer result = null;
            


            try
            {
                //string address = "127.0.0.1";

                using (var conn = new SoapTcpConnection(slaveNode.IpAddress, slaveNode.Port))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    //发送soap
                    result = conn.SendSoapMessage(soaMsg);
                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
 


            return result;


        }


    }
}
