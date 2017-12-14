using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{
    /// <summary>
    /// 当当商品
    /// </summary>
    public class DangdangProduct : ProductBase
    {
        public DangdangProduct()
        {
            this.Platform = SupportPlatformEnum.Dangdang;
        }
    }
}
