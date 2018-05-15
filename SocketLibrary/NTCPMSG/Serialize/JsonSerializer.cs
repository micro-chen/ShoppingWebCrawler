using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NTCPMessage.Serialize
{

    public class JsonSerializer<T> : ISerialize<T> where T : class
    {
      

        #region ISerialize<T> Members

        public byte[] GetBytes(ref T obj)
        {
            return ((ISerialize)this).GetBytes(obj);

        }

        public T GetObject(byte[] data)
        {
            return ((ISerialize)this).GetObject(data) as T;
        }
         byte[] ISerialize.GetBytes(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string sJSON = JsonConvert.SerializeObject(obj);

            return Encoding.UTF8.GetBytes(sJSON);
        }
        object ISerialize.GetObject(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentNullException("To DeserializeObject ,you must pass the data bytes can't be null or zero length!");
            }


            string jsonData = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        #endregion
    }
}
