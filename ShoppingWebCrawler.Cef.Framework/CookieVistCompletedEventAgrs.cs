using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Cef.Framework
{
    public class CookieVistCompletedEventAgrs : EventArgs
    {
        #region 属性
        /// <summary>
        /// 域名
        /// </summary>
        public string DomainName { get; set; }
        //指向cookie结果集合的引用
        public IList<CefCookie> Results;


        #endregion

    }
}
