using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host
{


    public class CookieVistCompletedEventAgrs:EventArgs
    {
        #region 属性

        //指向结果集合的引用
        public IEnumerable<CefCookie> Results;


        #endregion

    }


    public class LazyCookieVistor : CefCookieVisitor
    {

        #region 属性

        private List<CefCookie> _results;

        public List<CefCookie> Results
        {
            get
            {
                if (null == _results)
                {
                    _results = new List<CefCookie>();

                }
                return _results;
            }


        }


        private int _total;
        public int Total
        {
            get
            {
                return _total;
            }

            set
            {
                _total = value;
            }
        }



        #endregion


        //加载完毕后，事件

        public event EventHandler<CookieVistCompletedEventAgrs> VistCookiesCompleted;


        protected void OnVistCookiesCompleted(CookieVistCompletedEventAgrs agrs)
        {
            if (null!=this.VistCookiesCompleted)
            {
                this.VistCookiesCompleted.Invoke(this, agrs);

            }

        }

        protected override bool Visit(CefCookie cookie, int count, int total, out bool delete)
        {
            delete = false;

            this.Total = total;

            if (null!=cookie)
            {
                this.Results.Add(cookie);

            }
            if ((count+1)==total)
            {
                //遍历完毕最后的Cookie后，通知订阅事件
                var agrs = new CookieVistCompletedEventAgrs() { Results = this.Results };
                this.OnVistCookiesCompleted(agrs);
            }


            return true;
        }
    }
}
