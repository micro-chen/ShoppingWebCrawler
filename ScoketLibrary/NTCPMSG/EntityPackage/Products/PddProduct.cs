using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{

    /// <summary>
    /// 拼多多商品
    /// </summary>
    public class PddProduct : ProductBase
    {
        public PddProduct()
        {
            this.Platform = SupportPlatformEnum.Pdd;
        }
    }
}
