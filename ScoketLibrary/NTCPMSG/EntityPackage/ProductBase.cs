using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
   public class ProductBase
    {

        /// <summary>
        /// 卖家id
        /// </summary>
        public long sellerId { get; set; }

        /// <summary>
        /// 卖家店铺id
        /// </summary>
        public long shopId { get; set; }
        /// <summary>
        /// 卖家店铺名称
        /// </summary>
        public string  shopName { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        public long itemId { get; set; }

        /// <summary>
        /// 商品地址
        /// </summary>
        public string itemUrl { get; set; }

        /// <summary>
        /// 商品主图地址
        /// : "//gaitaobao3.alicdn.com/tfscom/i2/38365748/TB28APYr5RnpuFjSZFCXXX2DXXa_!!38365748.png"
        /// </summary>
        public string picUrl  { get; set; }


        /// <summary>
        /// 商品标题
        /// : "8胖男孩9男童夏装套装12大童男装短袖10儿童夏季运动服15岁纯棉13"
        /// </summary>
        public string title  { get; set; }
       
        /// <summary>
        /// 推广者pid 
        /// mm 三段式
        /// </summary>
        public string pid { get; set; }

        /// <summary>
        /// 标题价格--就是标题上面的删除线中的价格
        /// </summary>
        public decimal reservePrice { get; set; }


        /// <summary>
        /// 卖价
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 30天业务成交/付款/评论 数量
        /// 成交后才能评论 所有 评论数量也是成交数量
        /// </summary>
        public int biz30Day { get; set; }

        /// <summary>
        /// 总业务成交/付款/评论 数量
        /// </summary>
        public int totalBizCount { get; set; }

        /// <summary>
        /// 商品推广链接
        /// </summary>
        public string clickUrl { get; set; }


        /// <summary>
        /// 商品规格集合
        /// </summary>
        public List<SkuItem> skuList { get; set; }


        /// <summary>
        /// 优惠券集合
        /// </summary>
        public List<Youhuiquan> quanList { get; set; }
    }
}
