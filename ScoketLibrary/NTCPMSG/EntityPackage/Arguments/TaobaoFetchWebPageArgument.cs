using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【淘宝】搜索页面 参数
    /// </summary>
    public class TaobaoFetchWebPageArgument : BaseFetchWebPageArgument
    {
        /// <summary>
        /// 获取【淘宝】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="default" },
                 new OrderField { DisplayName="销量", FieldValue="sale-desc" },
                 new OrderField { DisplayName="价格", FieldValue="price-desc" },
                 new OrderField { DisplayName="人气", FieldValue="renqi-desc" },
                 new OrderField { DisplayName="信用", FieldValue="credit-desc" },
                
            };

            return fields;
        }
    }
}
