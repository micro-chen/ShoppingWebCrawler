using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NTCPMessage.Serialize
{
    public class BinSerializer<T> : ISerialize<T> where T :class
    {
   


        #region ISerialize Members

        public byte[] GetBytes(ref T obj)
        {
            return ((ISerialize)this).GetBytes(obj);
        }

        public T GetObject(byte[] data)
        {
            return ((ISerialize)this).GetObject(data) as T;
        }

      
        object ISerialize.GetObject(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }

            MemoryStream ms = new MemoryStream(data);
            IFormatter formatter = new BinaryFormatter();
            var obj = formatter.Deserialize(ms);
            return obj as T;
        }

         byte[] ISerialize.GetBytes(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Flush();

            return ms.ToArray();
        }

        #endregion
    }
}
