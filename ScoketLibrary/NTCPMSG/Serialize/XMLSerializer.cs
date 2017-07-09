using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace NTCPMessage.Serialize
{

    public class XMLSerializer<T> : ISerialize<T> where T : class
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

        byte[] ISerialize.GetBytes(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream();
            TextWriter writer = new StreamWriter(ms, Encoding.UTF8);

            XmlSerializer ser = new XmlSerializer(obj.GetType());

            ser.Serialize(writer, obj);
            return ms.ToArray();
        }
        object ISerialize.GetObject(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }

            MemoryStream ms = new MemoryStream(data);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            return (T)ser.Deserialize(ms);
        }

        #endregion
    }
}
