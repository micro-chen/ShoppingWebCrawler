using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace NTCPMessage.EntityPackage.Arguments
{

    /// <summary>
    /// 抓取网页需要的基础参数模型
    /// TODO：第一阶段只是按照各个平台官网的排序进行排序，筛选勾选字段 暂时没有实现
    /// </summary>
    public class BaseFetchWebPageArgument : IFetchWebPageArgument
    {

        public BaseFetchWebPageArgument()
        {
            this.IsNeedResolveHeaderTags = true;
            this.Brands = new List<BrandTag>();
            this.TagGroup = new KeyWordTagGroup();
            this.OrderFiled = this.GetCurrentPlatformSupportOrderFields().FirstOrDefault(x => x.Rule == OrderRule.Default);
            if (null == this.OrderFiled)
            {
                this.OrderFiled = new OrderField();
            }

            this.SystemAttachParas = new Dictionary<string, object>();
        }
        /// <summary>
        /// 归属平台
        /// </summary>
        public virtual SupportPlatformEnum Platform { get; set; }

        /// <summary>
        /// 是否需要解析 品牌、类别、规格内容;默认为：true
        /// </summary>
        public bool IsNeedResolveHeaderTags { get; set; }

        /// <summary>
        /// 已经解析完毕的搜索页面地址
        /// 有的平台搜索url 参数需要频繁变更，所以，需要在插件中解析地址
        /// 但是有的平台需要具体的参数；所以传递的时候 先尝试在site 的插件中进行解析地址
        /// </summary>
        public ResolvedSearchUrlWithParas ResolvedUrl { get; set; }

        /// <summary>
        /// 查询关键词
        /// </summary>
        public string KeyWord { get; set; }


        /// <summary>
        /// 品牌集合
        /// </summary>
        public List<BrandTag> Brands { get; set; }

        /// <summary>
        /// 高级筛选-选中的tag标签
        /// </summary>
        public KeyWordTagGroup TagGroup { get; set; }


        /// <summary>
        /// 支持的排序字段名
        /// </summary>
        public virtual string OrderFiledName { get; }

        /// <summary>
        /// 选择的排序字段
        /// </summary>
        public OrderField OrderFiled { get; set; }

        /// <summary>
        /// 价格区间-起始价格
        /// </summary>
        public decimal FromPrice { get; set; }
        /// <summary>
        /// 价格区间-最高价格
        /// </summary>
        public decimal ToPrice { get; set; }


        /// <summary>
        /// 页码-从0开始的页索引
        /// </summary>

        public int PageIndex { get; set; }



        /// <summary>
        /// 获取当前平台支持的排序字段列表
        /// 注意：此为虚方法，继承类可以实现重写
        /// </summary>
        /// <returns></returns>
        public virtual List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            return new List<OrderField>();
        }




        #region 实例的md5哈希值




        private string _MD5Sign;
        [JsonIgnore]
        public string MD5Sign
        {
            get
            {
                if (string.IsNullOrEmpty(this._MD5Sign) || this._MD5Sign.Length < 32)
                {
                    _MD5Sign = this.GetHashMD5SignString();
                }
                return _MD5Sign;
            }
        }

        /// <summary>
        /// 获取当前对象的特殊md5 哈希值;32 位长度小写
        /// </summary>
        /// <returns></returns>
        private string GetHashMD5SignString()
        {
            //拼接筛选条件的核心属性到字符串
            string brandString = string.Join("-", this.Brands.Select(x => x.BrandId ?? ""));
            string tagString = string.Join("-", this.TagGroup.Tags.Select(x => string.Concat(x.FilterFiled ?? "", x.Value ?? "")));
            string conditonString = string.Concat
                (
                this.Platform.ToString(),
                this.IsNeedResolveHeaderTags.ToString(),
                this.KeyWord,
                brandString,
                tagString,
                this.FromPrice,
                this.ToPrice,
                this.OrderFiled.FieldValue,
                this.OrderFiledName,
                this.PageIndex
                );

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var buffer = Encoding.UTF8.GetBytes(conditonString);
            byte[] hash_byte = md5.ComputeHash(buffer);
            string str = System.BitConverter.ToString(hash_byte);
            string md5Sign = str.Replace("-", "").ToLower();
            return md5Sign;
        }

        #endregion

        /// <summary>
        /// 对应的缓存键-结果键
        /// </summary>
        [JsonIgnore]
        public string CacheKey
        {
            get
            {
                return string.Concat("PageCach:", this.Platform.ToString(), ":", this.MD5Sign);
            }
        }

        /// <summary>
        /// 系统运行附带参数
        ///在穿插交互的时候 ，可以附带参数
        /// </summary>
        [JsonIgnore]
        public Dictionary<string,object> SystemAttachParas { get; set; }

        /// <summary>
        /// 是否是合法的参数
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            if (string.IsNullOrEmpty(this.KeyWord))
            {
                return false;
            }

            return true;
        }


    }
}
