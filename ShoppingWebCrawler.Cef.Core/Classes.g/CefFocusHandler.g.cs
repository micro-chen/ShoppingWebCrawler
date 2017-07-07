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
    public abstract unsafe partial class CefFocusHandler
    {
        private static Dictionary<IntPtr, CefFocusHandler> _roots = new Dictionary<IntPtr, CefFocusHandler>();
        
        private int _refct;
        private cef_focus_handler_t* _self;
        
        protected object SyncRoot { get { return this; } }
        
        private cef_focus_handler_t.add_ref_delegate _ds0;
        private cef_focus_handler_t.release_delegate _ds1;
        private cef_focus_handler_t.has_one_ref_delegate _ds2;
        private cef_focus_handler_t.on_take_focus_delegate _ds3;
        private cef_focus_handler_t.on_set_focus_delegate _ds4;
        private cef_focus_handler_t.on_got_focus_delegate _ds5;
        
        protected CefFocusHandler()
        {
            _self = cef_focus_handler_t.Alloc();
        
            _ds0 = new cef_focus_handler_t.add_ref_delegate(add_ref);
            _self->_base._add_ref = Marshal.GetFunctionPointerForDelegate(_ds0);
            _ds1 = new cef_focus_handler_t.release_delegate(release);
            _self->_base._release = Marshal.GetFunctionPointerForDelegate(_ds1);
            _ds2 = new cef_focus_handler_t.has_one_ref_delegate(has_one_ref);
            _self->_base._has_one_ref = Marshal.GetFunctionPointerForDelegate(_ds2);
            _ds3 = new cef_focus_handler_t.on_take_focus_delegate(on_take_focus);
            _self->_on_take_focus = Marshal.GetFunctionPointerForDelegate(_ds3);
            _ds4 = new cef_focus_handler_t.on_set_focus_delegate(on_set_focus);
            _self->_on_set_focus = Marshal.GetFunctionPointerForDelegate(_ds4);
            _ds5 = new cef_focus_handler_t.on_got_focus_delegate(on_got_focus);
            _self->_on_got_focus = Marshal.GetFunctionPointerForDelegate(_ds5);
        }
        
        ~CefFocusHandler()
        {
            Dispose(false);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_self != null)
            {
                cef_focus_handler_t.Free(_self);
                _self = null;
            }
        }
        
        private void add_ref(cef_focus_handler_t* self)
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
        
        private int release(cef_focus_handler_t* self)
        {
            lock (SyncRoot)
            {
                var result = --_refct;
                if (result == 0)
                {
                    lock (_roots) { _roots.Remove((IntPtr)_self); }
                    return 1;
                }
                return 0;
            }
        }
        
        private int has_one_ref(cef_focus_handler_t* self)
        {
            lock (SyncRoot) { return _refct == 1 ? 1 : 0; }
        }
        
        internal cef_focus_handler_t* ToNative()
        {
            add_ref(_self);
            return _self;
        }
        
        [Conditional("DEBUG")]
        private void CheckSelf(cef_focus_handler_t* self)
        {
            if (_self != self) throw ExceptionBuilder.InvalidSelfReference();
        }
        
    }
}
