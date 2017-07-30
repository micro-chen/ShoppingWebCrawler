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
        public long skuId { get; set; }

        /// <summary>
        /// 规格名称
        /// </summary>
        public string skuName { get; set; }


        /// <summary>
        /// 指定的规格的地址
        ///点击进入指定的规格
        /// </summary>
        public string skuUrl { get; set; }

        /// <summary>
        /// 规格图片地址
        /// </summary>
        public string skuImgUrl { get; set; }



        /// <summary>
        ///包含的属性集合
        /// </summary>
        public List<SkuAttribute> attrList { get; set; }


        /// <summary>
        /// 规格属性
        /// </summary>
        public class  SkuAttribute
        {
            /// <summary>
            /// 属性Id
            /// </summary>
            public long attrId { get; set; }
     
            /// <summary>
            /// 属性名称
            /// </summary>
            public string attrName { get; set; }
        }
    }


}
