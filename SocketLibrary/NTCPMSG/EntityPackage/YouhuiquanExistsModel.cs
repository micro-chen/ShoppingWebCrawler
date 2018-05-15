using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 优惠券实体是否存在模型
    /// </summary>
    public class YouhuiquanExistsModel
    {
        /// <summary>
        /// 卖家Id
        /// </summary>
        public long SellerId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// 是否有优惠券
        /// </summary>
        public bool IsExistsQuan{ get; set; }

    }
}
