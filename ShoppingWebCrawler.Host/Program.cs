using System;
using System.Collections.Generic;

using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.AppStart;


namespace ShoppingWebCrawler.Host
{ 
    internal static class Program
    {

        private static string ConvertTaobaoKeUrlToCommon(string encryUrl) {

            var schema = "https";
            var qs = encryUrl.Split('?')[encryUrl.Split('?').Length - 1].Split('&');
            var qso = new Dictionary<string, string>();
            for (var i = 0; i < qs.Length; i++)
            {
                if (qs[i] != "")
                {
                    var tmpa = qs[i].Split('=');
                    qso[tmpa[0]] = !string.IsNullOrEmpty(tmpa[1]) ? tmpa[1] : "";
                }
            }

            if (!qso.ContainsKey("tu"))
            {
                throw new Exception(string.Concat("未能转换此加密淘宝客链接：", encryUrl));

            }

            string jump_url=string.Empty;
            if (qso["tu"].IndexOf("https") == 0)
            {
                jump_url = qso["tu"].Substring(5);
            }
            else if (qso["tu"].IndexOf("http") == 0)
            {
                jump_url = qso["tu"].Substring(4);
            }
          

            var jump_address = schema + jump_url;

            var real_jump_address = Microsoft.JScript.GlobalObject.unescape(jump_address);

            return real_jump_address;

        }

        [STAThread]
        private static void Main(string[] args)
        {
            //return InitApp.Init(args);


            string url = "http://s.click.taobao.com/t?spm=1002.8113010.1999451596.1.197829d2jvKq9J&e=m%3D2%26s%3Dxx7h3yvW%2FlwcQipKwQzePOeEDrYVVa64szgHCoaJEBXomhrxaV0k4ZAA5CqNKnVlalBUWfSYtdXqadVuhJq1oW37Sy0WpaHc0S8eIUiNHrwNztF5RF%2BnklwTri0BQMnX1tZRX7Kk0roGkzEdSUwZLhvt%2FrpwP7nD09XRW5e8YPIgsgo%2FaWiDiMYl7w3%2FA2kb";

            var httpHelper = new HttpClassicClientHelper();
            var resp = httpHelper.CreateGetHttpResponse(url);//.// new Http.CookedHttpClient().Client.GetStringAsync(url).Result;

            string tuUrl = resp.ResponseUri.AbsoluteUri;

            string realUrl = ConvertTaobaoKeUrlToCommon(tuUrl);

            var resp2 = httpHelper.CreateGetHttpResponse(realUrl,tuUrl,50000);

           //string content = httpHelper.CreateGetHttpResponse(tuUrl, System.Text.Encoding.UTF8);

            return;



            try
            {
                //1 初始化CEF运行时
                InitApp.Init(args);
                //2 开启无窗口的浏览器 请求平台站点
                var appForm = new HeadLessMainForm();
                appForm.Start();

               

            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);
            }




            //3 开启总控TCP端口，用来接收站点的请求--开启后 会阻塞进程 防止结束
            // 总控端口 负责 1 收集请求 响应请求 2 收集分布的采集客户端 登记注册可用的端，用来做CDN 任务分发，做负载均衡
            RemoteServer.Start();

            var locker = RunningLocker.CreateNewLock();
            locker.Pause();


            // Clean up CEF.
            CefRuntime.Shutdown();
        }
    }
}
