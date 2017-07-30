using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{
    /// <summary>
    /// 淘宝商品实体
    /// </summary>
    public class TaobaoProduct:ProductBase
    {

        /// <summary>
        /// 卖家地址
        /// </summary>
        public string sellerAddress { get; set; }
        /// <summary>
        /// 是否为金牌卖家
        /// </summary>
        public bool isGold { get; set; }

        /// <summary>
        /// 是否天猫商品
        /// </summary>
        public bool isTmall { get; set; }

        /// <summary>
        /// 卖家承诺N天包退换
        /// </summary>
        public int dayReturn { get; set; }

        /// <summary>
        /// 是否有运费险
        /// </summary>
        public bool hasYunfeiXian { get; set; }

        
    }
}
