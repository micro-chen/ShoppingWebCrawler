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
using System.Collections.Specialized;

namespace ShoppingWebCrawler.Host.Common.Http
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


    public class HttpRequestHelper
    {


        /// <summary>
        /// 共享的CookieContainer
        /// </summary>
        private CookieContainer GlobleCookieContainer = new CookieContainer();


        //模拟客户端浏览器头部信息
        private readonly string DefaultUserAgent = GlobalContext.ChromeUserAgent;

        /// <summary>
        /// 网址匹配
        /// </summary>
        private readonly string PatternUrl = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";

        /// <summary>
        /// 不能新增的请求头
        /// </summary>
        private readonly string[] NotCustomAddModifyRequestHeaders = new[] {
            "Method",
            "User-Agent",
            "Cookie",
            "Accept",
            "KeepAlive",
            "Timeout",
            "Host",
            "Referer",
            "Host",
            "Connection"
        };



        /// <summary>
        /// 发送异步GET请求完毕事件
        /// </summary>
        public event EventHandler<HTTPRequestCompletedAgrs> OnGetUrlRequestCompleted;


        /// <summary>
        /// 发送异步POST请求完毕事件
        /// </summary>
        public event EventHandler<HTTPRequestCompletedAgrs> OnPostUrlRequestCompleted;


        /// <summary>
        /// 获取域名的顶级域名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string GetTopDomainName(string domain)
        {
            string str = domain;
            if (str.IndexOf(".") > 0)
            {
                string[] strArr = str.Split(':')[0].Split('.');
                if (NumbericExtension.IsNumeric(strArr[strArr.Length - 1]))
                {
                    return str;
                }
                else
                {
                    string domainRules = "||com.cn|net.cn|org.cn|gov.cn|com.hk|公司|中国|网络|com|net|org|int|edu|gov|mil|arpa|Asia|biz|info|name|pro|coop|aero|museum|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cf|cg|ch|ci|ck|cl|cm|cn|co|cq|cr|cu|cv|cx|cy|cz|de|dj|dk|dm|do|dz|ec|ee|eg|eh|es|et|ev|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gh|gi|gl|gm|gn|gp|gr|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|in|io|iq|ir|is|it|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|ml|mm|mn|mo|mp|mq|mr|ms|mt|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nt|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|pt|pw|py|qa|re|ro|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|st|su|sy|sz|tc|td|tf|tg|th|tj|tk|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|va|vc|ve|vg|vn|vu|wf|ws|ye|yu|za|zm|zr|zw|";
                    string tempDomain;
                    if (strArr.Length >= 4)
                    {
                        tempDomain = strArr[strArr.Length - 3] + "." + strArr[strArr.Length - 2] + "." + strArr[strArr.Length - 1];
                        if (domainRules.IndexOf("|" + tempDomain + "|") > 0)
                        {
                            return strArr[strArr.Length - 4] + "." + tempDomain;
                        }
                    }
                    if (strArr.Length >= 3)
                    {
                        tempDomain = strArr[strArr.Length - 2] + "." + strArr[strArr.Length - 1];
                        if (domainRules.IndexOf("|" + tempDomain + "|") > 0)
                        {
                            return strArr[strArr.Length - 3] + "." + tempDomain;
                        }
                    }
                    if (strArr.Length >= 2)
                    {
                        tempDomain = strArr[strArr.Length - 1];
                        if (domainRules.IndexOf("|" + tempDomain + "|") > 0)
                        {
                            return strArr[strArr.Length - 2] + "." + tempDomain;
                        }
                    }
                }
            }
            return str;
        }

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        public string CreateGetHttpResponseContent(string url, NameValueCollection requestHeaders)
        {

            string result = null;
            var response = this.CreateGetHttpResponse(url, requestHeaders, 10000, null);
            if (null != response)
            {
                using (Stream answer = response.GetResponseStream())
                {
                    // var encode=Encoding.GetEncoding("utf-8");
                    Encoding encode = Encoding.UTF8;
                    using (StreamReader answerData = new StreamReader(answer, encode))
                    {
                        result = answerData.ReadToEnd();
                    }


                }

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
        public HttpWebResponse CreateGetHttpResponse(string url, NameValueCollection requestHeaders,int? timeout = 5000, CookieContainer cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

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


            //---------固定的头--begin-----------
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.CookieContainer = cookies;//设定为共享Cookie容器
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            //---------固定的头----end------------

            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.KeepAlive = true;
            request.Timeout = timeout.Value;
            // request.Host = "s.click.taobao.com";
            // request.Headers.Add("Upgrade-Insecure-Requests", "1");

            this.FormarRequestHeaders(request,requestHeaders);

            if (cookies != null)
            {
                request.CookieContainer = cookies;
            }


            try
            {


                var response = request.GetResponse() as HttpWebResponse;
                return response;
            }
            catch (Exception ex)
            {
                if (null != this.OnGetUrlRequestCompleted)
                {
                    var args = new HTTPRequestCompletedAgrs()
                    {
                        URL = url,
                        Content = null,
                        Error = ex
                    };
                    this.OnGetUrlRequestCompleted.Invoke(this, args);
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
        public void CreateGetAsync(string url, NameValueCollection requestHeaders, int? timeout, CookieCollection cookies = null)
        {


            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            //webReq.ContentType = "text/html;charset=utf-8";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.UserAgent = DefaultUserAgent;
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;
            request.CookieContainer = GlobleCookieContainer;
            request.Timeout = 3000;

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            this.FormarRequestHeaders(request, requestHeaders);


            try
            {

                //开启异步请求
                IAsyncResult result = request.BeginGetResponse(new AsyncCallback(EndGetResponse), request);

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
        public string CreatePostHttpResponse(string url, NameValueCollection requestHeaders, string contentType, IDictionary<string, string> parameters, Encoding encode)
        {

            string result = null;
            var response = this.CreatePostHttpResponse(url, requestHeaders,contentType, parameters, 10000, null);
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
        public HttpWebResponse CreatePostHttpResponse(string url, NameValueCollection requestHeaders, string contentType, IDictionary<string, string> parameters, int? timeout = null, CookieContainer cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }


            //获取域名
            string domainName = GetDomainName(url);

            //设置默认的编码 UFT-8
            Encoding requestEncoding = Encoding.UTF8;
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
                request.CookieContainer = cookies;
            }
            this.FormarRequestHeaders(request, requestHeaders);


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
        public void CreatePostHttpResponseAsync(string url, NameValueCollection requestHeaders, string contentType, IDictionary<string, string> parameters, int? timeout = null, CookieContainer cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }


            //获取域名
            string domainName = GetDomainName(url);

            //设置默认的编码 UFT-8
            Encoding requestEncoding = Encoding.UTF8;

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
                request.CookieContainer = cookies;
            }
            this.FormarRequestHeaders(request, requestHeaders);


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
        /// 格式化请求头信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestHeaders"></param>

        private void FormarRequestHeaders(HttpWebRequest request, NameValueCollection requestHeaders)
        {
            if (null != requestHeaders)
            {
                if (!string.IsNullOrEmpty(requestHeaders["Referer"]))
                {
                    request.Referer = requestHeaders["Referer"];
                }
                if (!string.IsNullOrEmpty(requestHeaders["Host"]))
                {
                    request.Host = requestHeaders["Host"];
                }

                //注册自定义的头
                foreach (var key in requestHeaders.AllKeys)
                {
                    if (!this.NotCustomAddModifyRequestHeaders.Contains(key))
                    {
                        string val = requestHeaders[key];
                        request.Headers.Add(key, val);
                    }
                }

            }
        }





    }
}