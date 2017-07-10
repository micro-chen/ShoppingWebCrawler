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
    public abstract unsafe partial class CefGetGeolocationCallback
    {
        private static Dictionary<IntPtr, CefGetGeolocationCallback> _roots = new Dictionary<IntPtr, CefGetGeolocationCallback>();
        
        private int _refct;
        private cef_get_geolocation_callback_t* _self;
        
        protected object SyncRoot { get { return this; } }
        
        private cef_get_geolocation_callback_t.add_ref_delegate _ds0;
        private cef_get_geolocation_callback_t.release_delegate _ds1;
        private cef_get_geolocation_callback_t.has_one_ref_delegate _ds2;
        private cef_get_geolocation_callback_t.on_location_update_delegate _ds3;
        
        protected CefGetGeolocationCallback()
        {
            _self = cef_get_geolocation_callback_t.Alloc();
        
            _ds0 = new cef_get_geolocation_callback_t.add_ref_delegate(add_ref);
            _self->_base._add_ref = Marshal.GetFunctionPointerForDelegate(_ds0);
            _ds1 = new cef_get_geolocation_callback_t.release_delegate(release);
            _self->_base._release = Marshal.GetFunctionPointerForDelegate(_ds1);
            _ds2 = new cef_get_geolocation_callback_t.has_one_ref_delegate(has_one_ref);
            _self->_base._has_one_ref = Marshal.GetFunctionPointerForDelegate(_ds2);
            _ds3 = new cef_get_geolocation_callback_t.on_location_update_delegate(on_location_update);
            _self->_on_location_update = Marshal.GetFunctionPointerForDelegate(_ds3);
        }
        
        ~CefGetGeolocationCallback()
        {
            Dispose(false);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_self != null)
            {
                cef_get_geolocation_callback_t.Free(_self);
                _self = null;
            }
        }
        
        private void add_ref(cef_get_geolocation_callback_t* self)
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
        
        private int release(cef_get_geolocation_callback_t* self)
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
        
        private int has_one_ref(cef_get_geolocation_callback_t* self)
        {
            lock (SyncRoot) { return _refct == 1 ? 1 : 0; }
        }
        
        internal cef_get_geolocation_callback_t* ToNative()
        {
            add_ref(_self);
            return _self;
        }
        
        [Conditional("DEBUG")]
        private void CheckSelf(cef_get_geolocation_callback_t* self)
        {
            if (_self != self) throw ExceptionBuilder.InvalidSelfReference();
        }
        
    }
}