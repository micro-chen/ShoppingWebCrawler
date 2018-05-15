using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 解析平台搜索地址对象
    /// </summary>
    public class ResolvedSearchUrlWithParas
    {

        public ResolvedSearchUrlWithParas()
        {
            this.IsNeedPreRequest = true;
        }
        /// <summary>
        /// 已经解析完毕的搜索页面地址
        /// 有的平台搜索url 参数需要频繁变更，所以，需要在插件中解析地址
        /// 但是有的平台需要具体的参数；所以传递的时候 先尝试在site 的插件中进行解析地址
        /// 针对get 请求，在url中直接将参数拼接好
        /// </summary>
       public string Url { get; set; }

        /// <summary>
        /// Post请求附带的参数；
        /// </summary>
        public Dictionary<string,object> ParasPost { get; set; }

        /// <summary>
        /// 是否需要预请求
        /// true 将进行请求url ,false 将进行内容解析的时候，解析地址参数
        /// </summary>
        public bool IsNeedPreRequest { get; set; }
    }
}
