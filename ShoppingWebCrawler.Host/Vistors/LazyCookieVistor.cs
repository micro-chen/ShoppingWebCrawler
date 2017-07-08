using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShoppingWebCrawler.Cef.Core;
using System.Threading.Tasks;

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
        private TaskCompletionSource<IEnumerable<CefCookie>> _tcs = null;
        private EventHandler<CookieVistCompletedEventAgrs> _handler = null;


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



        /// <summary>
        /// 加载Cookies
        /// </summary>
        public IEnumerable<CefCookie> LoadCookies(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            ///获取异步执行的Task的结果
            IEnumerable<CefCookie> results= this.LoadCookiesAsyc(domain)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return results;

        }


        /// <summary>
        /// 返回异步的获取 指定网址的cookies 的Task
        /// </summary>
        /// <param name="domain">指定的网址</param>
        /// <returns></returns>
        public Task<IEnumerable<CefCookie>> LoadCookiesAsyc(string domain)
        {
            this._tcs = new TaskCompletionSource<IEnumerable<CefCookie>>();
            //事件回调
            this._handler = (s, e) =>
            {
                this._tcs.TrySetResult(e.Results);

            };
            this.VistCookiesCompleted += _handler;

            var ckManager = GlobalContext.DefaultCEFGlobalCookieManager;
            ckManager.VisitUrlCookies(domain, true, this);
             
            return this._tcs.Task;
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
