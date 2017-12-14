using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*查询指定的关键词后，会得到查询页面
1 品牌
2 高级选项，我们对这些高级选项进行合并匹配，能匹配的，合并到一个组，不能合并的，成为单一的tag组
3 排序
*/
namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 标签组
    /// </summary>
    public class KeyWordTagGroup
    {
        public string _GroupName;
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName
        {
            get
            {
                if (null != this.Tags && this.Tags.Count > 0)
                {
                    //有一组同名的tag集合
                    _GroupName = this.Tags[0].TagName;

                }
                return this._GroupName;
            }
        }

        /// <summary>
        /// 包含同名的的标签集合
        /// </summary>
        public List<KeyWordTag> Tags { get; set; }
    }
}
