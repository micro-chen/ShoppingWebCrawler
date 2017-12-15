using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 排序字段
    /// </summary>
    public class OrderField
    {
   
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 排序字段值
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// 排序规则
        /// </summary>
        public OrderRule Rule { get; set; }

    }
}
