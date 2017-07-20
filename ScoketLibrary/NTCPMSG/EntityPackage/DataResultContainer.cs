using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
    public class DataContainer : IDataContainer
    {
        public DataContainer()
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
        /// 返回客户端的结果
        /// 如果是对象请转为json格式
        /// </summary>
        public  string Result { get; set; }



        /// <summary>
        /// 返回空的内容消息容器
        /// </summary>
        /// <returns></returns>
        public static IDataContainer CreateNullDataContainer()
        {

            return new DataContainer { Status = 0, ErrorMsg = "参数未能被识别或者为空！" };

        }
    }
}
