using NTCPMessage.EntityPackage.Arguments;
using System.Collections.Generic;

namespace NTCPMessage.EntityPackage
{

    /// <summary>
    /// 抓取搜索网页的参数接口
    /// </summary>
    public interface IFetchWebPageArgument
    {

        SupportPlatformEnum Platform { get; set; }

        /// <summary>
        /// 是否需要解析 品牌、类别、规格内容
        /// </summary>
        bool IsNeedResolveHeaderTags { get; set; }
        /// <summary>
        /// 已经解析完毕的搜索页面地址
        /// 有的平台搜索url 参数需要频繁变更，所以，需要在插件中解析地址
        /// 但是有的平台需要具体的参数；所以传递的时候 先尝试在site 的插件中进行解析地址
        /// </summary>
        ResolvedSearchUrlWithParas ResolvedUrl { get; set; }

        /// <summary>
        /// 品牌集合
        /// </summary>
         List<BrandTag> Brands { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        string KeyWord { get; set; }

        /// <summary>
        /// 页码
        /// </summary>

        int PageIndex { get; set; }

        /// <summary>
        /// 支持的排序字段名
        /// </summary>
        string OrderFiledName { get; }
        /// <summary>
        /// 选择的排序字段值
        /// </summary>
        OrderField OrderFiled { get; set; }


        /// <summary>
        /// 高级筛选-选中的tag标签
        /// </summary>
        KeyWordTagGroup TagGroup { get; set; }

        /// <summary>
        /// 获取当前平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        List<OrderField> GetCurrentPlatformSupportOrderFields();

        /// <summary>
        /// 附加参数字典
        /// </summary>
        Dictionary<string,object> AttachParas { get; set; }
    }
}