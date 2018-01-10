using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        #region pdd 无用的json字段属性 不回发到页面

        
        /// <summary>
        /// 店铺地址
        /// </summary>
        [JsonIgnore]
        public override string ShopUrl { get; set; }
        /// <summary>
        /// 卖家id
        /// </summary>
        [JsonIgnore]
        public override long SellerId { get; set; }

        /// <summary>
        /// 卖家店铺id
        /// </summary>
        [JsonIgnore]
        public override long ShopId { get; set; }
        /// <summary>
        /// 卖家店铺名称
        /// </summary>
        [JsonIgnore]
        public override string ShopName { get; set; }

        /// <summary>
        /// 标题价格--就是标题上面的删除线中的价格
        /// </summary>
        [JsonIgnore]
        public override decimal ReservePrice { get; set; }

        /// <summary>
        /// 总业务成交/付款/评论 数量
        /// 注意：这个字段是字符串类型，而不是具体的数字，方便显示为 x.x万的格式，而不是一长溜的数字！！！！
        /// </summary>
        [JsonIgnore]
        public override string TotalBizRemarkCount { get; set; }

        /// <summary>
        /// 评论地址
        /// </summary>
        [JsonIgnore]
        public override string RemarkUrl { get; set; }
        /// <summary>
        /// 商品推广链接
        /// </summary>
        [JsonIgnore]
        public override string ClickUrl { get; set; }
        /// <summary>
        /// 商品规格集合
        /// </summary>
        [JsonIgnore]
        public override List<SkuItem> SkuList { get; set; }

        /// <summary>
        /// 是否自营
        /// </summary>
        [JsonIgnore]
        public override bool IsSelfSale { get; set; }
        #endregion

    }
}
