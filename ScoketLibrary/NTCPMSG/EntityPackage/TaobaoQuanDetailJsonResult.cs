using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{


    /// <summary>
    /// 淘宝券json详细结构
    /// </summary>
    public class TaobaoQuanDetailJsonResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public ResultStruct result { get; set; }

        public class ResultStruct
        {
            public int retStatus { get; set; }
            public decimal? startFee { get; set; }
            public decimal? amount { get; set; }
            public string shopLogo { get; set; }
            public string shopName { get; set; }
            public bool couponFlowLimit { get; set; }
            public DateTime? effectiveStartTime { get; set; }
            public DateTime? effectiveEndTime { get; set; }
            public string couponKey { get; set; }
            public string pid { get; set; }
            public ItemStruct item { get; set; }
        }

        public class ItemStruct
        {
            public string clickUrl { get; set; }
            public string picUrl { get; set; }
            public string title { get; set; }
            public decimal reservePrice { get; set; }
            public decimal discountPrice { get; set; }
            public int biz30Day { get; set; }
            public string tmall { get; set; }
            public string postFree { get; set; }
            public long itemId { get; set; }
            public string commission { get; set; }
            public string shareUrl { get; set; }
        }


    }

}
