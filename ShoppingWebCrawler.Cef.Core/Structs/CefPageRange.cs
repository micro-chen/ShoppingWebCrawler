namespace ShoppingWebCrawler.Cef.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ShoppingWebCrawler.Cef.Core.Interop;

    public struct CefPageRange
    {
        internal static unsafe CefPageRange Create(cef_page_range_t* pageRange)
        {
            return new CefPageRange(
                pageRange->from,
                pageRange->to
                );
        }

        private int _from;
        private int _to;

        public CefPageRange(int from, int to)
        {
            _from = from;
            _to = to;
        }

        public int From
        {
            get { return _from; }
            set { _from = value; }
        }

        public int To
        {
            get { return _to; }
            set { _to = value; }
        }
    }
}
