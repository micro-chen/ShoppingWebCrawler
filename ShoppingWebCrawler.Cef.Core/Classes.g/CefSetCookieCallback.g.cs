﻿//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
namespace ShoppingWebCrawler.Cef.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using ShoppingWebCrawler.Cef.Core.Interop;
    
    // Role: HANDLER
    public abstract unsafe partial class CefSetCookieCallback
    {
        private static Dictionary<IntPtr, CefSetCookieCallback> _roots = new Dictionary<IntPtr, CefSetCookieCallback>();
        
        private int _refct;
        private cef_set_cookie_callback_t* _self;
        
        protected object SyncRoot { get { return this; } }
        
        private cef_set_cookie_callback_t.add_ref_delegate _ds0;
        private cef_set_cookie_callback_t.release_delegate _ds1;
        private cef_set_cookie_callback_t.has_one_ref_delegate _ds2;
        private cef_set_cookie_callback_t.on_complete_delegate _ds3;
        
        protected CefSetCookieCallback()
        {
            _self = cef_set_cookie_callback_t.Alloc();
        
            _ds0 = new cef_set_cookie_callback_t.add_ref_delegate(add_ref);
            _self->_base._add_ref = Marshal.GetFunctionPointerForDelegate(_ds0);
            _ds1 = new cef_set_cookie_callback_t.release_delegate(release);
            _self->_base._release = Marshal.GetFunctionPointerForDelegate(_ds1);
            _ds2 = new cef_set_cookie_callback_t.has_one_ref_delegate(has_one_ref);
            _self->_base._has_one_ref = Marshal.GetFunctionPointerForDelegate(_ds2);
            _ds3 = new cef_set_cookie_callback_t.on_complete_delegate(on_complete);
            _self->_on_complete = Marshal.GetFunctionPointerForDelegate(_ds3);
        }
        
        ~CefSetCookieCallback()
        {
            Dispose(false);
        }
        
        private void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_self != null)
            {
                cef_set_cookie_callback_t.Free(_self);
                _self = null;
            }
        }
        
        private void add_ref(cef_set_cookie_callback_t* self)
        {
            lock (SyncRoot)
            {
                var result = ++_refct;
                if (result == 1)
                {
                    lock (_roots) { _roots.Add((IntPtr)_self, this); }
                }
            }
        }
        
        private int release(cef_set_cookie_callback_t* self)
        {
            lock (SyncRoot)
            {
                var result = --_refct;
                if (result == 0)
                {
                    lock (_roots) { _roots.Remove((IntPtr)_self); }
                    Dispose();
                    return 1;
                }
                return 0;
            }
        }
        
        private int has_one_ref(cef_set_cookie_callback_t* self)
        {
            lock (SyncRoot) { return _refct == 1 ? 1 : 0; }
        }
        
        internal cef_set_cookie_callback_t* ToNative()
        {
            add_ref(_self);
            return _self;
        }
        
        [Conditional("DEBUG")]
        private void CheckSelf(cef_set_cookie_callback_t* self)
        {
            if (_self != self) throw ExceptionBuilder.InvalidSelfReference();
        }
        
    }
}
