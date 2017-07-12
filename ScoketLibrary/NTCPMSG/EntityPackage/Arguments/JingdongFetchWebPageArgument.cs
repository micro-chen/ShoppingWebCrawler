using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【京东】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public class JingdongFetchWebPageArgument : BaseFetchWebPageArgument
    {
        /// <summary>
        /// 获取【京东】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="1" },
                 new OrderField { DisplayName="销量", FieldValue="3" },
                 new OrderField { DisplayName="评论数", FieldValue="4" },
                 new OrderField { DisplayName="新品", FieldValue="5" },
                 new OrderField { DisplayName="价格", FieldValue="2" },
                
            };

            return fields;
        }
    }
}
