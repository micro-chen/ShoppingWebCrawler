using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 优惠券实体
    /// </summary>
    public class Youhuiquan
    {
       
        /// <summary>
        /// 满减起始价格
        /// </summary>
        public decimal startFee { get; set; }

        /// <summary>
        /// 优惠券金额
        /// </summary>
        public decimal amount  { get; set; }

        /// <summary>
        /// 券开始时间
        /// effectiveStartTime : "2017-07-25 00:00:00"
        /// </summary>
        public DateTime effectiveStartTime { get; set; }

        /// <summary>
        /// 券结束时间
        /// effectiveEndTime : "2017-07-31 23:59:59"
        /// </summary>
        public DateTime effectiveEndTime { get; set; }

      

        /// <summary>
        /// 优惠券地址
        /// </summary>
        public string quanUrl { get; set; }

        /// <summary>
        /// 是否是隐藏券
        /// </summary>
        public bool isHiddenType { get; set; }

        /// <summary>
        /// 商品id
        /// </summary>
        public long itemId { get; set; }
        /// <summary>
        /// 活动id 非json 属性
        /// </summary>
        [JsonIgnore]
        public string activityId { get; set; }

    }
}
