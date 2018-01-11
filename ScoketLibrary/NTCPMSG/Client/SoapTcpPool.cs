using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NTCPMessage.Client
{

    /// <summary>
    /// 连接池
    /// </summary>
    public class SoapTcpPool
    {
        #region 字段属性
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public static bool IsHasInitPoolManager = false;
        private static ConcurrentDictionary<string, SoapTcpPool> _poolManager = new ConcurrentDictionary<string, SoapTcpPool>();
        private  ConcurrentQueue<SingleConnectionCable> _driverQueue;
        private ShoppingWebCrawlerSection.ConnectionStringConfig _config;
        private  int _hasInitDriverCount = 0;
      
        private   AutoResetEvent autoEvent;
         private  object _locker = new object();
        #endregion




        public SoapTcpPool()
        {
            autoEvent = new AutoResetEvent(false);

            _driverQueue = new ConcurrentQueue<SingleConnectionCable>();
        }

        /// <summary>
        /// 初始化连接池管理器
        /// </summary>
        /// <param name="lstConfigs"></param>
        public static void InitPoolManager(List<ShoppingWebCrawlerSection.ConnectionStringConfig> lstConfigs)
        {
            if (IsHasInitPoolManager==true)
            {
                return;
            }
            foreach (var config in lstConfigs)
            {
                GetPool(config);
            }
        }
        /// <summary>
        /// 获取制定连接配置的连接池
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static SoapTcpPool GetPool(ShoppingWebCrawlerSection.ConnectionStringConfig config)
        {
            SoapTcpPool _pool = null;
            var key = config.ToString();
            if (_poolManager.ContainsKey(key))
            {
                _pool = _poolManager[key];
                return _pool;
            }

            //如果字典没有池 那么创建池对象
             _pool = new SoapTcpPool();
            _pool.ConfigPool(config);
            if (!_poolManager.ContainsKey(key))
            {
                _poolManager.TryAdd(key, _pool);
            }
            return _pool;
        }
        /// <summary>
        /// 初始化连接池
        /// </summary>
        public void ConfigPool(ShoppingWebCrawlerSection.ConnectionStringConfig config)
        {
            if (null == config && !config.IsValidConfig())
            {
                return;
            }

            _config = config;
            if (_config.PoolingMinSize<=0)
            {
                _config.PoolingMinSize = 1;//必须大于0
            }
            if (_config.PoolingMaxSize<=0)
            {
                _config.PoolingMaxSize = 1;//必须大于0
            }
            try
            {
                for (int i = 0; i < _config.PoolingMinSize; i++)
                {
                    this.CreatOneConnectionToPool();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 创建一个连接，并压入队列
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        private  SingleConnectionCable CreatOneConnectionToPool()
        {
            SingleConnectionCable driver = null;
            if (_hasInitDriverCount <= this._config.PoolingMaxSize)
            {
                string address = _config.Address;
                int port = _config.Port;
                lock (_locker)
                {
                     driver = CreatNewConnection(address, port);
                    if (null != driver)
                    {
                        //尝试打开驱动连接
                        driver.Connect(_config.TimeOut * 1000,true); 
                    }
                    _driverQueue.Enqueue(driver);

                    _hasInitDriverCount += 1;
                }

            }
            else
            {
                throw new NTcpException("超过最大连接池设置的数目!", ErrorCode.OverPoolingSize);
            }

            return driver;
        }
        /// <summary>
        /// 创建新的连接对象
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static SingleConnectionCable CreatNewConnection(string address,int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            SingleConnectionCable driver = new SingleConnectionCable(endPoint, 7);
            return driver;
        }

        /// <summary>
        /// 释放连接到池子
        /// </summary>
        /// <param name="driver"></param>
        public void ReleaseToPool(SingleConnectionCable driver)
        {
            if (null!=driver)
            {
                this._driverQueue.Enqueue(driver);
            }
        }
        /// <summary>
        /// 从连接池 获取一个连接对象
        /// </summary>
        /// <returns></returns>
        public SingleConnectionCable GetConnection()
        {
            SingleConnectionCable driver = null;
            if (null == _driverQueue)
            {
                throw new Exception("未能正确初始化连接池！请检查配置！");
            }
            //检测是否为空
            if (_driverQueue.IsEmpty)
            {
                this.CreatOneConnectionToPool();
            }

            try
            {
                int fullTimeOut = _config.TimeOut * 1000;
                int timeOut = fullTimeOut;
                DateTime start = DateTime.Now;

                while (timeOut > 0)
                {
                    this._driverQueue.TryDequeue(out driver);
                    //连接驱动窗口必须不为0  ，0 表示无效的缆绳窗口,断开从新连接
                    if (driver != null)
                    {
                        if (driver.CableId==0||driver.Connected==false)
                        {
                            driver.Close();
                            driver.Connect(timeOut, true);
                        }
                        return driver;
                    }

                    // We have no tickets right now, lets wait for one.
                    if (!autoEvent.WaitOne(timeOut, false)) break;
                    timeOut = fullTimeOut - (int)DateTime.Now.Subtract(start).TotalMilliseconds;
                }
                
                if (null==driver)
                {
                    throw new Exception("连接池最大连接池数目已经被消耗完毕！未能正确获取连接对象！请及时关闭连接！");
                }


            }
            catch(NTcpException tcpEx)
            {
                //一旦内部故障 失败 ，捕获异常
                if (tcpEx.Code== ErrorCode.Disconnected&&null!= driver)
                {
                    
                    this._hasInitDriverCount -= 1;//打开阈值开关 将当前驱动的引用 置为最新的连接实例
                    driver = CreatOneConnectionToPool();
                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return driver;
        }
    }
}
