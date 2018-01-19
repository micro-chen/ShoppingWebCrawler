using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{
    /// <summary>
    /// 苏宁商品
    /// </summary>
    public class SuningProduct : ProductBase
    {
        public SuningProduct()
        {
            this.Platform = SupportPlatformEnum.Suning;
        }

        /// <summary>
        /// 覆盖shopId
        /// </summary>
        public override long ShopId
        {
            get
            {
                long _shopId;
                long.TryParse(this.BizCode, out _shopId);
                return _shopId;
            }
        }
        /// <summary>
        /// 经销商编码
        /// : "0000000000"
        /// </summary>
        public string BizCode { get; set; }

    }
}
