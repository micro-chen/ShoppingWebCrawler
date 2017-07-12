using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【蘑菇街】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public class MogujieFetchWebPageArgument : BaseFetchWebPageArgument
    {
        /// <summary>
        /// 获取【蘑菇街】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="pop" },
                 new OrderField { DisplayName="销量", FieldValue="sell" },
                 new OrderField { DisplayName="新品", FieldValue="new" },
                
            };

            return fields;
        }
    }
}
