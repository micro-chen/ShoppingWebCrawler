using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace ShoppingWebCrawler.Host
{
    /// <summary>
    /// 表单参数类型
    /// </summary>
    public class FormDataType
    {
        public static readonly string StandForm = "application/x-www-form-urlencoded";
        public static readonly string Json = "application/json";
        public static readonly string Xml = "application/xml";
    }


    /// <summary>
    /// 发送http 请求完毕的接受参数
    /// </summary>
    public class HTTPRequestCompletedAgrs : EventArgs
    {
        public string URL { get; set; }
        public string Content { get; set; }

        public Exception Error { get; set; }
    }


    public class HttpClassicClientHelper
    {


        /// <summary>
        /// 共享的CookieContainer
        /// </summary>
        private CookieContainer GlobleCookieContainer = new CookieContainer();



        //模拟客户端浏览器头部信息
        private readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";

        /// <summary>
        /// 网址匹配
        /// </summary>
        private readonly string PatternUrl = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";





        /// <summary>
        /// 发送异步GET请求完毕事件
        /// </summary>
        public event EventHandler<HTTPRequestCompletedAgrs> OnGetUrlRequestCompleted;


        /// <summary>
        /// 发送异步POST请求完毕事件
        /// </summary>
        public event EventHandler<HTTPRequestCompletedAgrs> OnPostUrlRequestCompleted;


        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        public string CreateGetHttpResponse(string url, Encoding encode)
        {

            string result = null;
            var response = this.CreateGetHttpResponse(url,"", 10000, null);
            if (null != response)
            {
                Stream answer = response.GetResponseStream();
                // var encode=Encoding.GetEncoding("utf-8");
                if (null == encode)
                {
                    encode = Encoding.UTF8;
                }
                StreamReader answerData = new StreamReader(answer, encode);
                result = answerData.ReadToEnd();


                //及时释放资源

                answer.Dispose();
                answerData.Dispose();
            }

            return result;
        }
        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public HttpWebResponse CreateGetHttpResponse(string url,string tranf="", int? timeout = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }


            //url = HttpUtility.HtmlEncode(url);


            //获取域名
            string domainName = GetDomainName(url);


            HttpWebRequest request = null;// WebRequest.Create(url) as HttpWebRequest;


            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }



            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.CookieContainer = GlobleCookieContainer;//设定为共享Cookie容器
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.KeepAlive = true;
            request.Timeout = 5000;
            request.Host = "s.click.taobao.com";
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            if (!string.IsNullOrEmpty(tranf))
            {
                request.Referer = tranf;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                ChangeGlobleCookies(cookies, domainName);
            }


            try
            {


                var response = request.GetResponse() as HttpWebResponse;
                return response;
            }
            catch (Exception ex)
            {
                if (null != this.OnPostUrlRequestCompleted)
                {
                    var args = new HTTPRequestCompletedAgrs()
                    {
                        URL = url,
                        Content = null,
                        Error = ex
                    };
                    this.OnPostUrlRequestCompleted.Invoke(this, args);
                }

                return null;

            }

        }




        /// <summary>
        /// 发送异步的HTTP -GET请求
        /// </summary>
        /// <param name="url"></param>

        /// <param name="timeout"></param>
        /// <param name="cookies"></param>
        public void CreateGetAsync(string url, int? timeout, CookieCollection cookies = null)
        {


            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);


            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);

            webReq.Method = "GET";
            //webReq.ContentType = "text/html;charset=utf-8";
            webReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webReq.UserAgent = DefaultUserAgent;
            webReq.AllowAutoRedirect = true;
            webReq.KeepAlive = true;
            webReq.CookieContainer = GlobleCookieContainer;
            webReq.Timeout = 3000;


            if (timeout.HasValue)
            {
                webReq.Timeout = timeout.Value;
            }



            try
            {

                //开启异步请求
                IAsyncResult result = webReq.BeginGetResponse(new AsyncCallback(EndGetResponse), webReq);

            }
            catch (Exception ex)
            {
                //未能连接请求  那么通知事件  已经触发，返回结果为空
                if (null != this.OnGetUrlRequestCompleted)
                {
                    var agrs = new HTTPRequestCompletedAgrs { URL = url, Content = null, Error = ex };
                    this.OnGetUrlRequestCompleted.Invoke(this, agrs);
                }
            }


        }


        private void EndGetResponse(IAsyncResult result)
        {
            //结束异步请求，获取结果
            HttpWebRequest webRequest = null;
            WebResponse webResponse = null;
            string url = null;
            try
            {

                webRequest = (HttpWebRequest)result.AsyncState;
                webResponse = webRequest.EndGetResponse(result);

                //把输出结果转化
                Stream answer = webResponse.GetResponseStream();
                // var encode=Encoding.GetEncoding("utf-8");
                var encode = Encoding.UTF8;
                StreamReader answerData = new StreamReader(answer, encode);


                //触发完毕事件
                url = webRequest.RequestUri.AbsoluteUri;
                string responseData = answerData.ReadToEnd();
                if (null != this.OnGetUrlRequestCompleted)
                {
                    var agrs = new HTTPRequestCompletedAgrs { URL = url, Content = responseData };
                    this.OnGetUrlRequestCompleted.Invoke(this, agrs);
                }
                //及时释放资源

                answer.Dispose();
                answerData.Dispose();

            }
            catch (Exception ex)
            {
                if (null != this.OnGetUrlRequestCompleted)
                {
                    var agrs = new HTTPRequestCompletedAgrs { URL = url, Content = null, Error = ex };
                    this.OnGetUrlRequestCompleted.Invoke(this, agrs);
                }
            }
            finally
            {
                if (null != webResponse)
                {
                    webResponse.Close();
                }
            }

        }





        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public string CreatePostHttpResponse(string url, string contentType, IDictionary<string, string> parameters, Encoding encode)
        {

            string result = null;
            var response = this.CreatePostHttpResponse(url, contentType, parameters, 10000, encode, null);
            if (null != response)
            {
                Stream answer = response.GetResponseStream();
                if (null == encode)
                {
                    encode = Encoding.UTF8;
                }
                StreamReader answerData = new StreamReader(answer, encode);
                result = answerData.ReadToEnd();


                //及时释放资源

                answer.Dispose();
                answerData.Dispose();
            }

            return result;
        }


        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="contentType">请求的内容类型</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public HttpWebResponse CreatePostHttpResponse(string url, string contentType, IDictionary<string, string> parameters, int? timeout = null, Encoding requestEncoding = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }


            //获取域名
            string domainName = GetDomainName(url);

            //设置默认的编码 UFT-8
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            HttpWebRequest request = null;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = contentType;
            request.CookieContainer = GlobleCookieContainer;
            request.UserAgent = DefaultUserAgent;





            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                ChangeGlobleCookies(cookies, domainName);
            }

            //如果需要POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                if (contentType == FormDataType.Json)
                {
                    // StringBuilder buffer = new StringBuilder();


                    //  buffer.AppendFormat("{0}", parameters.First().Value);//json  必须是一个整体的包结构
                    //传递Json 数据包 必须是  UTF-8编码。。。javascript 设计如此
                    requestEncoding = Encoding.UTF8;

                    ///json  必须是一个整体的包结构,而且不要 转码
                    string paraBag = parameters.First().Value;

                    byte[] data = requestEncoding.GetBytes(paraBag);//requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                }
                else if (contentType == FormDataType.StandForm)
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(parameters[key]));
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, HttpUtility.UrlEncode(parameters[key]));
                        }
                        i++;
                    }
                    byte[] data = requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }

                }

            }
            var response = request.GetResponse() as HttpWebResponse;

            return response;
        }



        /// <summary>  
        /// 创建POST方式的HTTP请求 ，基于异步的请求 
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="contentType">请求的内容类型</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public void CreatePostHttpResponseAsync(string url, string contentType, IDictionary<string, string> parameters, int? timeout = null, Encoding requestEncoding = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }


            //获取域名
            string domainName = GetDomainName(url);

            //设置默认的编码 UFT-8
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            HttpWebRequest request = null;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = contentType;
            request.CookieContainer = GlobleCookieContainer;
            request.UserAgent = DefaultUserAgent;





            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                ChangeGlobleCookies(cookies, domainName);
            }

            //如果需要POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                if (contentType == FormDataType.Json)
                {
                    // StringBuilder buffer = new StringBuilder();


                    //  buffer.AppendFormat("{0}", parameters.First().Value);//json  必须是一个整体的包结构
                    //传递Json 数据包 必须是  UTF-8编码。。。javascript 设计如此
                    requestEncoding = Encoding.UTF8;

                    ///json  必须是一个整体的包结构,而且不要 转码
                    string paraBag = parameters.First().Value;

                    byte[] data = requestEncoding.GetBytes(paraBag);//requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                }
                else if (contentType == FormDataType.StandForm)
                {

                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(parameters[key]));
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, HttpUtility.UrlEncode(parameters[key]));
                        }
                        i++;
                    }
                    byte[] data = requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                }


            }



            try
            {

                //开启异步请求
                IAsyncResult result = request.BeginGetResponse(new AsyncCallback(EndPostResponse), request);

            }
            catch (Exception ex)
            {
                //未能连接请求  那么通知事件  已经触发，返回结果为空
                if (null != this.OnPostUrlRequestCompleted)
                {
                    var agrs = new HTTPRequestCompletedAgrs { URL = url, Content = null, Error = ex };
                    this.OnPostUrlRequestCompleted.Invoke(this, agrs);
                }
            }



        }


        private void EndPostResponse(IAsyncResult result)
        {
            //结束异步请求，获取结果
            HttpWebRequest webRequest = null;
            WebResponse webResponse = null;
            string url = null;
            try
            {

                webRequest = (HttpWebRequest)result.AsyncState;
                webResponse = webRequest.EndGetResponse(result);

                //把输出结果转化
                Stream answer = webResponse.GetResponseStream();
                // var encode=Encoding.GetEncoding("utf-8");
                var encode = Encoding.UTF8;
                StreamReader answerData = new StreamReader(answer, encode);


                //触发完毕事件
                url = webRequest.RequestUri.AbsoluteUri;
                string responseData = answerData.ReadToEnd();
                if (null != this.OnPostUrlRequestCompleted)
                {
                    var agrs = new HTTPRequestCompletedAgrs { URL = url, Content = responseData };
                    this.OnPostUrlRequestCompleted.Invoke(this, agrs);
                }
                //及时释放资源

                answer.Dispose();
                answerData.Dispose();

            }
            catch (Exception ex)
            {
                if (null != this.OnPostUrlRequestCompleted)
                {
                    var agrs = new HTTPRequestCompletedAgrs { URL = url, Content = null, Error = ex };
                    this.OnPostUrlRequestCompleted.Invoke(this, agrs);
                }
            }
            finally
            {
                if (null != webResponse)
                {
                    webResponse.Close();
                }
            }

        }




        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        /// <summary>
        /// 获取URL的域名 带传输协议 HTTP HTTPS......(不带端口)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetDomainName(string url)
        {
            string domainName = "";//域名


            if (string.IsNullOrEmpty(url))
            {
                return domainName;
            }
            var match = Regex.Match(url, PatternUrl, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (null != match)
            {
                var gs = match.Groups;
                if (gs != null && gs.Count > 3)
                {
                    string portal = gs[1].Value;//传输 协议
                    string domain = gs[3].Value;//上述表达式 分组后 在索引的3位置

                    domainName = string.Concat(portal, domain);
                }
            }

            return domainName;
        }


        /// <summary>
        /// 更新对应域名下的Cookie设定
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="domainName"></param>
        private void ChangeGlobleCookies(CookieCollection cookies, string domainName)
        {
            //便利集合 ，重置对应的键值对
            foreach (Cookie item in cookies)
            {
                var key = item.Name;
                if (null != GlobleCookieContainer && !string.IsNullOrEmpty(domainName))
                {
                    //获取当前域名下的Cookies 
                    var currentDomainCookies = GlobleCookieContainer.GetCookies(new Uri(domainName));
                    //重置对应的集合下的同名的Cookie
                    if (null != currentDomainCookies[key])
                    {
                        currentDomainCookies[key].Value = item.Value;
                    }
                    else
                    {
                        GlobleCookieContainer.Add(item);//不存在此键值对  那么追加到集合中--注意，此处仅仅添加了cookie实例的引用，并不是一个实例对象的副本
                    }

                }
            }
        }






    }
}