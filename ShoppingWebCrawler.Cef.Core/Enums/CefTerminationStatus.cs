﻿//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_termination_status_t.
//
namespace ShoppingWebCrawler.Cef.Core
{
    /// <summary>
    /// Process termination status values.
    /// </summary>
    public enum CefTerminationStatus
    {
        /// <summary>
        /// Non-zero exit status.
        /// </summary>
        Termination,

        /// <summary>
        /// SIGKILL or task manager kill.
        /// </summary>
        WasKilled,

        /// <summary>
        /// Segmentation fault.
        /// </summary>
        ProcessCrashed,
    }
}
