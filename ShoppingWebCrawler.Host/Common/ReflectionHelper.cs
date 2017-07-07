using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShoppingWebCrawler.Host
{

    /// <summary>
    /// 实体类反射帮助类
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 根据输入类型获取对应的成员属性集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 根据输入类型获取对应的字段属性集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        }


        /// <summary>
        /// 实现实体的深度克隆（使用二进制序列化进行对象的序列化到流，然后再进行反序列化操作
        /// 对象必须是声明：Serializable
        /// ）
        /// </summary>
        /// <returns></returns>
        public static object CloneData<T>(T data)
        {
            IFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, data);

            ms.Seek(0, SeekOrigin.Begin);

            var obj = bf.Deserialize(ms);

            ms.Flush();
            ms.Close();
            ms.Dispose();

            return obj;
        }

    }

}


