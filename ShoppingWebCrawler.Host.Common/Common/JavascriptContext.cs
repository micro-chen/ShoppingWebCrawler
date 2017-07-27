using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Jint;

/*
  //        var engine = new Jint.Engine()
            //            .SetValue("log", new Action<object>(Console.WriteLine))
            //    ;

            //        engine.Execute(@"
            //  function hello() { 
            //    log('Hello World');
            //  };

            //  hello();
            //");

              var t = JavascriptContext.getUnixTimestamp();

            string sign = JavascriptContext.getEtaoJSSDKSign("8a1c196f9c4cc68b00b8e46858cef6d7_1499930011052",
                t,
                "12574478",
              "{\"s\":0,\"n\":40,\"q\":\"植物\",\"needEncode\":false,\"sort\":\"sales_desc\",\"maxPrice\":10000000,\"minPrice\":0,\"serviceList\":\"\",\"navigator\":\"all\",\"urlType\":2}"
                );
          
            Console.WriteLine(sign);
            Console.ReadKey();

            return;
*/

namespace ShoppingWebCrawler.Host.Common
{
    /// <summary>
    /// js 执行上下文
    /// 执行原生的js  脚本
    /// </summary>
    public static class JavascriptContext
    {

        /// <summary>
        /// 获取一淘的sign
        /// </summary>
        /// <param name="_m_h5_tk"> 第一个参数 是服务器产生的私钥-公钥 _m_h5_tk=8a1c196f9c4cc68b00b8e46858cef6d7_1499930011052;  8a开头_结束</param>
        /// <param name="timestnap">时间戳 ,通过js生成13位的时间戳</param>
        /// <param name="appkey">第三个是固定的key ，一淘的是 12574478</param>
        /// <param name="paraData">需要加密的参数字符串 ，比如："{\"s\":0,\"n\":40,\"q\":\"植物\",\"needEncode\":false,\"sort\":\"sales_desc\",\"maxPrice\":10000000,\"minPrice\":0,\"serviceList\":\"\",\"navigator\":\"all\",\"urlType\":2}"</param>
        /// <returns></returns>
        public static string getEtaoJSSDKSign(string _m_h5_tk,string timestamp,string appkey,string paraData)
        {

            //从 一淘官方提取的 js 加密sign 方法 在 https://g.alicdn.com/etao/lego2-pc/0.1.0/app/??libs/crox-min.js,libs/pc-mtop-min.js,libs/include-min.js,libs/popupManager-min.js

            //第一步 从js 文件中搜索 api 找到getSearchList:"mtop.etao.fe.search" 
             //第二部 使用IE 在这里尝试打断点，触发事件 也可以尝试断网，让js 报错 逆向找出请求源
            // 从index.js  中  监听 renderMain:function(a){var t=this,r=e.Defer(),n=r.promise;i({api:g.getSearchList,v:"1.0",data:t.generateRequestParams(),callback:fun  按钮点击排序时间 
            // 使用IE 11    F12进行跟踪断点调试  单步
            // 最后在 app js 
            ///w=function(t){if(t.api){t=S(t,E.params),t.retry=parseInt(t.retry)||0;var e=c(t.data,!1),a=i(),u=r([n.cookie.replace(/.*?(?:\b|;)\s*_m_h5_tk=([^;]+).*$|.*/,"$1").split("_")[0],a,t.appKey,e].join("&")); o("//" + d + "." + g + "." + h + "." + m + "/h5/" + t.api + "/" + t.v + "/",{ type: "jsonp",api: t.api,v: t.v,appKey: t.appKey,data: e,t: a,sign: u},function(n){ n = n ||{ }; var e = (n.ret ||["UNKNOW::未知错误，请检查网络是否正常后重试"])[0], r = n.data ||{ }; if (/^ SUCCESS /.test(e)) "function" == typeof t.callback&& t.callback(r, null, n); else { e = e.split("::"); var o = e[0], a = e[1]; if (/ SESSION_EXPIRED /.test(o)) a = "未登录淘宝账号"; else if (/^ FAIL_SYS | TOKEN_EXOIRED | TOKEN_EMPTY | BIZ_UNAVAILABLE | NO_NETWORK |^ UNKNOW$/.test(o) && !/^ FAIL_SYS_(SERVICE_NOT_EXIST | API_NOT_FOUNDED | ILLEGAL_ACCESS)$/.test(o) && --t.retry >= 0)return E(t),!1; "function" == typeof t.callback&& t.callback(r,{ code: o,msg: a},n)} })}},
             //可以尝试把代码 格式化到 typesctipt 里进行在线代码调试
            //单步调试跟踪到签名算法----下面的是格式化后的

            string getSignScript = @"var r=function(t){function n(t,n){return t<<n|t>>>32-n}function e(t,n){var e,r,o,a,c;return o=2147483648&t,a=2147483648&n,e=1073741824&t,r=1073741824&n,c=(1073741823&t)+(1073741823&n),e&r?2147483648^c^o^a:e|r?1073741824&c?3221225472^c^o^a:1073741824^c^o^a:c^o^a}function r(t,n,e){return t&n|~t&e}function o(t,n,e){return t&e|n&~e}function a(t,n,e){return t^n^e}function c(t,n,e){return n^(t|~e)}function i(t,o,a,c,i,u,l){return t=e(t,e(e(r(o,a,c),i),l)),e(n(t,u),o)}function u(t,r,a,c,i,u,l){return t=e(t,e(e(o(r,a,c),i),l)),e(n(t,u),r)}function l(t,r,o,c,i,u,l){return t=e(t,e(e(a(r,o,c),i),l)),e(n(t,u),r)}function f(t,r,o,a,i,u,l){return t=e(t,e(e(c(r,o,a),i),l)),e(n(t,u),r)}function s(t){for(var n,e=t.length,r=e+8,o=(r-r%64)/64,a=16*(o+1),c=new Array(a-1),i=0,u=0;u<e;)n=(u-u%4)/4,i=u%4*8,c[n]=c[n]|t.charCodeAt(u)<<i,u++;return n=(u-u%4)/4,i=u%4*8,c[n]=c[n]|128<<i,c[a-2]=e<<3,c[a-1]=e>>>29,c}function p(t){var n,e,r='',o='';for(e=0;e<=3;e++)n=t>>>8*e&255,o='0'+n.toString(16),r+=o.substr(o.length-2,2);return r}function d(t){t=t.replace(/\r\n/g,'\n');for(var n='',e=0;e<t.length;e++){var r=t.charCodeAt(e);r<128?n+=String.fromCharCode(r):r>127&&r<2048?(n+=String.fromCharCode(r>>6|192),n+=String.fromCharCode(63&r|128)):(n+=String.fromCharCode(r>>12|224),n+=String.fromCharCode(r>>6&63|128),n+=String.fromCharCode(63&r|128))}return n}var g,h,m,v,b,y,C,S,w,E=[],_=7,I=12,N=17,j=22,A=5,k=9,O=14,L=20,T=4,U=11,K=16,R=23,F=6,$=10,x=15,D=21;for(t=d(t),E=s(t),y=1732584193,C=4023233417,S=2562383102,w=271733878,g=0;g<E.length;g+=16)h=y,m=C,v=S,b=w,y=i(y,C,S,w,E[g+0],_,3614090360),w=i(w,y,C,S,E[g+1],I,3905402710),S=i(S,w,y,C,E[g+2],N,606105819),C=i(C,S,w,y,E[g+3],j,3250441966),y=i(y,C,S,w,E[g+4],_,4118548399),w=i(w,y,C,S,E[g+5],I,1200080426),S=i(S,w,y,C,E[g+6],N,2821735955),C=i(C,S,w,y,E[g+7],j,4249261313),y=i(y,C,S,w,E[g+8],_,1770035416),w=i(w,y,C,S,E[g+9],I,2336552879),S=i(S,w,y,C,E[g+10],N,4294925233),C=i(C,S,w,y,E[g+11],j,2304563134),y=i(y,C,S,w,E[g+12],_,1804603682),w=i(w,y,C,S,E[g+13],I,4254626195),S=i(S,w,y,C,E[g+14],N,2792965006),C=i(C,S,w,y,E[g+15],j,1236535329),y=u(y,C,S,w,E[g+1],A,4129170786),w=u(w,y,C,S,E[g+6],k,3225465664),S=u(S,w,y,C,E[g+11],O,643717713),C=u(C,S,w,y,E[g+0],L,3921069994),y=u(y,C,S,w,E[g+5],A,3593408605),w=u(w,y,C,S,E[g+10],k,38016083),S=u(S,w,y,C,E[g+15],O,3634488961),C=u(C,S,w,y,E[g+4],L,3889429448),y=u(y,C,S,w,E[g+9],A,568446438),w=u(w,y,C,S,E[g+14],k,3275163606),S=u(S,w,y,C,E[g+3],O,4107603335),C=u(C,S,w,y,E[g+8],L,1163531501),y=u(y,C,S,w,E[g+13],A,2850285829),w=u(w,y,C,S,E[g+2],k,4243563512),S=u(S,w,y,C,E[g+7],O,1735328473),C=u(C,S,w,y,E[g+12],L,2368359562),y=l(y,C,S,w,E[g+5],T,4294588738),w=l(w,y,C,S,E[g+8],U,2272392833),S=l(S,w,y,C,E[g+11],K,1839030562),C=l(C,S,w,y,E[g+14],R,4259657740),y=l(y,C,S,w,E[g+1],T,2763975236),w=l(w,y,C,S,E[g+4],U,1272893353),S=l(S,w,y,C,E[g+7],K,4139469664),C=l(C,S,w,y,E[g+10],R,3200236656),y=l(y,C,S,w,E[g+13],T,681279174),w=l(w,y,C,S,E[g+0],U,3936430074),S=l(S,w,y,C,E[g+3],K,3572445317),C=l(C,S,w,y,E[g+6],R,76029189),y=l(y,C,S,w,E[g+9],T,3654602809),w=l(w,y,C,S,E[g+12],U,3873151461),S=l(S,w,y,C,E[g+15],K,530742520),C=l(C,S,w,y,E[g+2],R,3299628645),y=f(y,C,S,w,E[g+0],F,4096336452),w=f(w,y,C,S,E[g+7],$,1126891415),S=f(S,w,y,C,E[g+14],x,2878612391),C=f(C,S,w,y,E[g+5],D,4237533241),y=f(y,C,S,w,E[g+12],F,1700485571),w=f(w,y,C,S,E[g+3],$,2399980690),S=f(S,w,y,C,E[g+10],x,4293915773),C=f(C,S,w,y,E[g+1],D,2240044497),y=f(y,C,S,w,E[g+8],F,1873313359),w=f(w,y,C,S,E[g+15],$,4264355552),S=f(S,w,y,C,E[g+6],x,2734768916),C=f(C,S,w,y,E[g+13],D,1309151649),y=f(y,C,S,w,E[g+4],F,4149444226),w=f(w,y,C,S,E[g+11],$,3174756917),S=f(S,w,y,C,E[g+2],x,718787259),C=f(C,S,w,y,E[g+9],D,3951481745),y=e(y,h),C=e(C,m),S=e(S,v),w=e(w,b);var Y=p(y)+p(C)+p(S)+p(w);return Y.toLowerCase()}";


            var ck = _m_h5_tk.Split('_')[0];//"8a1c196f9c4cc68b00b8e46858cef6d7";
            var t = timestamp;// "1499931938374";////function () { var n = (new Date).getTime(); return n <= t && (n = t + 1), t = n }();
            var key = appkey;// "12574478";
            var paras = paraData;// "{\"s\":0,\"n\":40,\"q\":\"植物\",\"needEncode\":false,\"sort\":\"sales_desc\",\"maxPrice\":10000000,\"minPrice\":0,\"serviceList\":\"\",\"navigator\":\"all\",\"urlType\":2}";

            var paraSring = string.Join("&", ck, t, key, paras);
            var add = new Jint.Engine()
              .Execute(getSignScript)
              .GetValue("r");

            var val = add.Invoke(paraSring);

            return val.AsString();

        }

        /// <summary>
        /// 通过js脚本执行获取时间戳
        /// </summary>
        /// <returns></returns>
        public  static string getUnixTimestamp() {

            string cmd = "function timeToken(){var t=0; var n=(new Date).getTime();return n<=t&&(n=t+1),t=n};";
            var engine = new Jint.Engine().Execute(cmd).GetValue("timeToken");
            string stamp = engine.Invoke().AsNumber().ToString();

            return stamp;
        }

    }

    /// <summary>
    /// 匹配HTML代码里的js脚本标记
    /// </summary>
    public class JavascriptRegex : Regex
    {
        private const string scriptPattern = @"<script.*?>(.|\n)*?</script\s*>|(?<=<\w+.*?)\son\w+="".+?""(?=.*?>)|<\w{2,}\s+[^>]*?javascript:[^>]*?>";

        public JavascriptRegex()
        : base(scriptPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)
        {

        }
    }

}
