using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 查询关键词 标签
    /// </summary>
    public class BrandTag
    {
       

        /// <summary>
        /// 品牌Id
        /// </summary>
        public string BrandId { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 过滤字段
        /// </summary>
        public string FilterField { get; set; }
        /// <summary>
        /// 来自的平台
        /// </summary>
        public SupportPlatformEnum Platform { get; set; }

    }
}
