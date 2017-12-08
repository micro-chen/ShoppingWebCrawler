using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShoppingWebCrawler.Host.Common.TypeFinder
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
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T entity, string propertyName)
        {
            PropertyInfo property = entity.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return property.FastGetValue(entity);
            }

            return null;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T entity, PropertyInfo property)
        {
            if (property != null)
            {
                return property.FastGetValue(entity);
            }

            return null;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static void SetPropertyValue<T>(T entity, PropertyInfo property,object value)
        {
            if (property != null)
            {
                 property.FastSetValue(entity, value);
            }

        }

        /// <summary>
        /// 获取属性类型
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Type GetPropertyType(Type classType, string propertyName)
        {
            PropertyInfo property = classType.GetProperty(propertyName);
            if (property != null)
            {
                return property.PropertyType;
            }

            return null;
        }







        public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            return FastReflectionCaches.MethodInvokerCache.Get(methodInfo).Invoke(instance, parameters);
        }

        public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value)
        {
            FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).SetValue(instance, value);
        }

        public static object FastGetValue(this PropertyInfo propertyInfo, object instance)
        {
            return FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).GetValue(instance);
        }

        public static object FastGetValue(this FieldInfo fieldInfo, object instance)
        {
            return FastReflectionCaches.FieldAccessorCache.Get(fieldInfo).GetValue(instance);
        }

        public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters)
        {
            return FastReflectionCaches.ConstructorInvokerCache.Get(constructorInfo).Invoke(parameters);
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

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// /// <param name="type">Type of the member.</param>
        public static T GetStaticFieldValue<T>(string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="typeName"></param>
        public static T GetStaticFieldValue<T>(string fieldName, string typeName)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(type);
            }
            return default(T);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public static void SetStaticFieldValue<T>(string fieldName, Type type, T value)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", fieldName));

            field.SetValue(null, value);
        }

        /// <summary>
        /// Sets the value of the private static member.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="typeName"></param>
        /// <param name="value"></param>
        public static void SetStaticFieldValue<T>(string fieldName, string typeName, T value)
        {
            Type type = Type.GetType(typeName, true);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", fieldName));

            field.SetValue(null, value);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="fieldName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        public static T GetPrivateInstanceFieldValue<T>(string fieldName, object source)
        {
            FieldInfo field = source.GetType().GetField(fieldName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(source);
            }
            return default(T);
        }

        /// <summary>
        /// Returns the value of the private member specified.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="source">The object that contains the member.</param>
        /// <param name="value">The value to set the member to.</param>
        public static void SetPrivateInstanceFieldValue(string memberName, object source, object value)
        {
            FieldInfo field = source.GetType().GetField(memberName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                throw new ArgumentException(string.Format("Could not find the private instance field '{0}'", memberName));

            field.SetValue(source, value);
        }

        public static object Instantiate(string typeName)
        {
            return Instantiate(typeName, null, null);
        }

        public static object Instantiate(string typeName, Type[] constructorArgumentTypes, params object[] constructorParameterValues)
        {
            return Instantiate(Type.GetType(typeName, true), constructorArgumentTypes, constructorParameterValues);
        }

        public static object Instantiate(Type type, Type[] constructorArgumentTypes, params object[] constructorParameterValues)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgumentTypes, null);
            return constructor.Invoke(constructorParameterValues);
        }

        /// <summary>
        /// Invokes a non-public static method.
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static TReturn InvokeNonPublicMethod<TReturn>(Type type, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, new Converter<object, Type>(delegate (object o) { return o.GetType(); }));

            MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null, paramTypes, null);
            if (method == null)
                throw new ArgumentException(string.Format("Could not find a method with the name '{0}'", methodName), "method");

            return (TReturn)method.Invoke(null, parameters);
        }

        public static TReturn InvokeNonPublicMethod<TReturn>(object source, string methodName, params object[] parameters)
        {
            Type[] paramTypes = Array.ConvertAll(parameters, new Converter<object, Type>(delegate (object o) { return o.GetType(); }));

            MethodInfo method = source.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, paramTypes, null);
            if (method == null)
                throw new ArgumentException(string.Format("Could not find a method with the name '{0}'", methodName), "method");

            return (TReturn)method.Invoke(source, parameters);
        }

        public static TReturn InvokeProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");

            return (TReturn)propertyInfo.GetValue(source, null);
        }

        public static TReturn InvokeNonPublicProperty<TReturn>(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance, null, typeof(TReturn), new Type[0], null);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");

            return (TReturn)propertyInfo.GetValue(source, null);
        }

        public static object InvokeNonPublicProperty(object source, string propertyName)
        {
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Could not find a propertyName with the name '{0}'", propertyName), "propertyName");

            return propertyInfo.GetValue(source, null);
        }

    }

}


