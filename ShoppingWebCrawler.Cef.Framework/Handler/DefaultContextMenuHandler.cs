
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
            model.Clear();//清除掉构建的菜单对象
            if (this._isShowMenu == false) { 
                return;
            }


            //添加刷新菜单
            //model.AddItem((int)CefMenuId.ReloadNoCache, "强制刷新");
            model.AddItem((int)CefMenuId.Back, "返回");
            model.AddItem((int)CefMenuId.Forward, "前进");
            model.AddItem((int)CefMenuId.ReloadNoCache, "强制刷新");
            model.AddItem((int)CefMenuId.ViewSource, "查看源码");

            base.OnBeforeContextMenu(browser, frame, state, model);
        }
    }
}
