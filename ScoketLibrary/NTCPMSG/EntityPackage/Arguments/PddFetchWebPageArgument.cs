using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【拼多多】搜索页面 参数
    /// 具体详细的搜索面板 回头慢慢完善
    /// </summary>
    public class PddFetchWebPageArgument : BaseFetchWebPageArgument
    {
        /// <summary>
        /// 获取【拼多多】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {
                //---------拼多多不支持排序过滤---------------
                 //new OrderField { DisplayName="综合", FieldName="s" },
                 //new OrderField { DisplayName="销量", FieldName="d" },
                 //new OrderField { DisplayName="价格", FieldName="p" },
                 //new OrderField { DisplayName="人气", FieldName="rq" },
                 //new OrderField { DisplayName="新品", FieldName="new" },
                
            };

            return fields;
        }
    }
}
