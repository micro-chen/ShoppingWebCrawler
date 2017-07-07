using System;

namespace ShoppingWebCrawler.Cef.Framework
{
	public class LoadingStateChangeEventArgs : EventArgs
	{
		public LoadingStateChangeEventArgs(bool isLoading, bool canGoBack, bool canGoForward)
		{
			IsLoading = isLoading;
			CanGoBack = canGoBack;
			CanGoForward = canGoForward;
		}

		public bool IsLoading { get; private set; } 
		public bool CanGoBack { get; private set; }
		public bool CanGoForward { get; private set; } 
	}
}
