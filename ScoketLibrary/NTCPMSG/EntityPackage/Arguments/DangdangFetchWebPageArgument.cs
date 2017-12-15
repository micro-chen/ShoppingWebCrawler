using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【当当】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public sealed class DangdangFetchWebPageArgument : BaseFetchWebPageArgument
    {
        public DangdangFetchWebPageArgument()
        {
            this.Platform = SupportPlatformEnum.Dangdang;
        }

        /// <summary>
        /// 排序参数
        /// </summary>
        public override string OrderFiledName
        {
            get
            {
                return "sort_type";
            }

          
        }



        /// <summary>
        /// 获取【当当】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="sort_default", Rule= OrderRule.Default },
                 new OrderField { DisplayName="销量", FieldValue="sort_sale_amt_asc",Rule=OrderRule.ASC },
                 new OrderField { DisplayName="销量", FieldValue="sort_sale_amt_desc",Rule=OrderRule.DESC },
                 new OrderField { DisplayName="价格", FieldValue="sort_xlowprice_asc",Rule=OrderRule.ASC },
                 new OrderField { DisplayName="价格", FieldValue="sort_xlowprice_desc" ,Rule=OrderRule.DESC },
                 new OrderField { DisplayName="好评", FieldValue="sort_score_asc",Rule=OrderRule.ASC  },
                 new OrderField { DisplayName="好评", FieldValue="sort_score_desc",Rule=OrderRule.DESC  },
                 new OrderField { DisplayName="最新", FieldValue="last_changed_date_asc",Rule=OrderRule.ASC  },
                 new OrderField { DisplayName="最新", FieldValue="last_changed_date_desc",Rule=OrderRule.DESC },

            };

            return fields;
        }
    }
}
