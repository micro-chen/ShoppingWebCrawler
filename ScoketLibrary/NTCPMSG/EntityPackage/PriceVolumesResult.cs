using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{

    /// <summary>
    ///淘宝 价格阶梯
    /// </summary>
    public class PriceVolumesResult
    {

        public List<PriceVolumeItem> priceVolumes { get; set; }

        /// <summary>
        /// 价格阶梯item
        /// {"priceVolumes":[{"condition":"满488减30","id":"e9c303182542418589c1a3ead872acd1","price":"30","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满488领劵立减30","type":"youhuijuan"},{"condition":"满188减20","id":"d9060db139fd4c9bac375e474632e485","price":"20","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满188领劵立减20","type":"youhuijuan"},{"condition":"满88减10","id":"2f9cc940cc0f4f8197e7e0f6dee45087","price":"10","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.08-2017.10.01","title":"满88领劵立减10元","type":"youhuijuan"},{"condition":"满45减5","id":"fbe4efe09a824b7dbc4b83e00d6adb77","price":"5","receivedAmount":"0","status":"unreceived","timeRange":"2017.07.21-2017.12.31","title":"买1立减5元","type":"youhuijuan"}],"receivedCount":0,"unreceivedCount":4}
        /// </summary>
        public class PriceVolumeItem
        {
            public string condition { get; set; }
            public string id { get; set; }
            public string price { get; set; }
            public string receivedAmount { get; set; }
            public string status { get; set; }
            public string timeRange { get; set; }
            public string title { get; set; }
            public string type { get; set; }
        }

    }
  
   
}
