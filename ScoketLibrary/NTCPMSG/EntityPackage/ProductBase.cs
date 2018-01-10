using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.EntityPackage.Arguments;

namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 商品的基类
    /// 注意：商品类基类是各个平台的属性的抽象，但是需要支持实例化
    /// 不允许平台个性化的显示各自的字段
    /// </summary>
    public  class ProductBase
    {
        public ProductBase()
        {
            this.SkuList = new List<SkuItem>();
        }

        /// <summary>
        /// 店铺地址
        /// </summary>
        public virtual string ShopUrl { get; set; }
        /// <summary>
        /// 卖家id
        /// </summary>
        public virtual long SellerId { get; set; }

        /// <summary>
        /// 卖家店铺id
        /// </summary>
        public virtual long ShopId { get; set; }
        /// <summary>
        /// 卖家店铺名称
        /// </summary>
        public virtual string ShopName { get; set; }

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
        public string PicUrl { get; set; }


        /// <summary>
        /// 商品标题
        /// : "8胖男孩9男童夏装套装12大童男装短袖10儿童夏季运动服15岁纯棉13"
        /// </summary>
        public string Title { get; set; }

        ///// <summary>
        ///// 推广者pid 
        ///// mm 三段式
        ///// </summary>
        //public string Pid { get; set; }

        /// <summary>
        /// 标题价格--就是标题上面的删除线中的价格
        /// </summary>
        public virtual decimal ReservePrice { get; set; }


        /// <summary>
        /// 卖价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 30天业务成交/付款/评论 数量
        /// 成交后才能评论 所有 评论数量也是成交数量
        /// 注意：这个字段是字符串类型，而不是具体的数字，方便显示为 x.x万的格式，而不是一长溜的数字！！！！
        /// </summary>
        public string Biz30Day { get; set; }

        /// <summary>
        /// 总业务成交/付款/评论 数量
        /// 注意：这个字段是字符串类型，而不是具体的数字，方便显示为 x.x万的格式，而不是一长溜的数字！！！！
        /// </summary>
        public virtual string TotalBizRemarkCount { get; set; }

        /// <summary>
        /// 评论地址
        /// </summary>
        public virtual string RemarkUrl { get; set; }
        /// <summary>
        /// 商品推广链接
        /// </summary>
        public virtual string ClickUrl { get; set; }


        /// <summary>
        /// 商品规格集合
        /// </summary>
        public virtual List<SkuItem> SkuList { get; set; }

        /// <summary>
        /// 归属的平台
        /// </summary>
        public SupportPlatformEnum Platform { get; set; }

        /// <summary>
        /// 是否自营
        /// </summary>
        public virtual bool IsSelfSale { get; set; }
        ///// <summary>
        ///// 优惠券集合
        ///// </summary>
        //public List<Youhuiquan> QuanList { get; set; }


    }
}
