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
    public class KeyWordTag
    {

        /// <summary>
        /// 所在的分组显示名称
        /// 品牌、分类、产地.......
        /// </summary>
        public string GroupShowName { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 对应的平台的特定的筛选字段
        /// </summary>
        public string FilterFiled { get; set; }
        /// <summary>
        /// 标签值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 来自的平台
        /// </summary>
        public SupportPlatformEnum Platform { get; set; }

    }
}
