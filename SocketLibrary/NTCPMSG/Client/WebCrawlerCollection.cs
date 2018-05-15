using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NTCPMessage.Client
{
    /// <summary>
    /// 蜘蛛服务连接集合
    /// </summary>
    public class WebCrawlerCollection : List<WebCrawlerConnection>
    {
        public WebCrawlerCollection()
        {

        }

        /// <summary>
        /// 索引器-按照名称获取连接对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WebCrawlerConnection this[string name]
        {
            get
            {

                var conn = this.FirstOrDefault(x => x.Name == name);
                return conn;
            }
            set
            {
                if (null == this)
                {
                    return;
                }
                var conn = this.FirstOrDefault(x => x.Name == name);

                var pos = this.IndexOf(conn);
                if (null != conn)
                {
                    this[pos] = value;
                }
            }
        }

    }
}
