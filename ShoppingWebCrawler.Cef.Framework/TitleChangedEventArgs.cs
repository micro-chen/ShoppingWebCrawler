using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingWebCrawler.Cef.Framework
{
	public class TitleChangedEventArgs : EventArgs
	{
		public TitleChangedEventArgs(string title)
		{
			Title = title;
		}

		public string Title { get; private set; }
	}
}
