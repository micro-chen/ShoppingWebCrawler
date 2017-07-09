
using System;
using System.Net;
using NTCPMessage.EntityPackage;
using NTCPMessage.Event;

namespace NTCPMessage.Serialize
{
    /// <summary>
    /// 消息接受处理转换接口
    /// </summary>
    public interface IMessageParse<T>
    {
        /// <summary>
        /// 接受数据并转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ReceiveEventHandler(object sender, ReceiveEventArgs args);

        /// <summary>
        /// 处理消息 
        /// </summary>
        /// <param name="SCBID"></param>
        /// <param name="RemoteIPEndPoint"></param>
        /// <param name="Flag"></param>
        /// <param name="CableId"></param>
        /// <param name="Channel"></param>
        /// <param name="Event"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, MessageFlag Flag,
                   UInt16 CableId, UInt32 Channel, UInt32 Event, T obj);
    }
}
