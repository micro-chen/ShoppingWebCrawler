using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NTCPMessage.Serialize
{
    public class JsonSerializer : ISerialize
    {
        Type _DataType;

        public JsonSerializer(Type dataType)
        {
            _DataType = dataType; 
        }

        #region ISerialize Members
        /// <summary>
        /// 转换对象为字节
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public byte[] GetBytes(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string sJSON = JsonConvert.SerializeObject(obj);

            return Encoding.UTF8.GetBytes(sJSON);

        }

        /// <summary>
        /// 将字节转换为对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public object GetObject(byte[] data)
        {
            if (data == null)
            {
                return null;
            }


            string jsonData = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject(jsonData, _DataType);

        }

        #endregion
    }

    public class JsonSerializer<T> : ISerialize<T>
    {

        #region ISerialize<T> Members

        public byte[] GetBytes(ref T obj)
        {
            if (obj == null)
            {
                return null;
            }

            string sJSON = JsonConvert.SerializeObject(obj);

            return Encoding.UTF8.GetBytes(sJSON);

        }

        public T GetObject(byte[] data)
        {
            if (data == null||data.Length<=0)
            {
                throw new ArgumentNullException("To DeserializeObject ,you must pass the data bytes can't be null or zero length!");
            }


            string jsonData = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(jsonData);


        }

        #endregion
    }
}
