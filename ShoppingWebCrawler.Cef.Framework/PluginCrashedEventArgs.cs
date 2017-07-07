using System;

namespace ShoppingWebCrawler.Cef.Framework
{
	public class PluginCrashedEventArgs : EventArgs
	{
		public PluginCrashedEventArgs(string pluginPath)
		{
			PluginPath = pluginPath;
		}

		public string PluginPath { get; private set; }
	}
}
