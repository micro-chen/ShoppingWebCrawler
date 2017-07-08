using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace NTCPMessage.Serialize
{
    public class XMLSerializer : ISerialize
    {
        Type _DataType;

        public XMLSerializer(Type dataType)
        {
            _DataType = dataType; 
        }

        #region ISerialize Members

        public byte[] GetBytes(object obj)
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

        public object GetObject(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream(data);
            XmlSerializer ser = new XmlSerializer(_DataType);
            TextReader reader = new StreamReader(ms, Encoding.UTF8);
            return ser.Deserialize(reader);
        }

        #endregion
    }

    public class XMLSerializer<T> : ISerialize<T>
    {
        #region ISerialize Members

        public byte[] GetBytes(ref T obj)
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

        public T GetObject(byte[] data)
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
