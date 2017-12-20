using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 唯品会网页搜索数据包
    /// </summary>
    public class VipSearchResultBag
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandStoreList { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string CategoryTree { get; set; }
        /// <summary>
        /// 结果列表
        /// </summary>
        public string SearchList { get; set; }
    }
}
