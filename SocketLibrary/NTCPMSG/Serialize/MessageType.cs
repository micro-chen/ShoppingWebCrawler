using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.Serialize
{

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 基础类型消息 字符串消息
        /// </summary>
        None = 20,
        /// <summary>
        /// 二进制消息
        /// </summary>
        Bin = 21,
        /// <summary>
        /// XML消息
        /// </summary>
        Xml = 22,
        /// <summary>
        /// Json消息
        /// </summary>
        Json = 23,
        /// <summary>
        /// 简单二进制消息
        /// </summary>
        SimpleBin = 24,
        /// <summary>
        /// 自定义格式消息
        /// </summary>
        Customer=25
    }
}
