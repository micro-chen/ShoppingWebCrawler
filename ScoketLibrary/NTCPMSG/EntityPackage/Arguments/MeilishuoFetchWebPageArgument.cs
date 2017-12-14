using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【美丽说】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public  sealed class MeilishuoFetchWebPageArgument:BaseFetchWebPageArgument
    {

        public MeilishuoFetchWebPageArgument()
        {
            this.Platform = SupportPlatformEnum.Meilishuo;
        }

      


        /// <summary>
        /// 获取【美丽说】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="流行", FieldValue="pop" },
                 new OrderField { DisplayName="热销", FieldValue="sell" },
                 new OrderField { DisplayName="上新", FieldValue="new" },
                 new OrderField { DisplayName="价格", FieldValue="price_asc,price_desc" },
                
            };

            return fields;
        }
    }
}
