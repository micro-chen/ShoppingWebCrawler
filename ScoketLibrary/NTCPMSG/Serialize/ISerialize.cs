using System;
using System.Collections.Generic;
using System.Text;

namespace NTCPMessage.Serialize
{
    public interface ISerialize
    {
        /// <summary>
        /// Object to bytes
        /// </summary>
        /// <param name="obj">object that will be serialized</param>
        /// <returns>bytes</returns>
        byte[] GetBytes(object obj);

        /// <summary>
        /// bytes to object
        /// </summary>
        /// <param name="data">data will be deserialized</param>
        /// <returns>object</returns>
         object GetObject(byte[] data);
    }

    public interface ISerialize<T>: ISerialize
    {
               /// <summary>
        /// Object to bytes
        /// </summary>
        /// <param name="obj">object that will be serialized</param>
        /// <returns>bytes</returns>
        byte[] GetBytes(ref T obj);

        /// <summary>
        /// bytes to object
        /// </summary>
        /// <param name="data">data will be deserialized</param>
        /// <returns>object</returns>
        new T GetObject(byte[] data);
    }
}
