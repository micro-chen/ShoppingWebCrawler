using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Net.Http;

namespace System.Web
{
    public static class HttpRequestExtension
    {
        /// <summary>
        /// 将WebApi的  请求上下文对象 转换为 HttpRequestBase
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HttpRequestBase ConvertToHttpRequestBase(this HttpRequestMessage request)
        {
            HttpContextBase context = (HttpContextBase)request.Properties["MS_HttpContext"];//获取传统context
            return context.Request;//定义传统request对象

        }
        #region HttpRequestBase Extension


        public static T GetQuery<T>(this HttpRequestBase request, string name)
        {
            try
            {
                Type t = typeof(T);
                object result = request.QueryString[name];
                switch (t.Name)
                {
                    case "String":
                    case "Object":
                        return (T)result;
                    default:
                        BindingFlags flag = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
                        
                        
                        return (T)t.InvokeMember("Parse", flag, null, null, new object[] { result });
                }
            }
            catch { return default(T); }
        }

        public static T GetForm<T>(this HttpRequestBase request, string name)
        {
            try
            {
                Type t = typeof(T);
                object result = request.Form[name];//request[name]
                switch (t.Name)
                {
                    case "String":
                    case "Object":
                        return (T)result;
                    default:
                        BindingFlags flag = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
                        return (T)t.InvokeMember("Parse", flag, null, null, new object[] { result });
                }
            }
            catch { return default(T); }
       }
        public static int GetFormCheck(this HttpRequestBase request, string name)
        {
            var r = GetForm<string>(request, name);
            if (r.IsNull())
            {
                return 0;
            }
            else
            {
                if (r.ToString().Equals("on"))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public static T GetFormOrDefault<T>(this HttpRequestBase request, string name, T value)
        {
            return GetForm<T>(request, name).IsNull() ? value : GetForm<T>(request, name);
        }

        public static T GetEntity<T>(this HttpRequestBase request)
        {
            string jsonStr = request.GetForm<string>("Entity");
            return jsonStr.FromJson<T>();
        }


        public static List<T> GetList<T>(this HttpRequestBase request)
        {
            string jsonStr = request.GetForm<string>("List");
            return jsonStr.FromJson<List<T>>();
        }

        public static string GetIP(this HttpRequestBase request)
        {
            string Ip = "127.0.0.1";
            string localLoopIp = "127.0.0.1";
            if (request.ServerVariables["HTTP_VIA"] != null)
            {
                if (request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
                {
                    if (request.ServerVariables["HTTP_CLIENT_IP"] != null)
                        Ip = request.ServerVariables["HTTP_CLIENT_IP"].ToString();
                    else
                        if (request.ServerVariables["REMOTE_ADDR"] != null)
                            Ip = request.ServerVariables["REMOTE_ADDR"].ToString();
                       
                }
                else
                    Ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (request.ServerVariables["REMOTE_ADDR"] != null)
            {
                Ip = request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            if (Ip == "::1")//本地回环Ip地址
            {
                Ip = localLoopIp;
            }
            return Ip;
        }

        #endregion


        #region HttpRequest Extension


        public static T GetQuery<T>(this HttpRequest request, string name)
        {
            try
            {
                Type t = typeof(T);
                object result = request.QueryString[name];
                switch (t.Name)
                {
                    case "String":
                    case "Object":
                        return (T)result;
                    default:
                        BindingFlags flag = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;


                        return (T)t.InvokeMember("Parse", flag, null, null, new object[] { result });
                }
            }
            catch { return default(T); }
        }

        public static T GetForm<T>(this HttpRequest request, string name)
        {
            try
            {
                Type t = typeof(T);
                object result = request.Form[name];//request[name]
                switch (t.Name)
                {
                    case "String":
                    case "Object":
                        return (T)result;
                    default:
                        BindingFlags flag = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
                        return (T)t.InvokeMember("Parse", flag, null, null, new object[] { result });
                }
            }
            catch { return default(T); }
        }
        public static int GetFormCheck(this HttpRequest request, string name)
        {
            var r = GetForm<string>(request, name);
            if (r.IsNull())
            {
                return 0;
            }
            else
            {
                if (r.ToString().Equals("on"))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public static T GetFormOrDefault<T>(this HttpRequest request, string name, T value)
        {
            return GetForm<T>(request, name).IsNull() ? value : GetForm<T>(request, name);
        }

        public static T GetEntity<T>(this HttpRequest request)
        {
            string jsonStr = request.GetForm<string>("Entity");
            return jsonStr.FromJson<T>();
        }


        public static List<T> GetList<T>(this HttpRequest request)
        {
            string jsonStr = request.GetForm<string>("List");
            return jsonStr.FromJson<List<T>>();
        }

        public static string GetIP(this HttpRequest request)
        {
            string Ip = "127.0.0.1";
            string localLoopIp = "127.0.0.1";
            if (request.ServerVariables["HTTP_VIA"] != null)
            {
                if (request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
                {
                    if (request.ServerVariables["HTTP_CLIENT_IP"] != null)
                        Ip = request.ServerVariables["HTTP_CLIENT_IP"].ToString();
                    else
                        if (request.ServerVariables["REMOTE_ADDR"] != null)
                        Ip = request.ServerVariables["REMOTE_ADDR"].ToString();

                }
                else
                    Ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (request.ServerVariables["REMOTE_ADDR"] != null)
            {
                Ip = request.ServerVariables["REMOTE_ADDR"].ToString();
            }

            if (Ip=="::1")//本地回环Ip地址
            {
                Ip = localLoopIp;
            }
            return Ip;
        }

        #endregion
    }
}
