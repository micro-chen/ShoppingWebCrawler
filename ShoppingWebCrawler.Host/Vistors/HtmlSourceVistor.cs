using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host
{

    public class HtmlVistCompletedEventAgrs : EventArgs
    {


        #region 属性

        //指向结果集合的引用
        public string HtmSourceCode;


        #endregion

    }

    /// <summary>
    /// 实现CEF内核的异步读取HTML的接口，用来获取网页源码
    /// </summary>
    public class HtmlSourceVistor : CefStringVisitor
    {

        #region 字段

        private TaskCompletionSource<string> _tcs = null;
        private EventHandler<HtmlVistCompletedEventAgrs> _handler = null;

        #endregion

        //加载完毕后，事件

        public event EventHandler<HtmlVistCompletedEventAgrs> VistHtmlSourceCompleted;


        protected void OnVistHtmlSourceCompleted(HtmlVistCompletedEventAgrs agrs)
        {
            if (null != this.VistHtmlSourceCompleted)
            {
                this.VistHtmlSourceCompleted.Invoke(this, agrs);

            }

        }


        /// <summary>
        /// 返回同步的获取HTML
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public string ReadHtmlSourceSync(CefFrame frame)
        {
            string result = string.Empty;
            var tsk = this.ReadHtmlSourceAsyc(frame, out result);

            tsk.Wait();//等待task执行完毕

            return result;
        }



        /// <summary>
        /// 返回异步的获取html 源码的Task
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public Task ReadHtmlSourceAsyc(CefFrame frame,out string result)
        {
            this._tcs = new TaskCompletionSource<string>();
            //事件回调
            this._handler = (s, e) =>
            {
                this._tcs.TrySetResult(e.HtmSourceCode);

            };
            this.VistHtmlSourceCompleted += _handler;

            frame.GetSource(this);
            result = this._tcs.Task.Result;
            return this._tcs.Task;
        }



        protected override void Visit(string value)
        {
            var agrs = new HtmlVistCompletedEventAgrs() { HtmSourceCode=value };
            this.OnVistHtmlSourceCompleted(agrs);
        }
    }
}
