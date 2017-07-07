﻿//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
namespace ShoppingWebCrawler.Cef.Core.Interop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Security;
    
    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
    internal unsafe struct cef_download_item_t
    {
        internal cef_base_t _base;
        internal IntPtr _is_valid;
        internal IntPtr _is_in_progress;
        internal IntPtr _is_complete;
        internal IntPtr _is_canceled;
        internal IntPtr _get_current_speed;
        internal IntPtr _get_percent_complete;
        internal IntPtr _get_total_bytes;
        internal IntPtr _get_received_bytes;
        internal IntPtr _get_start_time;
        internal IntPtr _get_end_time;
        internal IntPtr _get_full_path;
        internal IntPtr _get_id;
        internal IntPtr _get_url;
        internal IntPtr _get_original_url;
        internal IntPtr _get_suggested_file_name;
        internal IntPtr _get_content_disposition;
        internal IntPtr _get_mime_type;
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate void add_ref_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int release_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int has_one_ref_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int is_valid_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int is_in_progress_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int is_complete_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int is_canceled_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate long get_current_speed_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int get_percent_complete_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate long get_total_bytes_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate long get_received_bytes_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_time_t get_start_time_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_time_t get_end_time_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_full_path_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate uint get_id_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_url_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_original_url_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_suggested_file_name_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_content_disposition_delegate(cef_download_item_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_mime_type_delegate(cef_download_item_t* self);
        
        // AddRef
        private static IntPtr _p0;
        private static add_ref_delegate _d0;
        
        public static void add_ref(cef_download_item_t* self)
        {
            add_ref_delegate d;
            var p = self->_base._add_ref;
            if (p == _p0) { d = _d0; }
            else
            {
                d = (add_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(add_ref_delegate));
                if (_p0 == IntPtr.Zero) { _d0 = d; _p0 = p; }
            }
            d(self);
        }
        
        // Release
        private static IntPtr _p1;
        private static release_delegate _d1;
        
        public static int release(cef_download_item_t* self)
        {
            release_delegate d;
            var p = self->_base._release;
            if (p == _p1) { d = _d1; }
            else
            {
                d = (release_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(release_delegate));
                if (_p1 == IntPtr.Zero) { _d1 = d; _p1 = p; }
            }
            return d(self);
        }
        
        // HasOneRef
        private static IntPtr _p2;
        private static has_one_ref_delegate _d2;
        
        public static int has_one_ref(cef_download_item_t* self)
        {
            has_one_ref_delegate d;
            var p = self->_base._has_one_ref;
            if (p == _p2) { d = _d2; }
            else
            {
                d = (has_one_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(has_one_ref_delegate));
                if (_p2 == IntPtr.Zero) { _d2 = d; _p2 = p; }
            }
            return d(self);
        }
        
        // IsValid
        private static IntPtr _p3;
        private static is_valid_delegate _d3;
        
        public static int is_valid(cef_download_item_t* self)
        {
            is_valid_delegate d;
            var p = self->_is_valid;
            if (p == _p3) { d = _d3; }
            else
            {
                d = (is_valid_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(is_valid_delegate));
                if (_p3 == IntPtr.Zero) { _d3 = d; _p3 = p; }
            }
            return d(self);
        }
        
        // IsInProgress
        private static IntPtr _p4;
        private static is_in_progress_delegate _d4;
        
        public static int is_in_progress(cef_download_item_t* self)
        {
            is_in_progress_delegate d;
            var p = self->_is_in_progress;
            if (p == _p4) { d = _d4; }
            else
            {
                d = (is_in_progress_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(is_in_progress_delegate));
                if (_p4 == IntPtr.Zero) { _d4 = d; _p4 = p; }
            }
            return d(self);
        }
        
        // IsComplete
        private static IntPtr _p5;
        private static is_complete_delegate _d5;
        
        public static int is_complete(cef_download_item_t* self)
        {
            is_complete_delegate d;
            var p = self->_is_complete;
            if (p == _p5) { d = _d5; }
            else
            {
                d = (is_complete_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(is_complete_delegate));
                if (_p5 == IntPtr.Zero) { _d5 = d; _p5 = p; }
            }
            return d(self);
        }
        
        // IsCanceled
        private static IntPtr _p6;
        private static is_canceled_delegate _d6;
        
        public static int is_canceled(cef_download_item_t* self)
        {
            is_canceled_delegate d;
            var p = self->_is_canceled;
            if (p == _p6) { d = _d6; }
            else
            {
                d = (is_canceled_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(is_canceled_delegate));
                if (_p6 == IntPtr.Zero) { _d6 = d; _p6 = p; }
            }
            return d(self);
        }
        
        // GetCurrentSpeed
        private static IntPtr _p7;
        private static get_current_speed_delegate _d7;
        
        public static long get_current_speed(cef_download_item_t* self)
        {
            get_current_speed_delegate d;
            var p = self->_get_current_speed;
            if (p == _p7) { d = _d7; }
            else
            {
                d = (get_current_speed_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_current_speed_delegate));
                if (_p7 == IntPtr.Zero) { _d7 = d; _p7 = p; }
            }
            return d(self);
        }
        
        // GetPercentComplete
        private static IntPtr _p8;
        private static get_percent_complete_delegate _d8;
        
        public static int get_percent_complete(cef_download_item_t* self)
        {
            get_percent_complete_delegate d;
            var p = self->_get_percent_complete;
            if (p == _p8) { d = _d8; }
            else
            {
                d = (get_percent_complete_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_percent_complete_delegate));
                if (_p8 == IntPtr.Zero) { _d8 = d; _p8 = p; }
            }
            return d(self);
        }
        
        // GetTotalBytes
        private static IntPtr _p9;
        private static get_total_bytes_delegate _d9;
        
        public static long get_total_bytes(cef_download_item_t* self)
        {
            get_total_bytes_delegate d;
            var p = self->_get_total_bytes;
            if (p == _p9) { d = _d9; }
            else
            {
                d = (get_total_bytes_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_total_bytes_delegate));
                if (_p9 == IntPtr.Zero) { _d9 = d; _p9 = p; }
            }
            return d(self);
        }
        
        // GetReceivedBytes
        private static IntPtr _pa;
        private static get_received_bytes_delegate _da;
        
        public static long get_received_bytes(cef_download_item_t* self)
        {
            get_received_bytes_delegate d;
            var p = self->_get_received_bytes;
            if (p == _pa) { d = _da; }
            else
            {
                d = (get_received_bytes_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_received_bytes_delegate));
                if (_pa == IntPtr.Zero) { _da = d; _pa = p; }
            }
            return d(self);
        }
        
        // GetStartTime
        private static IntPtr _pb;
        private static get_start_time_delegate _db;
        
        public static cef_time_t get_start_time(cef_download_item_t* self)
        {
            get_start_time_delegate d;
            var p = self->_get_start_time;
            if (p == _pb) { d = _db; }
            else
            {
                d = (get_start_time_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_start_time_delegate));
                if (_pb == IntPtr.Zero) { _db = d; _pb = p; }
            }
            return d(self);
        }
        
        // GetEndTime
        private static IntPtr _pc;
        private static get_end_time_delegate _dc;
        
        public static cef_time_t get_end_time(cef_download_item_t* self)
        {
            get_end_time_delegate d;
            var p = self->_get_end_time;
            if (p == _pc) { d = _dc; }
            else
            {
                d = (get_end_time_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_end_time_delegate));
                if (_pc == IntPtr.Zero) { _dc = d; _pc = p; }
            }
            return d(self);
        }
        
        // GetFullPath
        private static IntPtr _pd;
        private static get_full_path_delegate _dd;
        
        public static cef_string_userfree* get_full_path(cef_download_item_t* self)
        {
            get_full_path_delegate d;
            var p = self->_get_full_path;
            if (p == _pd) { d = _dd; }
            else
            {
                d = (get_full_path_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_full_path_delegate));
                if (_pd == IntPtr.Zero) { _dd = d; _pd = p; }
            }
            return d(self);
        }
        
        // GetId
        private static IntPtr _pe;
        private static get_id_delegate _de;
        
        public static uint get_id(cef_download_item_t* self)
        {
            get_id_delegate d;
            var p = self->_get_id;
            if (p == _pe) { d = _de; }
            else
            {
                d = (get_id_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_id_delegate));
                if (_pe == IntPtr.Zero) { _de = d; _pe = p; }
            }
            return d(self);
        }
        
        // GetURL
        private static IntPtr _pf;
        private static get_url_delegate _df;
        
        public static cef_string_userfree* get_url(cef_download_item_t* self)
        {
            get_url_delegate d;
            var p = self->_get_url;
            if (p == _pf) { d = _df; }
            else
            {
                d = (get_url_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_url_delegate));
                if (_pf == IntPtr.Zero) { _df = d; _pf = p; }
            }
            return d(self);
        }
        
        // GetOriginalUrl
        private static IntPtr _p10;
        private static get_original_url_delegate _d10;
        
        public static cef_string_userfree* get_original_url(cef_download_item_t* self)
        {
            get_original_url_delegate d;
            var p = self->_get_original_url;
            if (p == _p10) { d = _d10; }
            else
            {
                d = (get_original_url_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_original_url_delegate));
                if (_p10 == IntPtr.Zero) { _d10 = d; _p10 = p; }
            }
            return d(self);
        }
        
        // GetSuggestedFileName
        private static IntPtr _p11;
        private static get_suggested_file_name_delegate _d11;
        
        public static cef_string_userfree* get_suggested_file_name(cef_download_item_t* self)
        {
            get_suggested_file_name_delegate d;
            var p = self->_get_suggested_file_name;
            if (p == _p11) { d = _d11; }
            else
            {
                d = (get_suggested_file_name_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_suggested_file_name_delegate));
                if (_p11 == IntPtr.Zero) { _d11 = d; _p11 = p; }
            }
            return d(self);
        }
        
        // GetContentDisposition
        private static IntPtr _p12;
        private static get_content_disposition_delegate _d12;
        
        public static cef_string_userfree* get_content_disposition(cef_download_item_t* self)
        {
            get_content_disposition_delegate d;
            var p = self->_get_content_disposition;
            if (p == _p12) { d = _d12; }
            else
            {
                d = (get_content_disposition_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_content_disposition_delegate));
                if (_p12 == IntPtr.Zero) { _d12 = d; _p12 = p; }
            }
            return d(self);
        }
        
        // GetMimeType
        private static IntPtr _p13;
        private static get_mime_type_delegate _d13;
        
        public static cef_string_userfree* get_mime_type(cef_download_item_t* self)
        {
            get_mime_type_delegate d;
            var p = self->_get_mime_type;
            if (p == _p13) { d = _d13; }
            else
            {
                d = (get_mime_type_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_mime_type_delegate));
                if (_p13 == IntPtr.Zero) { _d13 = d; _p13 = p; }
            }
            return d(self);
        }
        
    }
}
