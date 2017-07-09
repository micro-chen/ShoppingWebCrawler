using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
    public class DataResultContainer<T>:IDataContainer
    {
        public DataResultContainer()
        {
            this.Status = 1;
        }
        /// <summary>
        /// 状态 0 失败，1 成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 返回客户端的结果容器
        /// </summary>
        public T Result { get; set; }


    }
}
