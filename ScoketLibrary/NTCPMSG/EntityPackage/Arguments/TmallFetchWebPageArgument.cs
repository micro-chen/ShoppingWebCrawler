using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【天猫】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public sealed class TmallFetchWebPageArgument:BaseFetchWebPageArgument
    {

        public TmallFetchWebPageArgument()
        {
            this.Platform = SupportPlatformEnum.Tmall;
        }

        /// <summary>
        /// 平台标识
        /// </summary>
        public override SupportPlatformEnum Platform
        {
            get
            {
                return base.Platform;   
            }

             set
            {
                base.Platform = value;
            }
        }

        /// <summary>
        /// 获取【天猫】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="s" },
                 new OrderField { DisplayName="销量", FieldValue="d" },
                 new OrderField { DisplayName="价格", FieldValue="p" },
                 new OrderField { DisplayName="人气", FieldValue="rq" },
                 new OrderField { DisplayName="新品", FieldValue="new" },
                
            };

            return fields;
        }
    }
}
