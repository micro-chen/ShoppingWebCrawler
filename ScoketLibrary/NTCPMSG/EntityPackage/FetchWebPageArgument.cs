using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{

    /// <summary>
    /// 抓取网页需要的参数模型
    /// </summary>
    public class FetchWebPageArgument
    {
        /// <summary>
        /// 平台编号
        /// </summary>
        public int PlataformId { get; set; }

        /// <summary>
        /// 查询关键词
        /// </summary>
        public string KeyWord { get; set; }
    }
}
