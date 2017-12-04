
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using ShoppingWebCrawler.Host.Common.Caching.RedisClient;

namespace ShoppingWebCrawler.Host.Common.Caching
{
    /// <summary>
    /// Redis 缓存管理
    /// Represents a manager for caching in Redis store (http://redis.io/).
    /// Mostly it'll be used when running in a web farm or Azure.
    /// But of course it can be also used on any server or environment
    /// </summary>
    public class RedisCacheManager : ICacheManager
    {

        private static RedisCacheManager _current;


        /// <summary>
        /// 单例模式--获取默认配置的redis 连接
        /// </summary>
        public static RedisCacheManager Current
        {
            get
            {

                if (null == _current)
                {
                    var config = RedisConfig.LoadConfig();
                    _current = new RedisCacheManager(config);
                }
                return RedisCacheManager._current;
            }
            set
            {
                _current = value;
            }
        }


        #region Fields

   

        private readonly RedisConnectionWrapper _connectionWrapper;

        private IDatabase _Database;

        public  IDatabase Database
        {
            get
            {
                return _Database;
            }
        }



        #endregion

        #region Ctor

        //public RedisCacheManager()
        //{
        //    //var connStr = RedisConnectionWrapper.GetConnectionString();

        //    //if (String.IsNullOrEmpty(connStr))
        //    //    throw new Exception("Redis connection string is empty");

        //    // ConnectionMultiplexer.Connect should only be called once and shared between callers
        //    this._connectionWrapper = RedisConnectionWrapper.Current;

        //    var redisDb = ConfigHelper.GetConfigInt("redisDb");

        //    _Database= _connectionWrapper.GetDatabase(redisDb);
        //}

        /// <summary>
        /// 使用自定义的配置  获取redis 连接的实例
        /// </summary>
        public RedisCacheManager(RedisConfig config)
        {

            string redisHost = config.IpAddress;
            string redisPort = config.Port.ToString();
            string redisPassword = config.Pwd;
            string connStr = RedisConnectionWrapper.GetConnectionString(redisHost, redisPort, redisPassword);

            // ConnectionMultiplexer.Connect should only be called once and shared between callers
            this._connectionWrapper = new RedisConnectionWrapper(connStr);

            _Database = _connectionWrapper.GetDatabase(config.WhichDb);


        }



        #endregion

        #region Utilities

        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        protected virtual T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 切换redis  数据库
        /// </summary>
        /// <param name="redisDb"></param>
        /// <returns></returns>
        public bool SelectDb(int redisDb) {
            var result = false;
            try
            {
                if (redisDb<0)
                {
                    throw new Exception("redis db 索引不能小于0！");
                }
                 
                _Database = _connectionWrapper.GetDatabase(redisDb);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {

            var rValue = Database.StringGet(key);
            if (!rValue.HasValue)
                return default(T);
            var result = Deserialize<T>(rValue);

            return result;
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public virtual void Set(string key, object data, int cacheTime = CacheConfigFactory.DefaultTimeOut)
        {
            if (data == null)
                return;

            var entryBytes = Serialize(data);
            var expiresIn = TimeSpan.FromSeconds(cacheTime);

            
            Database.StringSet(key, entryBytes, expiresIn);
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public virtual void SetAsync(string key, object data, int cacheTime = CacheConfigFactory.DefaultTimeOut)
        {
            if (data == null)
                return;

            var entryBytes = Serialize(data);
            var expiresIn = TimeSpan.FromSeconds(cacheTime);


            Database.StringSetAsync(key, entryBytes, expiresIn);
        }
        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsHasSet(string key)
        {
            return Database.KeyExists(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            Database.KeyDelete(key);
        }

        public virtual Task<bool> RemoveAsync(string key)
        {
            var task = Task.Factory.StartNew(() => {
                return Database.KeyDelete(key);
            });

            return task;
          
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            foreach (var ep in _connectionWrapper.GetEndPoints())
            {
                var server = _connectionWrapper.GetServer(ep);
                var keys = server.Keys(database: Database.Database, pattern: "*" + pattern + "*");
                foreach (var key in keys)
                    Remove(key);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            _connectionWrapper.FlushDatabase();

            //foreach (var ep in _connectionWrapper.GetEndPoints())
            //{
            //    var server = _connectionWrapper.GetServer(ep);
            //    //we can use the code below (commented)
            //    //but it requires administration permission - ",allowAdmin=true"
            //    //server.FlushDatabase();

            //    //that's why we simply interate through all elements now
            //    var keys = server.Keys(database: _db.Database);
            //    foreach (var key in keys)
            //        Remove(key);
            //}
        }




        #endregion


        #region dispose
        private bool disposed = false;
        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool isDisposing)
        {
          
            if (!disposed)
            {
                this._Database = null;
                this._connectionWrapper.Dispose();
            }

            disposed = true;
        }
        #endregion
    }
}
