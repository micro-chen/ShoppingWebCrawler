using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace NTCPMessage.Serialize
{
    /// <summary>
    /// This serializer only can be used for the class that only include simple data type properties
    /// </summary>
    public class SimpleBinSerializer : ISerialize
    {
        #region static methods

        /// <summary>
        /// Get bytes from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="count">count of bytes</param>
        /// <returns></returns>
        static byte[] GetBytes(Stream s, int count)
        {
            int offset = 0;
            byte[] ret = new byte[count];

            while (offset < count)
            {
                int cnt = s.Read(ret, offset, count - offset);
                
                if (cnt <= 0)
                {
                    throw new IOException("Reach end of the stream");
                }

                offset += cnt;
            }

            return ret;
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, sbyte value)
        {
            s.WriteByte((byte)value);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, byte value)
        {
            s.WriteByte(value);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, short value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, ushort value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, int value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, uint value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }


        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, long value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, ulong value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, float value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, double value)
        {
            byte[] buf = LittleEndianBitConverter.GetBytes(value);

            s.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, string value)
        {
            if (value == null)
            {
                byte[] lenBuf = LittleEndianBitConverter.GetBytes((int)0);//data length
            }
            else
            {
                byte[] buf = Encoding.UTF8.GetBytes((string)value);
                byte[] lenBuf = LittleEndianBitConverter.GetBytes((int)buf.Length);
                s.Write(lenBuf, 0, lenBuf.Length); //data length
                s.Write(buf, 0, buf.Length);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, byte[] value)
        {
            if (value == null)
            {
                byte[] lenBuf = LittleEndianBitConverter.GetBytes((int)0);//data length
            }
            else
            {
                byte[] buf = value;
                byte[] lenBuf = LittleEndianBitConverter.GetBytes((int)buf.Length);
                s.Write(lenBuf, 0, lenBuf.Length); //data length
                s.Write(buf, 0, buf.Length);
            }
        }


        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, sbyte? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(sbyte)); //length
                s.WriteByte((byte)value);
            }
        }


        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, byte? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(byte)); //length
                s.WriteByte((byte)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, short? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(short)); //length
                Write(s, (short)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, ushort? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(ushort)); //length
                Write(s, (ushort)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, int? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(int)); //length
                Write(s, (int)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, uint? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(uint)); //length
                Write(s, (uint)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, long? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(long)); //length
                Write(s, (long)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, ulong? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(ulong)); //length
                Write(s, (ulong)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, float? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(float)); //length
                Write(s, (float)value);
            }
        }

        /// <summary>
        /// Write value to stream.
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="value">value</param>
        static public void Write(Stream s, double? value)
        {
            if (value == null)
            {
                s.WriteByte(0); //length
            }
            else
            {
                s.WriteByte(sizeof(double)); //length
                Write(s, (double)value);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static sbyte ToSByte(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(sbyte));

            return (sbyte)buf[0];
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static byte ToByte(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(byte));

            return (byte)buf[0];
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static short ToInt16(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(short));

            return LittleEndianBitConverter.ToInt16(buf, 0);
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static ushort ToUInt16(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(ushort));

            return LittleEndianBitConverter.ToUInt16(buf, 0);
        }


        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static int ToInt32(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(int));

            return LittleEndianBitConverter.ToInt16(buf, 0);
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static uint ToUInt32(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(uint));

            return LittleEndianBitConverter.ToUInt16(buf, 0);
        }


        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static long ToInt64(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(long));

            return LittleEndianBitConverter.ToInt16(buf, 0);
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static ulong ToUInt64(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(ulong));

            return LittleEndianBitConverter.ToUInt16(buf, 0);
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static float ToSingle(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(ulong));

            return LittleEndianBitConverter.ToSingle(buf, 0);
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static double ToDouble(Stream s)
        {
            byte[] buf = GetBytes(s, sizeof(ulong));

            return LittleEndianBitConverter.ToDouble(buf, 0);
        }


        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static string ToString(Stream s)
        {
            byte[] lenBuf = new byte[sizeof(int)];

            s.Read(lenBuf, 0, lenBuf.Length);

            int len = LittleEndianBitConverter.ToInt32(lenBuf, 0);

            if (len == 0)
            {
                return null;
            }
            else
            {
                byte[] buf = new byte[len];
                s.Read(buf, 0, len);
                return Encoding.UTF8.GetString(buf, 0, buf.Length);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static byte[] ToData(Stream s)
        {
            byte[] lenBuf = new byte[sizeof(int)];

            s.Read(lenBuf, 0, lenBuf.Length);

            int len = LittleEndianBitConverter.ToInt32(lenBuf, 0);

            if (len == 0)
            {
                return null;
            }
            else
            {
                byte[] buf = new byte[len];
                s.Read(buf, 0, len);
                return buf;
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static sbyte? ToNullableSByte(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToSByte(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static byte? ToNullableByte(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToByte(s);
            }
        }


        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static short? ToNullableInt16(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToInt16(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static ushort? ToNullableUInt16(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToUInt16(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static int? ToNullableInt32(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToInt32(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static uint? ToNullableUInt32(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToUInt32(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static long? ToNullableInt64(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToInt64(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static ulong? ToNullableUInt64(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToUInt64(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static float? ToNullableSingle(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToSingle(s);
            }
        }

        /// <summary>
        /// Read data from stream
        /// </summary>
        /// <param name="s">stream</param>
        /// <returns>data of specified type</returns>
        public static double? ToNullableDouble(Stream s)
        {
            int len = s.ReadByte();

            if (len == 0)
            {
                return null;
            }
            else
            {
                return ToDouble(s);
            }
        }
        #endregion

        Type _DataType;

        public SimpleBinSerializer(Type dataType)
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

            foreach (PropertyInfo pi in _DataType.GetProperties())
            {
                Type type = pi.PropertyType;

                object value = pi.GetValue(obj, null);

                if (type == typeof(sbyte))
                {
                    Write(ms, (sbyte)value);
                }
                else if (type == typeof(byte))
                {
                    Write(ms, (byte)value);
                }
                else if (type == typeof(short))
                {
                    Write(ms, (short)value);
                }
                else if (type == typeof(ushort))
                {
                    Write(ms, (ushort)value);
                }
                else if (type == typeof(int))
                {
                    Write(ms, (int)value);
                }
                else if (type == typeof(uint))
                {
                    Write(ms, (uint)value);
                }
                else if (type == typeof(long))
                {
                    Write(ms, (long)value);
                }
                else if (type == typeof(ulong))
                {
                    Write(ms, (ulong)value);
                }
                else if (type == typeof(float))
                {
                    Write(ms, (float)value);
                }
                else if (type == typeof(double))
                {
                    Write(ms, (double)value);
                }
                else if (type == typeof(string))
                {
                    Write(ms, (string)value);
                }
                else if (type == typeof(byte[]))
                {
                    Write(ms, (byte[])value);
                }
            }

            return ms.ToArray();
        }

        public object GetObject(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream(data);
            object obj = _DataType.Assembly.CreateInstance(_DataType.FullName);

            foreach (PropertyInfo pi in _DataType.GetProperties())
            {
                Type type = pi.PropertyType;

                if (type == typeof(sbyte))
                {
                    pi.SetValue(obj, ToSByte(ms), null);
                }
                else if (type == typeof(byte))
                {
                    pi.SetValue(obj, ToByte(ms), null);
                }
                else if (type == typeof(short))
                {
                    pi.SetValue(obj, ToInt16(ms), null);
                }
                else if (type == typeof(ushort))
                {
                    pi.SetValue(obj, ToUInt16(ms), null);
                }
                else if (type == typeof(int))
                {
                    pi.SetValue(obj, ToInt32(ms), null);
                }
                else if (type == typeof(uint))
                {
                    pi.SetValue(obj, ToUInt32(ms), null);
                }
                else if (type == typeof(long))
                {
                    pi.SetValue(obj, ToInt64(ms), null);
                }
                else if (type == typeof(ulong))
                {
                    pi.SetValue(obj, ToUInt64(ms), null);
                }
                else if (type == typeof(float))
                {
                    pi.SetValue(obj, ToSingle(ms), null);
                }
                else if (type == typeof(double))
                {
                    pi.SetValue(obj, ToDouble(ms), null);
                }
                else if (type == typeof(string))
                {
                    pi.SetValue(obj, ToString(ms), null);
                }
                else if (type == typeof(byte[]))
                {
                    pi.SetValue(obj, ToData(ms), null);
                }
            }

            return obj;
        }

        #endregion
    }
}
