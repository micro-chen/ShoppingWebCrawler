using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using NTCPMessage.Client;
using NTCPMessage.Event;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;
using NTCPMessage.Compress;

using System.Threading.Tasks;

namespace NTCPMessage.Client
{
    public class BaseSoapTcpClient
    {

        string _IPAddress;
        int _Port;
        byte[] buf;
       // int _packageSize;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address">远程地址</param>
        /// <param name="port">端口</param>
        /// <param name="packageSize">攒包大小</param>
        public BaseSoapTcpClient(string address, int port, int packageSize = 64)
        {

            this._IPAddress = address;
            this._Port = port;
            //this._packageSize = packageSize;
        }

        /// <summary>
        /// 注意 这种数据接收 是接收服务器推送来的数据
        /// 发送请求响应模式 必须用同步发送方法
        /// 异步 是单向通信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            Console.WriteLine("get event:{0}", args.Event);
        }

        public virtual void ErrorEventHandler(object sender, ErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorException);
        }

        public virtual void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            Console.WriteLine("Disconnect from {0}", args.RemoteIPEndPoint);
        }

        /// <summary>
        /// 发送soap消息 并返回结果
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IDataContainer SendSoapMessage(SoapMessage msg)
        {
            IDataContainer result = DataContainer.CreateNullDataContainer();

            //序列化消息
            ISerialize<SoapMessage> iSendMessageSerializer = null;
            ISerialize<DataContainer> iReturnDataSerializer = new NTCPMessage.Serialize.JsonSerializer<DataContainer>();
            iSendMessageSerializer = new NTCPMessage.Serialize.JsonSerializer<SoapMessage>();


            //初始化tcp client
            SingleConnectionCable client = new SingleConnectionCable(new IPEndPoint(IPAddress.Parse(_IPAddress), _Port), 7);
            client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
            client.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

            try
            {
                client.Connect();

                result = client.SyncSend(
               (UInt32)MessageType.Json,
                msg,
               20000,
               iSendMessageSerializer);
                //sw.Stop();
                //Console.WriteLine("Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Close();
            }
            return result;
        }


    }
}
