using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{
    /// <summary>
    /// 唯品会商品
    /// </summary>
    public class VipProduct : ProductBase
    {
        public VipProduct()
        {
            this.Platform = SupportPlatformEnum.Vip;
        }
    }
}
