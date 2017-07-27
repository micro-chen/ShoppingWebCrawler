using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShoppingWebCrawler.Cef.Core;
using System.Threading.Tasks;
using System.Net;

namespace ShoppingWebCrawler.Host.Common
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
        /// 加载Cookies 并返回Cookeie 容器
        /// </summary>
        public CookieContainer LoadCookieContainer(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            var results = this.LoadCookies(domain);

            var ckContainer = new CookieContainer();
            foreach (Cookie item in results)
            {
                ckContainer.Add(item);
            }
            return ckContainer;

        }

        /// <summary>
        /// Delete all cookies that match the specified parameters. 
        ///  If only |url| is specified all host cookies (but not domain cookies) irrespective of path will be deleted.
        /// If both |url| and
        /// |cookie_name| values are specified all host and domain cookies matching
        /// both will be deleted. If |url| is empty all
        /// cookies for all hosts and domains will be deleted. If |callback| is
        /// non-NULL it will be executed asnychronously on the IO thread after the
        /// cookies have been deleted. Returns false if a non-empty invalid URL is
        /// specified or if cookies cannot be accessed. Cookies can alternately be
        /// deleted using the Visit*Cookies() methods.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public bool DeleteCookies(string url,string cookieName) {
            var result = false;

            try
            {
                var ckManager = GlobalContext.DefaultCEFGlobalCookieManager;
                result = ckManager.DeleteCookies(url, cookieName,null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        /// <summary>
        /// 加载Cookies
        /// 加载cefcookie到 CLR 的Cookie对象集合
        /// </summary>
        public List<Cookie> LoadCookies(string domain)
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

            if (null==results)
            {
                return null;
            }

            var lst = new List<Cookie>();
            foreach (CefCookie item in results)
            {
                var name = item.Name;
                var value = item.Value;
                Cookie ck = new Cookie(name, value);
                ck.Domain = item.Domain;
                ck.Path = item.Path;
                ck.HttpOnly = item.HttpOnly;
                ck.Secure = item.Secure;
                
                if (null != item.Expires)
                {
                    ck.Expires = (DateTime)item.Expires;
                }
                //if (DateTime.Now > ck.Expires)
                //{
                //    ck.Expired = true;
                //}
                //ck.Version = 0;//Cookie的格式有2个不同的版本，第一个版本，我们称为Cookie Version 0，是最初由Netscape公司制定的，也被几乎所有的浏览器支持。而较新的版本，Cookie Version 1，则是根据RFC 2109文档制定的。为了确保兼容性，JAVA规定，前面所提到的涉及Cookie的操作都是针对旧版本的Cookie进行的。而新版本的Cookie目前还不被Javax.servlet.http.Cookie包所支持。
                //ck.Discard = false;
                
                lst.Add(ck);
            }
            return lst;
         

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
