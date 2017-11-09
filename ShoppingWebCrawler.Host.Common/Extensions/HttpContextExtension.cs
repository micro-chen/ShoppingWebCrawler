using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web
{
    /// <summary>
    /// 扩展 从http 上下文获取cookie信息
    /// </summary>
    public static class HttpContextExtension
    {
        public static void SetCookie<T>(this HttpContextBase context,string domain, string name, T value) where T : class
        {
            SetCookie<T>(context, domain,name, value, 0);
        }
        public static void SetCookie<T>(this HttpContextBase context, string domain, string name, T value, int day) where T : class
        {
            HttpCookie cookie = context.Request.Cookies[name];            
            cookie = cookie ?? new HttpCookie(name);
            if (day > 0) cookie.Expires = DateTime.Today.AddDays(day);
            cookie.Value = HttpUtility.UrlEncode(value.ToJson());
            cookie.Domain = domain;
            context.Response.Cookies.Add(cookie);
        }
        public static T GetCookie<T>(this HttpContextBase context, string name) where T : class
        {
            HttpCookie cookie = context.Request.Cookies[name];
            if (!cookie.IsNull()) return HttpUtility.UrlDecode(cookie.Value).FromJson<T>();
            return null;
        }
        public static void RemoveCookie(this HttpContextBase context, string domain, string name)
        {
            HttpCookie cookie = context.Request.Cookies[name];
            if (!cookie.IsNull())
            {
                cookie.Domain = domain;
                cookie.Expires = DateTime.Today.AddDays(-1);
                context.Response.Cookies.Add(cookie);
            }
        }


        public static void SetCookie<T>(this HttpContext context, string domain, string name, T value) where T : class
        {
            SetCookie<T>(context,domain, name, value, 0);
        }
        public static void SetCookie<T>(this HttpContext context, string domain, string name, T value, int day) where T : class
        {
            HttpCookie cookie = context.Request.Cookies[name];
            cookie = cookie ?? new HttpCookie(name);
            if (day > 0) cookie.Expires = DateTime.Today.AddDays(day);
            cookie.Value = HttpUtility.UrlEncode(value.ToJson());
            cookie.Domain = domain;
            context.Response.Cookies.Add(cookie);
        }
        public static T GetCookie<T>(this HttpContext context, string name) where T : class
        {
            HttpCookie cookie = context.Request.Cookies[name];
            if (!cookie.IsNull()) return HttpUtility.UrlDecode(cookie.Value).FromJson<T>();
            return null;
        }
        public static void RemoveCookie(this HttpContext context, string domain, string name)
        {
            HttpCookie cookie = context.Request.Cookies[name];
            if (!cookie.IsNull())
            {
                cookie.Domain = domain;
                cookie.Expires = DateTime.Today.AddDays(-1);
                context.Response.Cookies.Add(cookie);
            }
        }

    }
}
