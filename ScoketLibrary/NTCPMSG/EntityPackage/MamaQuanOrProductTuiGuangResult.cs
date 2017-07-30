using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{

    /// <summary>
    /// 推广商品返回的转换后的商品链接和券链接
    /// </summary>
    public class MamaQuanOrProductTuiGuangResult
    {
        public DataStruct data { get; set; }
        public InfoStruct info { get; set; }
        public bool ok { get; set; }
        public string invalidKey { get; set; }

        public class DataStruct
        {
            public string taoToken { get; set; }
            public string couponShortLinkUrl { get; set; }
            public string qrCodeUrl { get; set; }
            public string clickUrl { get; set; }
            public string couponLinkTaoToken { get; set; }
            public string couponLink { get; set; }
            public string type { get; set; }
            public string shortLinkUrl { get; set; }
        }
        public class InfoStruct
        {
            public string message { get; set; }
            public bool ok { get; set; }
        }
    }


  
}
