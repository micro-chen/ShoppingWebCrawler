using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace System
{
    public static class EnumExtensions
    {


        /// <summary>
        /// 获取枚举值 整数值的字符串
        /// </summary>
        /// <param name="emObj"></param>
        /// <returns></returns>
        public static string GetEnumValueString(this Enum emObj)
        {
            var type = emObj.GetType();
            var name = Enum.GetName(type, emObj);
            var value = Convert.ToInt32(Enum.Parse(type, name));

            return value.ToString();
        }
        /// <summary>
        /// 获取枚举值上的Description特性的说明
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="obj">枚举值</param>
        /// <returns>特性的说明</returns>
        public static string GetEnumDescription(this Enum emObj)
        {
            var type = emObj.GetType();
            FieldInfo field = type.GetField(Enum.GetName(type, emObj));
            DescriptionAttribute descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descAttr == null)
            {
                return string.Empty;
            }

            return descAttr.Description;
        }
    

    }
}
