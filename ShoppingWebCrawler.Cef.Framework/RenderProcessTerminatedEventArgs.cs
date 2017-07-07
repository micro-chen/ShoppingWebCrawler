using System;
using ShoppingWebCrawler.Cef.Core;
namespace ShoppingWebCrawler.Cef.Framework
{
	public class RenderProcessTerminatedEventArgs : EventArgs
	{
		public RenderProcessTerminatedEventArgs(CefTerminationStatus status)
		{
			Status = status;
		}

		public CefTerminationStatus Status { get; private set; }
	}
}
