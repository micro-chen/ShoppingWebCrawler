using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace ShoppingWebCrawler.Host.Common.Logging
{

    /// <summary>
    /// 日志记录
    /// </summary>
    public sealed class Logger
    {


        #region 属性

        



        /// <summary>
        /// 日志路径 添加上排它锁，保证目录的唯一性
        /// </summary>
        private readonly object _Locker_LogDir = new object();
        //读写锁，当资源处于写入模式时，其他线程写入需要等待本次写入结束之后才能继续写入
        // private  ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim FileLogWriteLock = new ReaderWriterLockSlim();


        /// <summary>
        /// 最大日志文件大小（8M）
        /// </summary>
        private const int MAX_SIZE_LOG_FILE = 1024 * 1024 * 8;

        private string _LogDir;

        /// <summary>
        /// 日志写入的路径
        /// </summary>
        private string LogDir
        {
            get
            {
                lock (_Locker_LogDir)
                {


                    if (string.IsNullOrEmpty(_LogDir))
                    {


                        //首先尝试从配置中加载日志路径
                        string logPath = ConfigHelper.GetConfig("LoggingPath");
                        if (!string.IsNullOrEmpty(logPath))
                        {
                            _LogDir = logPath;
                        }

                        ////如果没有配置日志路径 或者 配置的目录不存在,那么配置默认的日志路径在bin  运行目录
                        if (string.IsNullOrEmpty(_LogDir) || !Directory.Exists(_LogDir))
                        {
                            _LogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                            if (!Directory.Exists(_LogDir))
                            {
                                //需要检测是否存在目录，不存在，那么创建
                                Directory.CreateDirectory(_LogDir);
                            }
                        }

                        //获取配置的应用程序名称
                        var appName = ConfigHelper.GetConfig("AppName");
                        if (!string.IsNullOrEmpty(appName))
                        {
                            _LogDir = Path.Combine(_LogDir, appName);
                        }

                        //监测是否存在路径 不存在的话 创建路径
                        if (!Directory.Exists(_LogDir))
                        {
                            Directory.CreateDirectory(_LogDir);
                        }

                        //检测是否有类型目录
                        string logInfoDir = Path.Combine(_LogDir, LoggingType.Info.ToString());
                        string errorDir = Path.Combine(_LogDir, LoggingType.Error.ToString());
                        if (!Directory.Exists(logInfoDir))
                        {
                            Directory.CreateDirectory(logInfoDir);
                        }
                        if (!Directory.Exists(errorDir))
                        {
                            Directory.CreateDirectory(errorDir);
                        }

                    }
                    return _LogDir;
                }

            }


        }

        private string _InfoLogFilePath;

        /// 队列容器
        public static readonly MessageBulk<LogEventArgs> QueueListOfLogEvent = new MessageBulk<LogEventArgs>();

        #endregion



        /// <summary>
        /// 构造函数
        /// </summary>
        Logger()
        {
        }

        /// <summary>
        /// 创建新的实例
        /// </summary>
        /// <returns></returns>
        internal static Logger CrteatNew()
        {
            return new Logger();
        }


        /// <summary>
        /// 写入异常信息
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteException(Exception ex)
        {
            var agrs = new LogEventArgs() { LogMessage = ex.ToString(), LogType = LoggingType.Error };
            WriteToLog(agrs);
        }

        /// <summary>
        /// 写入日志内容
        /// </summary>
        /// <param name="content"></param>
        public static void WriteToLog(string content)
        {
            WriteToLog(new LogEventArgs { LogMessage = content, LogType = LoggingType.Info });
        }
        /// <summary>
        /// 写入日志内容
        /// 添加判断是否开启日志输出--update by chen wenguang
        /// </summary>
        /// <param name="content"></param>
        public static void WriteToLog(LogEventArgs agrs)
        {
            var isLogging = ConfigHelper.GetConfig("IsLogging");
            if (!string.IsNullOrEmpty(isLogging) && isLogging.ToLower() == "true")
            {
                //将日志消息添加到集合
                QueueListOfLogEvent.SetIntoEnqueue(agrs);
            }


        }



        /// <summary>
        /// 写入日志内容到文件
        /// </summary>
        /// <param name="contentContaienr">日志消息容器</param>
        internal void WriteMessageToLogInternal(LogEventArgs contentContaienr)
        {
            if (null == contentContaienr)
            {
                return;
            }
            var logType = contentContaienr.LogType;
            string content = contentContaienr.LogMessage;
            WriteLogContentToFileAsync(content, logType);
        }


        /// <summary>
        /// 写入日志内容到文件
        /// 异步文件写入，WriteAsync 方法使您能够在不阻塞主线程的情况下执行占用大量资源的 I/O 操作。
        /// </summary>
        /// <param name="content"></param>
        private void WriteLogContentToFileAsync(string content, LoggingType logType)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            //内容格式化
            content = FormatLogContent(content);

            bool isAppend = false;


            var fileFullPath = GenerateFilePath(logType);


            if (File.Exists(fileFullPath))
            {
                isAppend = true;
            }

            //内容追加
            if (isAppend == true)
            {


                using (var fs = new FileStream(fileFullPath, FileMode.Append))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        sr.Write(content);
                    }
                }

            }
            else
            {
                //内容创建

                using (var fs = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        sr.Write(content);
                    }
                }
            }




        }

        /// <summary>
        /// 格式化 消息内容
        /// </summary>
        /// <param name="content"></param>
        private string FormatLogContent(string content)
        {
            StringBuilder sb = new StringBuilder();
            var charOfNewLine = Environment.NewLine;
            //sb.Append("-----------------------------------begin--------------------------------");
            sb.AppendFormat(DateTime.Now.ToString("HH:mm:ss:fff"));
            sb.Append(charOfNewLine);
            sb.Append(content);
            //sb.Append(charOfNewLine);
            //sb.AppendFormat("Excute DateTime：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            //sb.Append(charOfNewLine);
            //sb.Append("-----------------------------------end--------------------------------");
            sb.Append(charOfNewLine);

            return sb.ToString();
        }

        /// <summary>
        /// 生成日志文件全路径
        /// yyyy_MM_dd_HH_mm
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>
        private string GenerateFilePath(LoggingType logType)
        {

            string filePath = string.Empty;


            var token = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");// + "_" + Guid.NewGuid().ToString().Split('-')[0];
            var logTypeDir = logType.ToString();
            filePath = string.Format("{0}\\{1}\\{2}.log", LogDir, logTypeDir, token);


            if (string.IsNullOrEmpty(_InfoLogFilePath))
            {
                _InfoLogFilePath = filePath;
            }


            //当前日志文件路径不为空，那么检查大小是否越界
            var fi = new FileInfo(_InfoLogFilePath);

            if (File.Exists(_InfoLogFilePath) //文件存在
                && fi.Length > MAX_SIZE_LOG_FILE)//超过最大文件上限
            {

                if (fi.CreationTime.AddMinutes(1) < DateTime.Now)
                {
                    //一旦上次创建的文件超过1min  那么不再使用上一分钟的文件名
                    _InfoLogFilePath = filePath;
                }
                else
                {
                    //毫秒追加
                    string miniFileName = string.Concat(Path.GetFileNameWithoutExtension(fi.FullName), "_", DateTime.Now.Millisecond, ".log");

                    _InfoLogFilePath = Path.Combine(fi.DirectoryName, miniFileName);
                }

            }



            return _InfoLogFilePath;

        }

    }
}
