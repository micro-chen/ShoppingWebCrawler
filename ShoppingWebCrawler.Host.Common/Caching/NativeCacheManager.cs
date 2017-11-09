using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Caching;

namespace ShoppingWebCrawler.Host.Common.Caching
{
    /// <summary>
    /// 缓存处理类
    /// 提供一个基于 .net  Catche对象的管理容器
    /// 实现缓存的管理
    /// </summary>
    public sealed class NativeCacheManager : ICacheManager
    {

        #region 单例实例



        /// <summary>
        /// </summary>
        private static NativeCacheManager _Default;
        /// <summary>
        /// 单例实例
        /// </summary>
        public static NativeCacheManager Current
        {
            get
            {
                if (null == _Default)
                {
                    _Default = new NativeCacheManager();
                }
                return _Default;
            }

        }


        private static  ObjectCache CacheCore
        {
            get
            {
                return MemoryCache.Default;
            }
        }


        #endregion



        #region 构造函数

        /// <summary>
        /// 空参数的构造函数，默认所有的缓存不过期
        /// </summary>
        public NativeCacheManager() { }

        ///// <summary>
        ///// 设置过期时间
        ///// </summary>
        ///// <param name="timeOut"></param>
        //public NativeCacheManager(int timeOut)
        //{
        //    this._timeOut = timeOut;
        //}

        #endregion



        ///// <summary>
        ///// timeout ( 默认为300s  5min过期，如果太大，会导致缓存中的数据量越来越大，降低缓存的检索效率 )
        ///// </summary>
        //private int _timeOut = 300;
        ///// <summary>
        ///// time out (seconds)   
        ///// </summary>
        //public int TimeOut
        //{
        //    set { _timeOut = value; }
        //    get { return _timeOut; }
        //}


        #region 添加缓存

        public bool IsHasSet(string key)
        {
            return (CacheCore.Contains(key));
        }



        /// <summary>
        /// 添加缓存 (绝对有效期)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        /// <param name="timeout">绝对有效期（单位: 秒）</param>
        public void Set(string cacheKey, object cacheValue, int timeout = CacheConfigFactory.DefaultTimeOut)
        {

            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (null == cacheValue)
            {
                Remove(cacheKey);
                return;
            }
            //缓存策略
            var policy = new CacheItemPolicy();
            //绝对过期
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(timeout);
            CacheCore.Add(new CacheItem(cacheKey, cacheValue), policy);
            //if (timeout <= 0)
            //{
            //    //绝对过期
            //    policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(TimeOut);
            //    CacheCore.Add(new CacheItem(cacheKey, cacheValue), policy);
            //}
            //else
            //{
            //    //相对过期
            //    policy.SlidingExpiration = TimeSpan.FromSeconds(TimeOut);
            //    CacheCore.Add(new CacheItem(cacheKey, cacheValue), policy);
            //}
        }





        #endregion


        #region 删除缓存



        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        public void Remove(string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
                CacheCore.Remove(cacheKey);
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void Clear()
        {
            foreach (var item in CacheCore)
                Remove(item.Key);
        }

        #endregion


        #region 获取缓存


  

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public  T Get<T>(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return default(T);
            }
            return (T)CacheCore[cacheKey];
        }



        /// <summary>
        /// 返回缓存键值列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCacheKeys()
        {

            List<string> cacheKeys = new List<string>();
            foreach (var cacheEnum in CacheCore)
            {
                cacheKeys.Add(cacheEnum.Key.ToString());
            }

            return cacheKeys;
        }


        #endregion




    }
}
