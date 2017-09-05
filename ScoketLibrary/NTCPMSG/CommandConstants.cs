using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage
{
    /// <summary>
    /// SOAP命令
    /// </summary>
    public static class CommandConstants
    {
        /// <summary>
        /// 支持的电商平台集合
        /// </summary>
        public const string CMD_Platforms = "platforms";
        /// <summary>
        /// 抓取网页
        /// </summary>
        public const string CMD_FetchPage = "fetch_page";
        /// <summary>
        /// 检索优惠券是否存在
        /// </summary>
        public const string CMD_FetchquanExistsList = "quan_exists_list";

        /// <summary>
        /// 检索指定商品的券详细
        /// </summary>
        public const string CMD_FetchquanDetails = "quan_details";
    }
}
