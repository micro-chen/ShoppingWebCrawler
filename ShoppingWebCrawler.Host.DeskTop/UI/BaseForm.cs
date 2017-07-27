
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Common;
using System.Net;

namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    public class BaseForm : Form
    {

        public BaseForm()
        {
        }



        /// <summary>
        /// 归属的域
        /// </summary>
        public string DomainIdentity { get; set; }

        protected IEnumerable<Cookie> _PageCooies;

        /// <summary>
        /// 加载Cookies
        /// </summary>
        protected  void LoadCookies()
        {
            if (null == this.DomainIdentity || this.DomainIdentity.Length <= 0)
            {
                return;
            }

            var ckVistor = new LazyCookieVistor();
            _PageCooies =  ckVistor.LoadCookies(this.DomainIdentity);

        }




    }


}
