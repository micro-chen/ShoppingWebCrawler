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
    /// <summary>
    /// soap 消息连接
    /// </summary>
    public class SoapTcpConnection : IDisposable
    {


        private string _IPAddress;
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get
            {
                return _IPAddress;
            }
            set
            {
                _IPAddress = value;
            }
        }

        private int _Port;
        /// <summary>
        /// 端口
        /// </summary>
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }

        private ConnectionState _State;

        /// <summary>
        /// 连接状态
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
            }
        }

        /// <summary>
        /// 连接超时时间（秒）,默认为10s
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// socket 连接驱动
        /// </summary>
        private SingleConnectionCable driver = null;


        /// <summary>
        /// soap   消息序列化-json
        /// </summary>
        ISerialize<SoapMessage> iSendMessageSerializer = new NTCPMessage.Serialize.JsonSerializer<SoapMessage>();
        public SoapTcpConnection()
        {
            this.State = ConnectionState.Closed;
            //TimeOut = 30 * 1000;//设置默认连接超时时间
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="address">远程地址</param>
        /// <param name="port">端口</param>
        ///<param name="timeOut">连接超时时间（秒）</param>
        public SoapTcpConnection(string address, int port,int timeOut=30) : this()
        {
            this.State = ConnectionState.Closed;
            this.IPAddress = address;
            this.Port = port;
            this.TimeOut = timeOut * 1000;
        }

        public SoapTcpConnection(ShoppingWebCrawlerSection.ConnectionStringConfig connectionString)
        {
            this.State = ConnectionState.Closed;
            this.IPAddress = connectionString.Address;
            this.Port = connectionString.Port;
            this.TimeOut = connectionString.TimeOut * 1000;
        }

        /// <summary>
        /// 同步发送纯字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SendString(string data)
        {
            if (this.State != ConnectionState.Open)
            {
                this.Open();
            }
            try
            {
                var buffer = Encoding.UTF8.GetBytes(data);
                var resultBytes = this.driver.SyncSend((UInt32)MessageType.None, buffer);
                return Encoding.UTF8.GetString(resultBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 同步发送SOAP消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDataContainer SendSoapMessage(SoapMessage data)
        {
            if (this.State != ConnectionState.Open)
            {
                this.Open();
            }
            IDataContainer repData = null;
            try
            {
                repData = this.driver.SyncSend(
                              (UInt32)MessageType.Json,
                           data,
                           this.TimeOut,
                          iSendMessageSerializer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return repData;

        }


        /// <summary>
        /// 异步发送SOAP消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<IDataContainer> SendSoapMessageAsync(SoapMessage data)
        {
            if (this.State != ConnectionState.Open)
            {
                this.Open();
            }
            return Task.Factory.StartNew(() =>
            {
                IDataContainer repData = null;

                try
                {

                    repData = this.driver.SyncSend(
                               (UInt32)MessageType.Json,
                               data,
                               this.TimeOut,
                              iSendMessageSerializer);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return repData;

            });
        }
        /// <summary>
        /// Ping
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            var pingResult = false;
            if (this.State != ConnectionState.Open)
            {
                this.Open();
            }
            try
            {
               
                //发送ping
                var buffer = Encoding.UTF8.GetBytes("ping");
                var resultBytes = driver.SyncSend((UInt32)MessageType.None, buffer);

                var data = Encoding.UTF8.GetString(resultBytes);
                if (!string.IsNullOrEmpty(data) && data.Equals("pong"))
                {
                    pingResult = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pingResult;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            if (null != this.driver && State == ConnectionState.Open)
            {
                return;
            }

            try
            {

                var currentSettings = new ShoppingWebCrawlerSection.ConnectionStringConfig { Address = this.IPAddress, Port = this.Port, TimeOut= this.TimeOut };
                SoapTcpPool pool = SoapTcpPool.GetPool(currentSettings);
                if (null != pool)
                {
                    driver = pool.GetConnection();
                }
                if (driver == null)
                {
                    driver = SoapTcpPool.CreatNewConnection(this.IPAddress, this.Port);
                }
                if (!driver.Connected)
                {
                    driver.Connect(this.TimeOut);
                }
              
                this.State = ConnectionState.Open;
            }
            catch (Exception ex)
            {
                this.State = ConnectionState.Closed;
                throw ex;
            }


        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (this.State != ConnectionState.Closed && driver != null)
            {
                //扔回连接池中
                var currentSettings = new ShoppingWebCrawlerSection.ConnectionStringConfig { Address = this.IPAddress, Port = this.Port };
                SoapTcpPool pool = SoapTcpPool.GetPool(currentSettings);
                if (null != pool)
                {
                    pool.ReleaseToPool(this.driver);
                    this.driver = null;
                }
                else
                {
                    this.driver.Close();//如果没有在池中 那么直接关闭对象
                }

            }
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



        #region Dispose


        private bool disposed;



        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (State == ConnectionState.Open)
                        Close();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion

    }
}
