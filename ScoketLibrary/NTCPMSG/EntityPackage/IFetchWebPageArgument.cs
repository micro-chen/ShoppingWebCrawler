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
        /// 关键词
        /// </summary>
        string KeyWord { get; set; }

        /// <summary>
        /// 页码
        /// </summary>

        int PageNumber { get; set; }

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
        KeyWordTagGroup Tags { get; set; }

        /// <summary>
        /// 获取当前平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        List<OrderField> GetCurrentPlatformSupportOrderFields();
    }
}