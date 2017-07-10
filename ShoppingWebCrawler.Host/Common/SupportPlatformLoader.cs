using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Caching;
using NTCPMessage.EntityPackage;

namespace ShoppingWebCrawler.Host
{

    /// <summary>
    /// 支持平台配置文件 变更的时候 触发的事件类参数
    /// </summary>
    public class SupportPlatformsChangedEventArgs : EventArgs
    {
        public List<SupportPlatform> CurrentSupportPlatforms { get; set; }
    }
    [Serializable]
    public class SupportPlatformLoader
    {
        /// <summary>
        /// 静态字段
        /// </summary>
        private static string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "SupportPlatforms.xml");

        /// <summary>
        /// 一个静态的事件，用来进行注册多播委托
        /// </summary>
        private static event EventHandler<SupportPlatformsChangedEventArgs> OnConfigFileChanged;



        /// <summary>
        /// 从配置文件 加载支持平台列表
        /// </summary>
        /// <returns></returns>
        public static List<SupportPlatform> LoadConfig()
        {
            var lstData = new List<SupportPlatform>();

            try
            {
                if (!File.Exists(configFilePath))
                {
                    throw new FileNotFoundException(string.Concat("指定的配置文件不存在：", configFilePath));
                }


                XDocument doc = XDocument.Load(configFilePath);
                var allPlatformElements = doc.Root.Elements();
                if (null == allPlatformElements)
                {
                    return lstData;
                }
                foreach (var item in allPlatformElements)
                {
                    var model = new SupportPlatform();
                    model.Id = int.Parse(item.Attribute("id").Value);
                    model.Name = item.Attribute("name").Value;
                    model.Description = item.Attribute("description").Value;
                    model.SiteUrl = item.Attribute("siteUrl").Value;

                    lstData.Add(model);
                }

            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);

            }

            return lstData;
        }




        /// <summary>
        /// 监视配置文件
        /// </summary>
        public static void MonitorConfigFile(EventHandler<SupportPlatformsChangedEventArgs> callbackOnConfigChangedHandler)
        {
            //使用基于文件缓存依赖的方式去监听文件  而不是使用 FileSystemWatch
            var cacheContainer = MemoryCache.Default;

            string cacheKey = "SupportPlatform-MonitorConfigFile";
            if (cacheContainer.Contains(cacheKey) && null != callbackOnConfigChangedHandler)
            {
                //如果已经存在这个缓存 表示已经加入了依赖监视 ，直接注册委托广播源即可
                OnConfigFileChanged += callbackOnConfigChangedHandler;
                return;
            }



            //缓存策略
            var policy = new CacheItemPolicy();

            //绝对过期
            policy.Priority = CacheItemPriority.NotRemovable;
            cacheContainer.Add(new CacheItem(cacheKey, DateTime.Now.ToString()), policy);

            //开启文件缓存 进行监视依赖
            HostFileChangeMonitor monitor = new HostFileChangeMonitor(new List<string> { configFilePath });
            //当文件发生变更的时候 触发通知
            monitor.NotifyOnChanged((state) =>
            {

                //获取最新的配置 并触发事件
                var lstNewConfigs = SupportPlatformLoader.LoadConfig();
                OnConfigFileChanged(null, new SupportPlatformsChangedEventArgs { CurrentSupportPlatforms = lstNewConfigs });
            });
            policy.ChangeMonitors.Add(monitor);


        }


    }
}
