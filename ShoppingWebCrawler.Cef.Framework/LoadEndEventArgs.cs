using System;
using ShoppingWebCrawler.Cef.Core;
namespace ShoppingWebCrawler.Cef.Framework
{
	public class LoadEndEventArgs : EventArgs
	{
		public LoadEndEventArgs(CefFrame frame, int httpStatusCode)
		{
			Frame = frame;
			HttpStatusCode = httpStatusCode;
		}

		public int HttpStatusCode { get; private set; }

		public CefFrame Frame { get; private set; }
	}
}
