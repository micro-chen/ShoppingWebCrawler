using System;
using System.Net;
using NTCPMessage.EntityPackage;
using NTCPMessage.Event;
using NTCPMessage.Serialize;

namespace NTCPMessage.Client
{
    public interface ISingleConnectionCable:INTCPConnection
    {
        /// <summary>
        /// 是否自动连接
        /// </summary>
        bool AutoConnect { get; }
        /// <summary>
        /// 窗口Id
        /// </summary>
        ushort CableId { get; }

        /// <summary>
        /// 连接池容量大小
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 尝试连接次数
        /// </summary>
        int TryConnectInterval { get; }

    }
}