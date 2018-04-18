using System;
using System.Net;
using NTCPMessage.EntityPackage;
using NTCPMessage.Event;
using NTCPMessage.Serialize;

namespace NTCPMessage.Client
{
    public interface INTCPConnection
    {
        /// <summary>
        /// 远程server IP 地址
        /// </summary>
        IPEndPoint RemoteIPEndPoint { get; }

        /////// <summary>
        /////// 绑定的本地IP 地址
        /////// </summary>
        ////IPEndPoint BindIPEndPoint { get; }

        /// <summary>
        /// 是否连接
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// 默认消息序列化器
        /// </summary>
        ISerialize DefaultDataSerializer { get; set; }

        /// <summary>
        /// 默认接收消息序列化器
        /// </summary>
        ISerialize DefaultReturnSerializer { get; set; }

        #region 事件注册


        event EventHandler<CableConnectedEventArgs> ConnectedEventHandler;
        event EventHandler<ErrorEventArgs> ErrorEventHandler;
        event EventHandler<ReceiveEventArgs> ReceiveEventHandler;
        event EventHandler<DisconnectEventArgs> RemoteDisconnected;

        #endregion
        #region 异步发送


        /// <summary>
        /// 异步发送对象
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="obj"></param>
        void AsyncSend(uint evt, object obj);

        /// <summary>
        /// 异步发送字节数组
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="data"></param>
        void AsyncSend(uint evt, byte[] data);

        /// <summary>
        /// 异步发送对象
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="obj"></param>
        /// <param name="serializer"></param>
        void AsyncSend(uint evt, object obj, ISerialize serializer);
        void AsyncSend<T>(uint evt, ref T obj, ISerialize<T> serializer);

        #endregion

        #region 同步发送

        IDataContainer SyncSend(uint evt, object obj);
        byte[] SyncSend(uint evt, byte[] data);
        IDataContainer SyncSend(uint evt, object obj, int millisecondsTimeout);
        byte[] SyncSend(uint evt, byte[] data, int millisecondsTimeout);
        IDataContainer SyncSend(uint evt, object obj, int millisecondsTimeout, ISerialize serializer);
        IDataContainer SyncSend(uint evt, object obj, int millisecondsTimeout, ISerialize dataSerializer, ISerialize returnSerializer);
        R SyncSend<T, R>(uint evt, ref T obj, int millisecondsTimeout, ISerialize<T> dataSerializer, ISerialize<R> returnSerializer);

        #endregion
        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();

        /// <summary>
        /// 尝试连接
        /// </summary>
        void Connect();


        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <param name="millisecondsTimeout">超时时间</param>
        void Connect(int millisecondsTimeout);
        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <param name="millisecondsTimeout">超时时间</param>
        /// <param name="setThreadAffinityMask">指定线程所在的CPU</param>
        void Connect(int millisecondsTimeout, bool setThreadAffinityMask);

        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();
        void Dispose();

    }
}