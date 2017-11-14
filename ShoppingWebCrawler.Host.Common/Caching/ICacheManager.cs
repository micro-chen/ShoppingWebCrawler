using System;
using System.Collections.Generic;
 namespace ShoppingWebCrawler.Host.Common.Caching
{
    /// <summary>
    /// Cache manager interface
    /// 暂时只实现了内置的Cache对象管理器
    /// MemCache Redis 未加入实现
    /// </summary>
    public interface ICacheManager:IDisposable
    {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        T Get<T>(string key);

        /// <summary>
        /// 添加数据到缓存中
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">数据</param>
        /// <param name="cacheTime">缓存时间（单位：秒）；</param>
        void Set(string key, object data, int cacheTime = CacheConfigFactory.DefaultTimeOut);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool IsHasSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        ///// <summary>
        ///// Removes items by pattern
        ///// </summary>
        ///// <param name="pattern">pattern</param>
        //void RemoveByPattern(string pattern);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();

        ///// <summary>
        ///// get the cache manager all keys 
        ///// </summary>
        ///// <returns></returns>
        //IEnumerable<string> GetCacheKeys();
    }
}
