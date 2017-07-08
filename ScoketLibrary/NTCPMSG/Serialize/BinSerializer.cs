using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NTCPMessage.Serialize
{
    public class BinSerializer : ISerialize
    {
        #region ISerialize Members

        public byte[] GetBytes(object obj)
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

        public object GetObject(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream(data);
            IFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(ms);
        }

        #endregion
    }
}
