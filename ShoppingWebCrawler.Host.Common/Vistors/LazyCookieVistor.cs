using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShoppingWebCrawler.Cef.Core;
using System.Threading.Tasks;
using System.Net;

namespace ShoppingWebCrawler.Host.Common
{


    public class CookieVistCompletedEventAgrs : EventArgs
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
        private const string __temp_cookie_key_securityGetCookies = "_temp_cef_security_chrome";

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
            if (null != this.VistCookiesCompleted)
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
        /// 注册Cookie到目标站点地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="toRegisterCookies"></param>
        /// <returns></returns>
        public bool SetCookieToCookieManager(string url, List<CefCookie> toRegisterCookies)
        {
            bool result = false;
            if (string.IsNullOrEmpty(url) || toRegisterCookies == null)
            {
                return result;
            }

            try
            {
                var ckManager = GlobalContext.DefaultCEFGlobalCookieManager;
                if (null==ckManager)
                {
                    return false;
                }
                foreach (var item in toRegisterCookies)
                {
                    ckManager.SetCookie(url, item, null);
                }

                result = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return result;
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
        public bool DeleteCookies(string url, string cookieName)
        {
            var result = false;

            try
            {
                var ckManager = GlobalContext.DefaultCEFGlobalCookieManager;
                result = ckManager.DeleteCookies(url, cookieName, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 获取原始的 cefcookie集合
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public IEnumerable<CefCookie> LoadNativCookies(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }


            ///获取异步执行的Task的结果
            IEnumerable<CefCookie> results = this.LoadCookiesAsyc(domain)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return results;
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
            IEnumerable<CefCookie> results = this.LoadCookiesAsyc(domain)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (null == results)
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
            //为了安全获取 首先插入一个临时无效的cookie,否则在没有访问页面cookie的时候会不能正确出发 visit 委托
            var tempCookie = new List<CefCookie> {
                new CefCookie {
                    Domain =domain.GetUrlCookieDomain(),
                    Name =__temp_cookie_key_securityGetCookies,
                    Value =DateTime.Now.ToString(),
                    Expires =DateTime.Now.AddYears(1),
                    Creation =DateTime.Now ,
                    HttpOnly=false,
                    Path="/",
                   LastAccess=DateTime.Now
                },

            };


            this.SetCookieToCookieManager(domain,tempCookie );

            this._tcs = new TaskCompletionSource<IEnumerable<CefCookie>>();
            //var oldListeners=this.VistCookiesCompleted.GetInvocationList();
            //事件回调
            this.VistCookiesCompleted -= HandlerVistCookiesCompleted;
            this.VistCookiesCompleted += HandlerVistCookiesCompleted;

            var ckManager = GlobalContext.DefaultCEFGlobalCookieManager;

            var canAccess = ckManager.VisitUrlCookies(domain, true, this);
            if (canAccess == false)
            {
                //如果未能获取有效的cookie 从指定的域  那么立即返回空结果
                return Task.FromResult<IEnumerable<CefCookie>>(null);
            }


            return this._tcs.Task;
        }
        /// <summary>
        /// 内部委托接受读取cookies 事件完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlerVistCookiesCompleted(object sender, CookieVistCompletedEventAgrs e)
        {
            if (null != this._tcs)
            {
                this._tcs.TrySetResult(e.Results);
            }
        }


        protected override bool Visit(CefCookie cookie, int count, int total, out bool delete)
        {
            delete = false;

            this.Total = total;

            if (null != cookie)
            {
                if (cookie.Name != __temp_cookie_key_securityGetCookies)
                {
                    this.Results.Add(cookie);
                }

            }
            if ((count + 1) == total)
            {
                //遍历完毕最后的Cookie后，通知订阅事件
                var agrs = new CookieVistCompletedEventAgrs() { Results = this.Results };
                this.OnVistCookiesCompleted(agrs);
            }


            return true;
        }
    }
}
