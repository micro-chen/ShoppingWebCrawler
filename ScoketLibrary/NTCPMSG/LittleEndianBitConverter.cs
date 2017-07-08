/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace NTCPMessage
{
    /// <summary>
    /// Convert with little endian 
    /// </summary>
    public class LittleEndianBitConverter
    {
        private static void ReverseBytes(byte[] array, int startIndex, int length)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(array, startIndex, length);
            }
        }

        #region To(PrimitiveType) conversions
        /// <summary>
        /// Returns a Boolean value converted from one byte at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>true if the byte at startIndex in value is nonzero; otherwise, false.</returns>
        static public bool ToBoolean(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(bool));
            return BitConverter.ToBoolean(value, startIndex);
        }

        /// <summary>
        /// Returns a Unicode character converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A character formed by two bytes beginning at startIndex.</returns>
        static public char ToChar(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(bool));
            return BitConverter.ToChar(value, startIndex);
        }

        /// <summary>
        /// Returns a single-precision floating point number converted from four bytes 
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A single precision floating point number formed by four bytes beginning at startIndex.</returns>
        static public float ToSingle(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(float));
            return BitConverter.ToSingle(value, startIndex);
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
        static public short ToInt16(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(short));
            return BitConverter.ToInt16(value, startIndex);
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit signed integer formed by four bytes beginning at startIndex.</returns>
        static public int ToInt32(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(int));
            return BitConverter.ToInt32(value, startIndex);
        }

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
        static public long ToInt64(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(long));
            return BitConverter.ToInt64(value, startIndex);
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
        static public ushort ToUInt16(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(ushort));
            return BitConverter.ToUInt16(value, startIndex);
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
        static public uint ToUInt32(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(uint));
            return BitConverter.ToUInt32(value, startIndex);
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
        static public ulong ToUInt64(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(ulong));
            return BitConverter.ToUInt64(value, startIndex);
        }

        /// <summary>
        /// Returns a double-precision floating point number converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes. </param>
        /// <param name="startIndex">The starting position within value. </param>
        /// <returns>A double precision floating point number formed by eight bytes beginning at startIndex.</returns>
        static public double ToDouble(byte[] value, int startIndex)
        {
            ReverseBytes(value, startIndex, sizeof(double));
            return BitConverter.ToDouble(value, startIndex);
        }

        #endregion

        #region ToString conversions
        /// <summary>
        /// Returns a String converted from the elements of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <remarks>All the elements of value are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair 
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
        public static string ToString(byte[] value)
        {
            return BitConverter.ToString(value);
        }

        /// <summary>
        /// Returns a String converted from the elements of a byte array starting at a specified array position.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <remarks>The elements from array position startIndex to the end of the array are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair 
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
        public static string ToString(byte[] value, int startIndex)
        {
            return BitConverter.ToString(value, startIndex);
        }

        /// <summary>
        /// Returns a String converted from a specified number of bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <param name="length">The number of bytes to convert.</param>
        /// <remarks>The length elements from array position startIndex are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair 
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
        public static string ToString(byte[] value, int startIndex, int length)
        {
            return BitConverter.ToString(value, startIndex, length);
        }
        #endregion

        #region GetBytes conversions

        /// <summary>
        /// Returns the specified Boolean value as an array of bytes.
        /// </summary>
        /// <param name="value">A Boolean value.</param>
        /// <returns>An array of bytes with length 1.</returns>
        static public byte[] GetBytes(bool value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified Unicode character value as an array of bytes.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        static public byte[] GetBytes(char value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified 16-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        static public byte[] GetBytes(short value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        static public byte[] GetBytes(int value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified 64-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        static public byte[] GetBytes(long value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        //
        // Summary:
        //     Returns the specified double-precision floating point value as an array of
        //     bytes.
        //
        // Parameters:
        //   value:
        //     The number to convert.
        //
        // Returns:
        //     An array of bytes with length 8.
        public static byte[] GetBytes(double value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified single-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        static public byte[] GetBytes(float value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified 16-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
        static public byte[] GetBytes(ushort value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
        static public byte[] GetBytes(uint value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        /// <summary>
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
        static public byte[] GetBytes(ulong value)
        {
            byte[] ret = BitConverter.GetBytes(value);
            ReverseBytes(ret, 0, ret.Length);
            return ret;
        }

        #endregion
  
    }
}
