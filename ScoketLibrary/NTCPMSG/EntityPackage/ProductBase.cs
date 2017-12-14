using NTCPMessage.EntityPackage.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
   public abstract class ProductBase
    {

        /// <summary>
        /// 卖家id
        /// </summary>
        public long SellerId { get; set; }

        /// <summary>
        /// 卖家店铺id
        /// </summary>
        public long ShopId { get; set; }
        /// <summary>
        /// 卖家店铺名称
        /// </summary>
        public string  ShopName { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// 商品地址
        /// </summary>
        public string ItemUrl { get; set; }

        /// <summary>
        /// 商品主图地址
        /// : "//gaitaobao3.alicdn.com/tfscom/i2/38365748/TB28APYr5RnpuFjSZFCXXX2DXXa_!!38365748.png"
        /// </summary>
        public string PicUrl  { get; set; }


        /// <summary>
        /// 商品标题
        /// : "8胖男孩9男童夏装套装12大童男装短袖10儿童夏季运动服15岁纯棉13"
        /// </summary>
        public string Title  { get; set; }
       
        ///// <summary>
        ///// 推广者pid 
        ///// mm 三段式
        ///// </summary>
        //public string Pid { get; set; }

        /// <summary>
        /// 标题价格--就是标题上面的删除线中的价格
        /// </summary>
        public decimal ReservePrice { get; set; }


        /// <summary>
        /// 卖价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 30天业务成交/付款/评论 数量
        /// 成交后才能评论 所有 评论数量也是成交数量
        /// </summary>
        public int Biz30Day { get; set; }

        /// <summary>
        /// 总业务成交/付款/评论 数量
        /// </summary>
        public int TotalBizCount { get; set; }

        /// <summary>
        /// 商品推广链接
        /// </summary>
        public string ClickUrl { get; set; }


        /// <summary>
        /// 商品规格集合
        /// </summary>
        public List<SkuItem> SkuList { get; set; }

        /// <summary>
        /// 归属的平台
        /// </summary>
        public SupportPlatformEnum Platform { get; set; }
        /// <summary>
        /// 优惠券集合
        /// </summary>
        public List<Youhuiquan> QuanList { get; set; }

       
    }
}
