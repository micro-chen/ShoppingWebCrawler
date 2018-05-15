using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NTCPMessage.EntityPackage.Products
{

    /// <summary>
    /// 京东商品
    /// </summary>
    public class JingdongProduct : ProductBase
    {
        public JingdongProduct()
        {
            this.Platform = SupportPlatformEnum.Jingdong;
        }

        /// <summary>
        /// 商品的pid
        /// </summary>
        [JsonIgnore]
        public string Pid { get; set; }

        /// <summary>
        /// 上一页的起始位置
        /// </summary>
        public int Prev_start { get; set; }
        /// <summary>
        /// 下一页的起始位置
        /// </summary>
        public int Next_start { get; set; }
    }
}
