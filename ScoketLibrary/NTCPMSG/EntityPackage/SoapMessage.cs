using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.IO;

using NTCPMessage.Serialize;
using NTCPMessage;

/*
 示范定义传输结构
 自定义序列化 性能最好，注意使用方法
 默认使用内置的 Bin 二进制序列化 ，由于会使用反射对象。降低性能
 要么使用结构 struct  对性能要求较高的可以使用自定义序列化。防止反射
*/
namespace NTCPMessage.EntityPackage
{
    public class SoapMessageSerializer : ISerialize<SoapMessage>
    {
    

        #region ISerialize<TestMessage> Members

        public byte[] GetBytes(ref SoapMessage obj)
        {
            return ((ISerialize)this).GetBytes(obj);
        }

        public SoapMessage GetObject(byte[] data)
        {
            return ((ISerialize)this).GetObject(data) as SoapMessage;
        }

        object ISerialize.GetObject(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            SoapMessage msg = new SoapMessage();
            MemoryStream ms = new MemoryStream(data);
            msg.Id = SimpleBinSerializer.ToString(ms);
            msg.Head = SimpleBinSerializer.ToString(ms);
            msg.Body = SimpleBinSerializer.ToString(ms);

            return msg;
        }

         byte[] ISerialize.GetBytes(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var instance = obj as SoapMessage;
            MemoryStream ms = new MemoryStream();

            SimpleBinSerializer.Write(ms, instance.Id);
            SimpleBinSerializer.Write(ms, instance.Head);
            SimpleBinSerializer.Write(ms, instance.Body);

            return ms.ToArray();
        }

        #endregion
    }

    [Serializable]
    public class SoapMessage
    {
        /// <summary>
        /// 唯一消息的guid 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 消息头（指定使用的Action）
        /// </summary>
        public string Head { get; set; }

        /// <summary>
        /// 消息正文 （json 格式的消息）
        /// </summary>
        public string Body { get; set; }

        // public byte[] Data { get; set; }

        public SoapMessage()
        {
            this.Id = Guid.NewGuid().ToString();
        }
     
    }

}
