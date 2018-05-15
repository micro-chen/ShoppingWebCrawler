using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Products
{

    /// <summary>
    /// 美丽说商品
    /// </summary>
    public class MeilishuoProduct : ProductBase
    {
        public MeilishuoProduct()
        {
            this.Platform = SupportPlatformEnum.Meilishuo;
        }
    }
}
