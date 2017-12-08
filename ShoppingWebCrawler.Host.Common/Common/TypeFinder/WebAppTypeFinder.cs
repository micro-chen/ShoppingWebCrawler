using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace ShoppingWebCrawler.Host.Common.TypeFinder
{
    /// <summary>
    /// Provides information about types in the current web application. 
    /// Optionally this class can look at all assemblies in the bin folder.
    /// 继承AppDomainTypeFinder  来具体化  Web程序中的程序集的操作访问
    /// </summary>
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        #region Fields

        private bool _binFolderAssembliesLoaded = false;

        #endregion

        #region Ctor

        public WebAppTypeFinder()
        {
        }

        #endregion

        #region Properties


        
        #endregion

        #region Methods



        public override IList<Assembly> GetAssemblies()
        {
            if (!_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                string binPath = GetBinDirectory();
                //binPath = _webHelper.MapPath("~/bin");
                LoadMatchingAssemblies(binPath);
            }

            return base.GetAssemblies();
        }

        #endregion
    }
}
