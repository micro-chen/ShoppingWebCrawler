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
        /// 卖家Id
        /// </summary>
        public long SellerId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public long ItemId { get; set; }
    }
}
