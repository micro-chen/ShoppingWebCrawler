using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【一号店】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public class YhdFetchWebPageArgument: BaseFetchWebPageArgument
    {
        /// <summary>
        /// 获取【一号店】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="s1" },
                 new OrderField { DisplayName="销量", FieldValue="s2" },
                 new OrderField { DisplayName="评论数", FieldValue="s5" },
                 new OrderField { DisplayName="新品", FieldValue="s6" },
                 new OrderField { DisplayName="价格", FieldValue="s3" },
                
            };

            return fields;
        }
    }
}
