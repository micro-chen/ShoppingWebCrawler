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
        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string Pwd { get; set; }

        /// <summary>
        /// 从指定的配置文件加载配置
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static RedisConfig LoadConfig(string configPath)
        {
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
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static bool WriteConfig(RedisConfig model,string configPath)
        {
            var result = false;
            try
            {
                string content = JsonConvert.SerializeObject(model);
                if (!File.Exists(configPath))
                {
                    File.Delete(configPath);
                }
                File.WriteAllText(configPath, content, Encoding.UTF8);
                result = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
            
        }
    }
}
