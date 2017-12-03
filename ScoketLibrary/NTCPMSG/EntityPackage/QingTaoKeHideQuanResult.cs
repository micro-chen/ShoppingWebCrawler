using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 轻淘API 查询出的隐藏活动优惠券
    /// </summary>
    public class QingTaoKeHideQuanResult
    {
        /// <summary>
        /// 0 是正确响应 非0是错误
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 券集合
        /// </summary>
        public List<HideQuanItem> data { get; set; }

        public string v { get; set; }

        /// <summary>
        /// 隐藏券item
        /// </summary>
        public class HideQuanItem
        {
            public long sellerId { get; set; }
            public string activityId { get; set; }
            public decimal? amount { get; set; }
            public decimal? applyAmount { get; set; }
            public string startDate { get; set; }

            public string endDate { get; set; }

            //public string remain { get; set; }
            //public string requisitioned { get; set; }
            //public int total { get; set; }
            //public int quan_class { get; set; }
            public bool useAble { get; set; }
        }

    }



}
