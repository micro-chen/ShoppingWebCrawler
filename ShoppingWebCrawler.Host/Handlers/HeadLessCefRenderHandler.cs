
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingWebCrawler.Cef.Core;
using System.Drawing;
using System.Drawing.Imaging;

namespace ShoppingWebCrawler.Host.Handlers
{

    internal class HeadLessCefRenderHandler : CefRenderHandler
    {
        private readonly int _windowHeight;
        private readonly int _windowWidth;

        public HeadLessCefRenderHandler(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return GetViewRect(browser, ref rect);
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect.X = 0;
            rect.Y = 0;
            rect.Width = _windowWidth;
            rect.Height = _windowHeight;
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            // Save the provided buffer (a bitmap image) as a PNG.
            //var bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppRgb, buffer);
            //bitmap.Save("LastOnPaint.png", ImageFormat.Png);
            //int x = 9;
        }


        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
            //throw new NotImplementedException();
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
            //throw new NotImplementedException();
        }

    }
}
