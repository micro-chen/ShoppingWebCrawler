using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{

    /// <summary>
    /// 商品规格
    /// 规格：尺寸 大小 功率 等 算是一个规格
    /// 属性：是规格的具体 ，一个规格包含多个属性
    /// </summary>
    public class SkuItem
    {
        /// <summary>
        /// 规格编码
        /// </summary>
        public string SkuId { get; set; }

        /// <summary>
        /// 规格名称
        /// </summary>
        public string SkuName { get; set; }


        /// <summary>
        /// 指定的规格的地址
        ///点击进入指定的规格
        /// </summary>
        public string SkuUrl { get; set; }

        /// <summary>
        /// 规格图片地址
        /// </summary>
        public string SkuImgUrl { get; set; }



        /// <summary>
        ///包含的属性集合
        ///颜色 尺寸  型号 ......
        /// </summary>
        public List<SkuAttribute> AttrList { get; set; }


        /// <summary>
        /// 规格属性
        /// </summary>
        public class  SkuAttribute
        {
            /// <summary>
            /// 属性Id
            /// </summary>
            public long AttrId { get; set; }
     
            /// <summary>
            /// 属性名称
            /// </summary>
            public string AttrName { get; set; }
        }
    }


}
