
using System;
using System.Collections.Generic;
using ShoppingWebCrawler.Cef.Core;
namespace ShoppingWebCrawler.Cef.Framework
{

    /// <summary>
    /// 默认的 响应右键的菜单处理
    /// 
    /// </summary>
    public class DefaultContextMenuHandler: CefContextMenuHandler
    {

        private bool _isShowMenu;

        public DefaultContextMenuHandler(bool isShowMenu)
        {
            this._isShowMenu = isShowMenu;
        }

        protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
        {
            if (this._isShowMenu == false)
            {
                model.Clear();//清除掉构建的菜单对象
                return;
            }

            base.OnBeforeContextMenu(browser, frame, state, model);
        }
    }
}
