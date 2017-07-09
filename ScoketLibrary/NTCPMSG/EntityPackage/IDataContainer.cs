using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 返回给客户端的数据容器接口
    /// </summary>
    public interface IDataContainer
    {
        /// <summary>
        /// 状态 0 失败，1 成功
        /// </summary>
         int Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
         string ErrorMsg { get; set; }

    }
}
