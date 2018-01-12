using System;
using System.ComponentModel;
namespace NTCPMessage.EntityPackage
{
    /// <summary>
    /// 电商平台类型枚举
    /// 天猫.....
    /// </summary>
    public enum SupportPlatformEnum
    {
        [Description("天猫")]
        Tmall = 0,
        [Description("淘宝")]
        Taobao = 1,
        [Description("京东")]
        Jingdong = 2,
        [Description("拼多多")]
        Pdd = 3,
        [Description("唯品会")]
        Vip = 4,
        [Description("国美")]
        Guomei = 5,
        [Description("苏宁")]
        Suning = 6,
        [Description("当当")]
        Dangdang = 7,
        [Description("一号店")]
        Yhd = 8,
        //[Description("美丽说")]
        //Meilishuo = 9,
        [Description("蘑菇街")]
        Mogujie = 10,
        //[Description("折800")]
        //Zhe800 = 11,
        [Description("一淘")]
        ETao = 12,
        [Description("阿里妈妈")]
        Alimama = 13
    }
}
