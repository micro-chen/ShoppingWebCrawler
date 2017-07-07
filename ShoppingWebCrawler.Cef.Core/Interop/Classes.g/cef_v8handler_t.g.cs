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
    internal unsafe struct cef_v8handler_t
    {
        internal cef_base_t _base;
        internal IntPtr _execute;
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate void add_ref_delegate(cef_v8handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int release_delegate(cef_v8handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int has_one_ref_delegate(cef_v8handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int execute_delegate(cef_v8handler_t* self, cef_string_t* name, cef_v8value_t* @object, UIntPtr argumentsCount, cef_v8value_t** arguments, cef_v8value_t** retval, cef_string_t* exception);
        
        private static int _sizeof;
        
        static cef_v8handler_t()
        {
            _sizeof = Marshal.SizeOf(typeof(cef_v8handler_t));
        }
        
        internal static cef_v8handler_t* Alloc()
        {
            var ptr = (cef_v8handler_t*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_v8handler_t();
            ptr->_base._size = (UIntPtr)_sizeof;
            return ptr;
        }
        
        internal static void Free(cef_v8handler_t* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
        
    }
}
