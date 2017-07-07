using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host.UI
{
    public  class BaseForm: Form
    {

        public BaseForm()
        {
        }



        /// <summary>
        /// 归属的域
        /// </summary>
        public string DomainIdentity { get; set; }

        protected IEnumerable<CefCookie> _PageCooies;

        /// <summary>
        /// 加载Cookies
        /// </summary>
        protected void LoadCookies()
        {
            if (null == this.DomainIdentity || this.DomainIdentity.Length <= 0)
            {
                return;
            }

            GlobalContext.OnInvokeProcessDomainCookies(this.DomainIdentity, (currentDomainCookies) =>
            {
                _PageCooies = currentDomainCookies;
            });

        }

        /// <summary>
        /// 加载Cookies 完毕并执行特定的回调
        /// </summary>
        /// <param name="callBackHandler"></param>
        protected void LoadCookies(Action<IEnumerable<CefCookie>> callBackHandler)
        {
            if (null == this.DomainIdentity || this.DomainIdentity.Length <= 0)
            {
                return;
            }

            GlobalContext.OnInvokeProcessDomainCookies(this.DomainIdentity, callBackHandler);

        }


    }
}
