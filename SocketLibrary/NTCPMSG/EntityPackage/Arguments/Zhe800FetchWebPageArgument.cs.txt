using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【zhe800】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public sealed class Zhe800FetchWebPageArgument : BaseFetchWebPageArgument
    {


        public Zhe800FetchWebPageArgument()
        {
            this.Platform = SupportPlatformEnum.Zhe800;
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
        /// 获取【zhe800】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="hottest" },
                 new OrderField { DisplayName="销量", FieldValue="sale" },
                 new OrderField { DisplayName="价格", FieldValue="price_up,price_down" },
                 new OrderField { DisplayName="最新", FieldValue="newest" },
                
            };

            return fields;
        }
    }
}
