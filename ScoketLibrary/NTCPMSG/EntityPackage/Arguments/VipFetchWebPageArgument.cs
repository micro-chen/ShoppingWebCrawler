using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【唯品会】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public sealed class VipFetchWebPageArgument : BaseFetchWebPageArgument
    {
        public VipFetchWebPageArgument()
        {
            this.Platform = SupportPlatformEnum.Vip;
        }

       
        /// <summary>
        /// 获取【唯品会】平台支持的排序字段列表
        /// 0 默认 ，1价格升序 ，2价格降序，3 折扣升序 ，4 折扣降序
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {
                 new OrderField { DisplayName="综合", FieldValue="0" },
                 new OrderField { DisplayName="价格", FieldValue="1,2" },
                 new OrderField { DisplayName="折扣", FieldValue="3,4" },
              
                
            };

            return fields;
        }
    }
}
