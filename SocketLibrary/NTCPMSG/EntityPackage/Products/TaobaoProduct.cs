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
        public TaobaoProduct()
        {
            this.Platform = SupportPlatformEnum.Taobao;
        }

        /// <summary>
        /// 卖家地址
        /// </summary>
        public string SellerAddress { get; set; }
        /// <summary>
        /// 是否为金牌卖家
        /// </summary>
        public bool IsGold { get; set; }

        /// <summary>
        /// 是否天猫商品
        /// </summary>
        public bool IsTmall { get; set; }
        /// <summary>
        /// 是否流行
        /// </summary>
        public bool IsFashion { get; set; }
        /// <summary>
        /// 是否新品
        /// </summary>
        public bool IsXinPin { get; set; }

        /// <summary>
        /// 是否有运费险
        /// </summary>
        public bool IsHasYunfeiXian { get; set; }

        
    }
}
