using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 唯品会查询品牌参数
    /// </summary>
    public class VipSearchParaBrand : VipSearchPara
    {
        public VipSearchParaBrand(string keyWord) : base(keyWord)
        {
            this.id = 1513762611804;
            this.method = "SearchRpc.getBrandStoreList";

        }

    }
    /// <summary>
    /// 唯品会查询分类参数
    /// </summary>
    public class VipSearchParaCategoryTree : VipSearchPara
    {
        public VipSearchParaCategoryTree(string keyWord) : base(keyWord)
        {
            this.id = 1513765088375;
            this.method = "SearchRpc.getCategoryTree";


        }
    }
    /// <summary>
    /// 唯品会检索商品参数
    /// </summary>
    public class VipSearchParaSearchList
    {
        public VipSearchParaSearchList(string keyWord)
        {
            this.id = 1513765088377;
            this.method = "SearchRpc.getSearchList";
            this.jsonrpc = "2.0";

            this.paramsDetails = new SearchDetails();
            this.paramsDetails.keyword = keyWord;

        }

        public string method { get; set; }

        [JsonProperty("params")]
        public SearchDetails paramsDetails { get; set; }
        public long id { get; set; }
        public string jsonrpc { get; set; }


        public class SearchDetails: ParaDetails
        {
            public SearchDetails()
            {
                this.ep = 20;
            }
            public string channel_id { get; set; }
            public int np { get; set; }
            public int ep { get; set; }
            public int sort { get; set; }
            public string sizeNames { get; set; }
        }

    }

    public class VipSearchPara
    {
        public VipSearchPara(string keyWord)
        {
            this.jsonrpc = "2.0";
            this.paramsDetails = new ParaDetails();
            this.paramsDetails.keyword = keyWord;
        }

        public string method { get; set; }

        [JsonProperty("params")]
        public ParaDetails paramsDetails { get; set; }
        public long id { get; set; }
        public string jsonrpc { get; set; }

    }



    public class ParaDetails
    {

        public ParaDetails()
        {
            this.page = "searchlist.html";

        }
        public string page { get; set; }
        public string keyword { get; set; }
        public string brand_ids { get; set; }
        public string brand_store_sn { get; set; }
        public string props { get; set; }
        public string price_start { get; set; }
        public string price_end { get; set; }
        public string category_id_1_show { get; set; }
        public string category_id_1_5_show { get; set; }
        public string category_id_2_show { get; set; }
        public string category_id_3_show { get; set; }
        public string minPrice { get; set; }
        public string maxPrice { get; set; }
        public string query
        {
            get
            {
                return string.Format("q={0}&channel_id=", this.keyword);
            }
        }
    }

}
