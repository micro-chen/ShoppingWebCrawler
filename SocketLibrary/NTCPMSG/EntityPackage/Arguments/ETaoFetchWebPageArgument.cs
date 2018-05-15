using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【一淘】搜索页面 参数
    /// </summary>
    public sealed class ETaoFetchWebPageArgument : BaseFetchWebPageArgument
    {


        public ETaoFetchWebPageArgument()
        {
            this.Platform = SupportPlatformEnum.ETao;
        }


        /// <summary>
        /// 排序参数
        /// </summary>
        public override string OrderFiledName
        {
            get
            {
                return "sort";
            }


        }


        /// <summary>
        /// 获取【一淘】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="default", Rule= OrderRule.Default },
                 new OrderField { DisplayName="销量", FieldValue="sales_desc" , Rule= OrderRule.DESC},
                 new OrderField { DisplayName="价格", FieldValue="price_asc", Rule= OrderRule.ASC },
                 new OrderField { DisplayName="价格", FieldValue="price_desc"  ,Rule= OrderRule.DESC},

            };

            return fields;
        }
    }
}
