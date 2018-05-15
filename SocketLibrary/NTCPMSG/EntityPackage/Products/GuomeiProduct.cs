using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{

    /// <summary>
    /// 国美商品
    /// </summary>
    public class GuomeiProduct : ProductBase
    {
        public GuomeiProduct()
        {
            this.Platform = SupportPlatformEnum.Guomei;
        }
    }
}
