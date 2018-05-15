using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{


    /// <summary>
    /// 阿里妈妈竞价活动
    /// 官方推广活动查询返回的数据结构
    /// </summary>
    public class MamaQuanAuctioin
    {
        public DataStruct data { get; set; }
        public InfoStruct info { get; set; }
        public bool ok { get; set; }
        public string invalidKey { get; set; }

        public class DataStruct
        {
            //public head head { get; set; }
            //public condition condition { get; set; }
            //public paginator paginator { get; set; }
            public IEnumerable<PageListStruct> pageList { get; set; }
            //public string navigator { get; set; }
            //public string extraInfo { get; set; }
        }

        public class PageListStruct
        {
            //public tkSpecialCampaignIdRateMap tkSpecialCampaignIdRateMap { get; set; }
            //public int? rootCatId { get; set; }
            //public int? leafCatId { get; set; }
            //public int? eventCreatorId { get; set; }
            //public string debugInfo { get; set; }
            //public int? rootCatScore { get; set; }
            public long sellerId { get; set; }
            public int? userType { get; set; }
            public string shopTitle { get; set; }
            public string pictUrl { get; set; }
            public string title { get; set; }
            public long? auctionId { get; set; }
            //public string tkMktStatus { get; set; }
            //public int? biz30day { get; set; }
            //public double? tkRate { get; set; }
            //public string nick { get; set; }
            //public int? includeDxjh { get; set; }
            //public decimal? reservePrice { get; set; }
            //public decimal? tkCommFee { get; set; }
            //public decimal? totalFee { get; set; }
            //public int? totalNum { get; set; }
            //public decimal? zkPrice { get; set; }
            //public string auctionTag { get; set; }
            //public string auctionUrl { get; set; }
            //public decimal? rlRate { get; set; }
            //public int? hasRecommended { get; set; }
            //public int? hasSame { get; set; }
            //public string tk3rdRate { get; set; }
            //public string sameItemPid { get; set; }
            public string couponActivityId { get; set; }
            public int? couponTotalCount { get; set; }
            public int? couponLeftCount { get; set; }
            public string couponLink { get; set; }
            public string couponLinkTaoToken { get; set; }
            public decimal? couponAmount { get; set; }
            public int? dayLeft { get; set; }
            //public string couponShortLink { get; set; }
            //public string couponInfo { get; set; }
            public decimal? couponStartFee { get; set; }
            public DateTime? couponEffectiveStartTime { get; set; }
            public DateTime? couponEffectiveEndTime { get; set; }
            //public decimal? eventRate { get; set; }
            //public string hasUmpBonus { get; set; }
            //public string isBizActivity { get; set; }
            //public string umpBonus { get; set; }
            //public string rootCategoryName { get; set; }
            //public string couponOriLink { get; set; }
            //public string userTypeName { get; set; }
        }

        public class InfoStruct
        {
            public string message { get; set; }
            public string pvid { get; set; }
            public bool ok { get; set; }
        }

    }    //public class head
    //{
    //    public string version { get; set; }
    //    public string status { get; set; }
    //    public int? pageSize { get; set; }
    //    public int? pageNo { get; set; }
    //    public string searchUrl { get; set; }
    //    public string pvid { get; set; }
    //    public string errmsg { get; set; }
    //    public string fromcache { get; set; }
    //    public int? processtime { get; set; }
    //    public int? ha3time { get; set; }
    //    public int? docsfound { get; set; }
    //    public int? docsreturn { get; set; }
    //    public string responseTxt { get; set; }
    //}
    //public class condition
    //{
    //    public string userType { get; set; }
    //    public string queryType { get; set; }
    //    public string sortType { get; set; }
    //    public string loc { get; set; }
    //    public string includeDxjh { get; set; }
    //    public string auctionTag { get; set; }
    //    public string startDsr { get; set; }
    //    public string hasUmpBonus { get; set; }
    //    public string isBizActivity { get; set; }
    //    public string freeShipment { get; set; }
    //    public string startTkRate { get; set; }
    //    public string endTkRate { get; set; }
    //    public string startTkTotalSales { get; set; }
    //    public string startPrice { get; set; }
    //    public string endPrice { get; set; }
    //    public string startRatesum { get; set; }
    //    public string endRatesum { get; set; }
    //    public string startQuantity { get; set; }
    //    public string startBiz30day { get; set; }
    //    public string startPayUv30 { get; set; }
    //    public string hPayRate30 { get; set; }
    //    public string hGoodRate { get; set; }
    //    public string jhs { get; set; }
    //    public string lRfdRate { get; set; }
    //    public string startSpay30 { get; set; }
    //    public string hSellerGoodrat { get; set; }
    //    public string hSpayRate30 { get; set; }
    //    public string subOeRule { get; set; }
    //    public string auctionTagRaw { get; set; }
    //    public string startRlRate { get; set; }
    //    public string shopTag { get; set; }
    //    public string npxType { get; set; }
    //    public string picQuality { get; set; }
    //    public string selectedNavigator { get; set; }
    //    public string typeTagName { get; set; }
    //}
    //public class paginator
    //{
    //    public int? length { get; set; }
    //    public int? offset { get; set; }
    //    public int? page { get; set; }
    //    public int? beginIndex { get; set; }
    //    public int? endIndex { get; set; }
    //    public int? items { get; set; }
    //    public int? lastPage { get; set; }
    //    public int? itemsPerPage { get; set; }
    //    public int? previousPage { get; set; }
    //    public int? nextPage { get; set; }
    //    public int? pages { get; set; }
    //    public int? firstPage { get; set; }
    //    public IEnumerable<int?> slider { get; set; }
    //}
    //public class tkSpecialCampaignIdRateMap
    //{
    //    public string _28623940 { get; set; }
    //    public string _28929421 { get; set; }
    //    public string _28929471 { get; set; }
    //    public string _45332944 { get; set; }
    //    public string _63960348 { get; set; }
    //}



}
