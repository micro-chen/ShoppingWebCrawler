using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingWebCrawler.Host.Logging
{

    /// <summary>
    /// A delegate used for log.
    /// </summary>
    /// <param name="logMsg">The msg to write to log.</param>
    public delegate void LogHandler(LogEventArgs args);

    /// <summary>
    /// 日志类型
    /// </summary>
    [Serializable]
    public enum LoggingType
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        Info = 1,
        /// <summary>
        /// 错误信息
        /// </summary>
        Error = 2

    }

    /// <summary>
    /// 日志器的状态
    /// </summary>
    [Serializable]
    public enum LoggingStateEnum
    {
        /// <summary>
        /// 准备完毕，可以进行
        /// </summary>
        Ready = 0,
        /// <summary>
        /// 正在执行
        /// </summary>
        Excuting = 1
    }

    /// <summary>
    /// 写入日志的事件参数
    /// </summary>
    [Serializable]
    public sealed class LogEventArgs : EventArgs
    {
        /// <summary>
        /// 日志消息
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public LoggingType LogType { get; set; }
    }
}
