using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{

    /// <summary>
    /// 蘑菇街商品
    /// </summary>
    public class MogujieProduct : ProductBase
    {
        public MogujieProduct()
        {
            this.Platform = SupportPlatformEnum.Mogujie;
        }
    }
}
