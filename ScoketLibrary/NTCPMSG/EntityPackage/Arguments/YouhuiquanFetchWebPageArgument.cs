using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 抓取淘宝优惠券参数结构
    /// </summary>
    public class YouhuiquanFetchWebPageArgument
    {

        /// <summary>
        /// 参数列表
        /// 批量查询卖家的商品的优惠券
        /// </summary>
        public List<QuanArgument> ArgumentsList { get; set; }


        public class QuanArgument {
            /// <summary>
            /// 卖家Id
            /// </summary>
            public long SellerId { get; set; }

            /// <summary>
            /// 商品Id
            /// </summary>
            public long ItemId { get; set; }
        }
 
    }
}
