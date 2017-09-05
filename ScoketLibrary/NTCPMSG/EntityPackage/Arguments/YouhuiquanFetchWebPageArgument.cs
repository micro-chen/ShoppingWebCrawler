using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 查询淘宝优惠券参数结构
    /// 前段 调用展示列表的时候 
    /// 第一阶段发送此参数 用来查询是否商品有优惠券，但是不查询优惠券的额度门槛详细
    /// </summary>
    public class YouhuiquanFetchWebPageArgument
    {

        /// <summary>
        /// 参数列表
        /// 批量查询卖家的商品的优惠券是否存在信息列表
        /// </summary>
        public List<QuanArgument> ArgumentsForExistsList { get; set; }

        /// <summary>
        /// 参数-查询单个商品的优惠券信息
        /// </summary>
        public QuanArgument ArgumentsForQuanDetails { get; set; }


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
