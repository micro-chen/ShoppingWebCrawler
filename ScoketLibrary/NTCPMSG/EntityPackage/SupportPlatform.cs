using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage
{
    public class SupportPlatform
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 平台简称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 平台描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 站点地址
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// 平台枚举根据id编码走
        /// </summary>
        public SupportPlatformEnum Platform
        {
            get
            {
                return (SupportPlatformEnum)this.Id;
            }
        }


    }
}
