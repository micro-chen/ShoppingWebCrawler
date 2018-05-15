using System;
using System.Collections.Generic;
using System.Text;

namespace NTCPMessage.Client
{
    /// <summary>
    /// 配置文件中的蜘蛛服务器区域
    /// </summary>
    public class WebCrawlerSection
    {
        /// <summary>
        /// 配置节点名称
        /// </summary>
        public const string SectionName = "ShoppingWebCrawler";

        /// <summary>
        /// 对应的蜘蛛连接配置集合
        /// </summary>
        public WebCrawlerCollection ConnectionStringCollection { get; set; }



    }

    /// <summary>
    /// 蜘蛛连接对象
    /// </summary>
    public class WebCrawlerConnection
    {

        #region 字段
        /// <summary>
        /// 当前正在被占用的计数
        /// </summary>
        public int _SysCurrentUseCount = 0;
        #endregion
        /// <summary>
        /// 命名连接
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 远程地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 连接超时时间（秒）
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// 是否开启连接池模式
        /// </summary>
        public bool Pooling { get; set; }

        private int _PoolingMinSize;
        /// <summary>
        /// 连接池最小阈值，不能为小于1
        /// </summary>
        public int PoolingMinSize
        {
            get
            {
                if (this._PoolingMinSize <= 0)
                {
                    this._PoolingMinSize = 1;
                }
                if (this.Pooling == false)
                {
                    //非连接池模式
                    this._PoolingMinSize = 1;//必须=1
                }
                return this._PoolingMinSize;
            }
            set
            {
                this._PoolingMinSize = value;
            }
        }

        private int _PoolingMaxSize;
        /// <summary>
        /// 连接池最大阈值；不能小于1
        /// </summary>
        public int PoolingMaxSize
        {
            get
            {
                if (this._PoolingMaxSize <= 0)
                {
                    this._PoolingMaxSize = 1;
                }
                if (this.Pooling == false)
                {
                    //非连接池模式
                    this._PoolingMaxSize = 1;//必须=1
                }
                return this._PoolingMaxSize;
            }
            set
            {
                this._PoolingMaxSize = value;
            }
        }


        /// <summary>
        /// 是否是合法的配置
        /// </summary>
        /// <returns></returns>
        public bool IsValidConfig()
        {
            if (!string.IsNullOrEmpty(this.Address) && this.Port > 0)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            if (!IsValidConfig())
            {
                throw new Exception("不合法的配置！");
            }
            return string.Concat(this.Address, ":", this.Port);
        }
    }

}
