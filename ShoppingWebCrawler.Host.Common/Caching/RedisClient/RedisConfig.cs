using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ShoppingWebCrawler.Host.Common.Caching.RedisClient
{

    /// <summary>
    /// redis 连接配置
    /// </summary>
    public class RedisConfig
    {

        private const string _redis_config_fileName = "redis.config.json";
        private const string _redis_test_connt_key = "ShoppingWebCrawler.DeskTop.Redis.Test";


        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string Pwd { get; set; }

        public int WhichDb { get; set; }
        /// <summary>
        /// 默认的配置文件
        /// </summary>
        private static readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "redis.config.json");
        /// <summary>
        /// 从指定的配置文件加载配置
        /// </summary>
        /// <returns></returns>
        public static RedisConfig LoadConfig()
        {
            string configPath = _configPath;
            if (!File.Exists(configPath))
            {
                return null;
            }

            var configContent = File.ReadAllText(configPath, System.Text.Encoding.UTF8);

            var model = JsonConvert.DeserializeObject<RedisConfig>(configContent);

            return model;
        }

        /// <summary>
        /// 将配置写入到指定的文件
        /// </summary>
        /// <returns></returns>
        public static bool WriteConfig(RedisConfig model)
        {
            var result = false;
            try
            {
                string content = JsonConvert.SerializeObject(model);
                if (!File.Exists(_configPath))
                {
                    File.Delete(_configPath);
                }
                File.WriteAllText(_configPath, content, Encoding.UTF8);
                result = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;

        }


        public static bool IsValidConfig
        {
            get
            {

                bool result = false;
                try
                {


                    //加载redis client
                    var redisConfig = LoadConfig();
                    var client = new RedisCacheManager(redisConfig);
                    string testValue = Guid.NewGuid().ToString();
                    client.Set(_redis_test_connt_key, testValue);
                    string remoteValue = client.Get<string>(_redis_test_connt_key);
                    string msg = string.Empty;
                    if (testValue.Equals(remoteValue))
                    {
                        result = true;
                        client.RemoveAsync(_redis_test_connt_key);
                    }


                }
                catch (Exception ex)
                {

                    Logging.Logger.Error(ex);
                }

                return result;
            }
        }
    }
}
