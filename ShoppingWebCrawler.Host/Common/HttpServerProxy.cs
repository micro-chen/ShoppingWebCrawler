using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ShoppingWebCrawler.Host.Http
{

    /// <summary>
    /// post请求数据类型
    /// </summary>
    public enum PostDataContentType
    {
        /// <summary>
        /// 表单键值对
        /// </summary>
        Form,
        /// <summary>
        /// json 数据体
        /// </summary>
        Json,
        /// <summary>
        /// 二进制数据
        /// </summary>
        Binary
    }


    /// <summary>
    /// 将HttpClient的  Cookies容器暴露
    /// </summary>
    public class CookedHttpClient
    {
        public HttpClient Client { get; set; }

        /// <summary>
        /// 暴露的HttpClinet 实例的Cookies
        /// 智能修改 不能重新定义实例-因为这个实例容器绑定到了HttpClient的Handler的引用
        /// </summary>
        public CookieContainer Cookies { get; private set; }


        private HttpClientHandler _clientHander;
        public CookedHttpClient() : this(5000)
        {
        }
        public CookedHttpClient(int timeOut)
        {

            //保持 Cookie 容器 跟httpclient  之间的引用关系
            this.Cookies = new CookieContainer();

            this._clientHander = new HttpClientHandler() { CookieContainer = this.Cookies, AutomaticDecompression = DecompressionMethods.GZip, AllowAutoRedirect = true };
            this.Client = new HttpClient(_clientHander);
            this.Client.Timeout = TimeSpan.FromMilliseconds(5000);
            this.Client.MaxResponseContentBufferSize = 256000;
        }


        /// <summary>
        /// 更新对应域名下的Cookie设定
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="domainName"></param>
        public void ChangeGlobleCookies(List<Cookie> cookies, string domainName)
        {
            if (null == cookies || string.IsNullOrEmpty(domainName))
            {
                return;
            }
            //便利集合 ，重置对应的键值对
            foreach (Cookie item in cookies)
            {
                var key = item.Name;
                if (null != this.Cookies && !string.IsNullOrEmpty(domainName))
                {
                    //获取当前域名下的Cookies 
                    var currentDomainCookies = this.Cookies.GetCookies(new Uri(domainName));
                    //重置对应的集合下的同名的Cookie
                    if (null != currentDomainCookies[key])
                    {
                        currentDomainCookies[key].Value = item.Value;
                    }
                    else
                    {
                        this.Cookies.Add(item);//不存在此键值对  那么追加到集合中--注意，此处仅仅添加了cookie实例的引用，并不是一个实例对象的副本
                    }

                }
            }
        }

    }

    /// <summary>
    /// 服务端  请求转发代理类
    /// 将客户端的请求 转发到指定的地址，进行ISV数据接入
    /// </summary>
    public class HttpServerProxy : IDisposable
    {



        #region 字段+属性

        /// <summary>
        /// https
        /// </summary>
        public const string HttpSchemaOfHttps = "https";
        //UA标识键
        public const string RequestHeaderKeyUserAgent = "User-Agent";
        /// <summary>
        /// cookies 容器
        /// </summary>
        public CookieContainer Cookies { get; set; }

        /// <summary>
        /// Web客户端
        /// </summary>
        public HttpClient Client { get; set; }

        /// <summary>
        /// 是否保持client 的激活
        /// </summary>
        public bool KeepAlive { get; set; }

        #endregion

        #region 私有方法

        public HttpServerProxy()
        {
            this.KeepAlive = false;
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }

        /// <summary>
        /// 更改请求的UA标识的扩展方法
        /// </summary>
        public static void ChangeHttpClientUserAgent(HttpClient client, string userAgent)
        {
            if (!string.IsNullOrEmpty(userAgent) && null != client)
            {
                client.DefaultRequestHeaders.Remove(HttpServerProxy.RequestHeaderKeyUserAgent);
                client.DefaultRequestHeaders.Add(HttpServerProxy.RequestHeaderKeyUserAgent, userAgent);
            }
        }

        ///// <summary>
        ///// 判断是否为 gzip 请求
        ///// 已经在请求的时候 自动处理             this._clientHander = new HttpClientHandler() { CookieContainer = this.Cookies, AutomaticDecompression = DecompressionMethods.GZip };
        ///// </summary>
        ///// <param name="headers"></param>
        ///// <returns></returns>
        //private bool IsGzipRequest(HttpRequestHeaders headers) {
        //    if (null != headers.AcceptEncoding && headers.AcceptEncoding.Any(x => x.Value.Contains("gzip")))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        /// <summary>
        /// 对传递过来的 数据进行初始化
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        private HttpContent InitHttpContent(HttpContent content, PostDataContentType dataType, Dictionary<string, string> data)
        {
            //处理需要提交的数据
            if (null != data && data.Count > 0)
            {
                switch (dataType)
                {
                    case PostDataContentType.Form:
                        //直接将字典数据作为参数体
                        content = new FormUrlEncodedContent(data);
                        break;
                    case PostDataContentType.Json:
                        //json数据 直接将首个数据作为传递数据 并进行转码
                        var realData = data.First().Value;
                        // string decodeBody = HttpUtility.UrlEncode(realData);
                        content = new StringContent(realData, Encoding.UTF8, "application/json");
                        //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        break;
                    case PostDataContentType.Binary:
                        throw new NotImplementedException("未实现指定的功能.....");
                    default:
                        break;
                }

            }
            return content;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HttpClient CreateNewHttpClient()
        {
            HttpClient client = null;
            HttpClientHandler handler = null;
            if (null != this.Cookies)
            {
                handler = new HttpClientHandler() { CookieContainer = this.Cookies };
            }
            if (null != handler)
            {
                client = new HttpClient(handler);
            }
            else
            {
                client = new HttpClient();
            }
            //设定最大的容积量 防止溢出
            client.MaxResponseContentBufferSize = 256000;

            return client;
        }

        /// <summary>
        /// 格式化转发请求的头部信息
        /// 现在只转发 验证信息 其他不转发
        /// </summary>
        /// <param name="reqHeaders"></param>
        /// <param name="fromHeaders"></param>
        public static void FormatRequestHeader(HttpRequestHeaders reqHeaders, NameValueCollection fromHeaders)
        {
            if (null == fromHeaders || fromHeaders.Count <= 0)
            {
                return;
            }
            foreach (var key in fromHeaders.AllKeys)
            {
                var value = fromHeaders[key];
                reqHeaders.Add(key, value);
            }

        }
        #endregion


        #region GET  请求转发处理



        /// <summary>
        /// get 请求转发
        /// 同步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fromHeaders"></param>
        /// <returns></returns>
        public string GetRequestTransfer(string url, NameValueCollection fromHeaders)
        {
            string result = string.Empty;
            try
            {

                var tskResponse = this.GetResponseTransferAsync(url, fromHeaders);
                if (null == tskResponse)
                {
                    return string.Empty;
                }
                //等待 task执行完毕 返回结果
                result = tskResponse.Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {

                Logging.Logger.WriteException(ex);
            }

            return result;

        }


        /// <summary>
        /// 大文件请求连续下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fromHeaders"></param>
        /// <returns></returns>
        public async Task GetHttpForTransferEncodingChunkedReponse(string url, NameValueCollection fromHeaders)
        {
            if (null == this.Client)
            {
                this.Client = this.CreateNewHttpClient();
            }

            //修改请求对象的头信息
            FormatRequestHeader(this.Client.DefaultRequestHeaders, fromHeaders);


            var targetUri = new Uri(url);
            if (targetUri.Scheme == HttpSchemaOfHttps)
            {
                //开启 https 默认证书验证
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }


            try
            {


                using (HttpResponseMessage response = await this.Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    string fileToWriteTo = Path.GetTempFileName();
                    using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }
                }

                #region 废弃



                //using (HttpResponseMessage response = await this.Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                //using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                //{

                //    using (MemoryStream streamOfBuffer = new MemoryStream())
                //    {
                //        await streamToReadFrom.CopyToAsync(streamOfBuffer);


                //        streamOfBuffer.Seek(0, SeekOrigin.Begin);
                //        using (var reader = new StreamReader(streamOfBuffer, Encoding.GetEncoding("gb2312"), false, 1024))
                //        {
                //            while (-1 != reader.Peek())
                //            {
                //                strList.Add(reader.ReadLine());
                //            }
                //        }
                //    }

                //}

                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }




        }

        /// <summary>
        /// get 请求转发
        /// 异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fromHeaders"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetResponseTransferAsync(string url, NameValueCollection fromHeaders)
        {


            try
            {
                if (null == this.Client)
                {
                    this.Client = this.CreateNewHttpClient();
                }

                //修改请求对象的头信息
                FormatRequestHeader(this.Client.DefaultRequestHeaders, fromHeaders);


                var targetUri = new Uri(url);
                if (targetUri.Scheme == HttpSchemaOfHttps)
                {
                    //开启 https 默认证书验证
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }


                Task<HttpResponseMessage> tskResponse;
                try
                {
                    tskResponse = this.Client.GetAsync(targetUri, HttpCompletionOption.ResponseContentRead);
                    if (null == tskResponse || null == tskResponse.Result)
                    {
                        throw new Exception(string.Concat("指定的地址未能正确get响应！uri:", url));
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }



                //普通文本请求
                return tskResponse;
                //return client.GetStringAsync(targetUri);

            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);
            }


            return null;

        }


        #endregion


        #region POST  请求转发处理



        /// <summary>
        /// post 请求转发
        /// 同步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string PostRequestTransfer(string url, PostDataContentType dataType, Dictionary<string, string> data, NameValueCollection fromHeaders)
        {
            try
            {
                var tskResponse = this.PostRequestTransferAsync(url, dataType, data, fromHeaders);

                //等待 task执行完毕 返回结果
                return tskResponse.Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// post 请求转发
        /// 异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="fromHeaders"></param>
        /// <returns></returns>
        public Task<string> PostRequestTransferAsync(string url, PostDataContentType dataType, Dictionary<string, string> data, NameValueCollection fromHeaders)
        {

            if (null == this.Client)
            {
                this.Client = this.CreateNewHttpClient();
            }
            //using (var handler = new HttpClientHandler() { CookieContainer = this.Cookies })
            //using (var client = new HttpClient(handler))
            //{
            try
            {


                //修改请求对象的头信息
                FormatRequestHeader(this.Client.DefaultRequestHeaders, fromHeaders);

                //包装提交的数据
                HttpContent content = null;

                content = this.InitHttpContent(content, dataType, data);

                var targetUri = new Uri(url);
                if (targetUri.Scheme == HttpSchemaOfHttps)
                {
                    //开启 https 默认证书验证
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }
                //post 响应  。异步返回内容字符串
                var tskResponse = this.Client.PostAsync(targetUri, content);
                if (tskResponse.Result.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(string.Concat("指定的地址未能正确post响应！uri:", url));
                }
                return tskResponse.Result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }

        }





        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~HttpServerProxy() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            if (this.KeepAlive == false)
            {
                this.Client.Dispose();
            }
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
