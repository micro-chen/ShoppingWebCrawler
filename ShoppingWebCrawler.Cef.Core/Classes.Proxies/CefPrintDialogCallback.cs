namespace ShoppingWebCrawler.Cef.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using ShoppingWebCrawler.Cef.Core.Interop;

    /// <summary>
    /// Callback interface for asynchronous continuation of print dialog requests.
    /// </summary>
    public sealed unsafe partial class CefPrintDialogCallback
    {
        /// <summary>
        /// Continue printing with the specified |settings|.
        /// </summary>
        public void Continue(CefPrintSettings settings)
        {
            cef_print_dialog_callback_t.cont(_self, settings.ToNative());
        }

        /// <summary>
        /// Cancel the printing.
        /// </summary>
        public void Cancel()
        {
            cef_print_dialog_callback_t.cancel(_self);
        }
    }
}
