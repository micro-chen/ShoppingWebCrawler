using System;
using System.Collections.Generic;

namespace NTCPMessage.EntityPackage.Arguments
{

    /// <summary>
    /// 抓取网页需要的基础参数模型
    /// TODO：第一阶段只是按照各个平台官网的排序进行排序，筛选勾选字段 暂时没有实现
    /// </summary>
    public abstract class BaseFetchWebPageArgument : IFetchWebPageArgument
    {

        /// <summary>
        /// 查询关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 页码
        /// </summary>

        public int PageNumber { get; set; }

        /// <summary>
        /// 排序字段集合
        /// </summary>
        public List<OrderField> OrderFiledList { get; set; }

        /// <summary>
        /// 需要过滤的字段集合 对于勾选的字段  传递过来
        /// </summary>
        public List<FilterFiled> FilterFiledList { get; set; }



        /// <summary>
        /// 获取当前平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public abstract List<OrderField> GetCurrentPlatformSupportOrderFields();

    }
}
