﻿namespace ShoppingWebCrawler.Cef.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class CefVersionMismatchException : CefRuntimeException
    {
        public CefVersionMismatchException(string message)
            : base(message)
        {
        }
    }
}
