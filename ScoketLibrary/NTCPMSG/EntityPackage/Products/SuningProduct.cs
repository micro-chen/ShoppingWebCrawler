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
    }
}
