using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 注册端口到主节点
    /// </summary>
    public class RegisterPortArgument
    {
        /// <summary>
        /// 从节点的guid编码
        /// </summary>
        public string SlaveIdentity { get; set; }
    }
}
