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
        /// 连接池最小阈值
        /// </summary>
        public int PoolingMinSize { get; set; }

        /// <summary>
        /// 连接池最大阈值
        /// </summary>
        public int PoolingMaxSize { get; set; }


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
