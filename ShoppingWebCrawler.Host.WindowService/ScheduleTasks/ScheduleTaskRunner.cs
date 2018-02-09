using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;
using System.Configuration;

using Quartz;
using Quartz.Impl;
using ShoppingWebCrawler.Host.WindowService.App_Start;
using ShoppingWebCrawler.Host.Common.Logging;

namespace ShoppingWebCrawler.Host.WindowService.ScheduleTasks
{

    /// <summary>
    /// 服务任务调度管理
    /// </summary>
    public class ScheduleTaskRunner
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static readonly string QuartzConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "winSer", "quartz.config");

        private static ScheduleTaskRunner _instance;
        /// <summary>
        /// 单例实例
        /// </summary>
        public static ScheduleTaskRunner Instance
        {
            get
            {
                if (null==_instance)
                {
                    _instance = new ScheduleTaskRunner();
                }
                return _instance;
            }

            
        }



        private IScheduler _SchedulerContrller;
        /// <summary>
        /// 调度控制器
        /// </summary>
        protected IScheduler SchedulerContrller { get {
                return this._SchedulerContrller;
            }
        }


        public ScheduleTaskRunner()
        {

            this.InitSchedule();
        }

        /// <summary>
        /// 通过配置初始化 Quartz
        /// </summary>
        public void InitSchedule()
        {
            ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
            filemap.ExeConfigFilename = QuartzConfigPath; 
            var config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);

             
           var quartzProperties = new  NameValueCollection();
            foreach (KeyValueConfigurationElement item in config.AppSettings.Settings)
            {
                quartzProperties.Add(item.Key, item.Value);
            }
           

            IScheduler scheduler = new StdSchedulerFactory(quartzProperties).GetScheduler();
            this._SchedulerContrller = scheduler;// StdSchedulerFactory.GetDefaultScheduler();
        }
        /// <summary>
        /// 开启全部的任务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {

            Logger.Info("服务任务调度 开启......");
            if (!this.SchedulerContrller.IsStarted)
            {
                this.SchedulerContrller.Start();
            }
            return true;
        }
        /// <summary>
        /// 停止全部任务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            Logger.Info("服务任务调度 停止......");
            if (this.SchedulerContrller.IsStarted)
            {
                this.SchedulerContrller.Shutdown(false);
            }
          
            return true;
        }
     
    }
}
