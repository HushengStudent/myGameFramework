/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/07 23:11:53
** desc:  类型转换;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework
{
    public static class ConverterUtility
    {

        #region bool

        private static int _boolLength = 1;

        public static int BOOL_LENGTH { get { return _boolLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的布尔值;
        /// </summary>
        /// <param name="value">要转换的布尔值;</param>
        /// <returns>长度为1的字节数组;</returns>
        public static byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中首字节转换来的布尔值;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>如果value中的首字节非零,则为 true,否则为 false;</returns>
        public static bool GetBoolean(byte[] value)
        {
            return BitConverter.ToBoolean(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的一个字节转换来的布尔值;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value 内的起始位置;</param>
        /// <returns>如果 value 中指定位置的字节非零,则为true,否则为false;</returns>
        public static bool GetBoolean(byte[] value, int startIndex)
        {
            return BitConverter.ToBoolean(value, startIndex);
        }

        #endregion

        #region int

        private static int _intLength = 4;

        public static int INT_LENGTH { get { return _intLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的32位有符号整数值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为4的字节数组;</returns>
        public static byte[] GetBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前四个字节转换来的32位有符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由四个字节构成的32位有符号整数;</returns>
        public static int GetInt32(byte[] value)
        {
            return BitConverter.ToInt32(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的32位有符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由四个字节构成的32位有符号整数;</returns>
        public static int GetInt32(byte[] value, int startIndex)
        {
            return BitConverter.ToInt32(value, startIndex);
        }

        #endregion

        #region uint

        private static int _uintLength = 4;

        public static int UINT_LENGTH { get { return _uintLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的32位无符号整数值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为4的字节数组;</returns>
        public static byte[] GetBytes(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前四个字节转换来的32位无符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由四个字节构成的32位无符号整数;</returns>
        public static uint GetUInt32(byte[] value)
        {
            return BitConverter.ToUInt32(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的32位无符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由四个字节构成的32位无符号整数;</returns>
        public static uint GetUInt32(byte[] value, int startIndex)
        {
            return BitConverter.ToUInt32(value, startIndex);
        }

        #endregion

        #region char

        private static int _charLength = 2;

        public static int CHAR_LENGTH { get { return _charLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的Unicode字符值;
        /// </summary>
        /// <param name="value">要转换的字符;</param>
        /// <returns>长度为2的字节数组;</returns>
        public static byte[] GetBytes(char value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前两个字节转换来的Unicode字符;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由两个字节构成的字符;</returns>
        public static char GetChar(byte[] value)
        {
            return BitConverter.ToChar(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的两个字节转换来的Unicode字符;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由两个字节构成的字符;</returns>
        public static char GetChar(byte[] value, int startIndex)
        {
            return BitConverter.ToChar(value, startIndex);
        }

        #endregion

        #region short

        private static int _shortLength = 2;

        public static int SHORT_LENGTH { get { return _shortLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的16位有符号整数值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为2的字节数组;</returns>
        public static byte[] GetBytes(short value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前两个字节转换来的16位有符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由两个字节构成的16位有符号整数;</returns>
        public static short GetInt16(byte[] value)
        {
            return BitConverter.ToInt16(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的两个字节转换来的16位有符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由两个字节构成的16位有符号整数;</returns>
        public static short GetInt16(byte[] value, int startIndex)
        {
            return BitConverter.ToInt16(value, startIndex);
        }

        #endregion

        #region ushort

        private static int _ushortLength = 2;

        public static int USHORT_LENGTH { get { return _ushortLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的16位无符号整数值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为2的字节数组;</returns>
        public static byte[] GetBytes(ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前两个字节转换来的16位无符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由两个字节构成的16位无符号整数;</returns>
        public static ushort GetUInt16(byte[] value)
        {
            return BitConverter.ToUInt16(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的两个字节转换来的16位无符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由两个字节构成的16位无符号整数;</returns>
        public static ushort GetUInt16(byte[] value, int startIndex)
        {
            return BitConverter.ToUInt16(value, startIndex);
        }

        #endregion

        #region long

        private static int _longLength = 8;

        public static int LONG_LENGTH { get { return _longLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的64位有符号整数值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为8的字节数组;</returns>
        public static byte[] GetBytes(long value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前八个字节转换来的64位有符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由八个字节构成的64位有符号整数;</returns>
        public static long GetInt64(byte[] value)
        {
            return BitConverter.ToInt64(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的八个字节转换来的64位有符号整数;
        /// </summary>
        /// <param name="value">字节数组。</param>
        /// <param name="startIndex">value内的起始位置</param>
        /// <returns>由八个字节构成的64位有符号整数;</returns>
        public static long GetInt64(byte[] value, int startIndex)
        {
            return BitConverter.ToInt64(value, startIndex);
        }

        #endregion

        #region ulong

        private static int _ulongLength = 8;

        public static int ULONG_LENGTH { get { return _ulongLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的64位无符号整数值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为8的字节数组;</returns>
        public static byte[] GetBytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前八个字节转换来的64位无符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由八个字节构成的64位无符号整数;</returns>
        public static ulong GetUInt64(byte[] value)
        {
            return BitConverter.ToUInt64(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的八个字节转换来的64位无符号整数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由八个字节构成的64位无符号整数;</returns>
        public static ulong GetUInt64(byte[] value, int startIndex)
        {
            return BitConverter.ToUInt64(value, startIndex);
        }

        #endregion

        #region float

        private static int _floatLength = 4;

        public static int FLOAT_LENGTH { get { return _floatLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的单精度浮点值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为4的字节数组;</returns>
        public static byte[] GetBytes(float value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前四个字节转换来的单精度浮点数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由四个字节构成的单精度浮点数;</returns>
        public static float GetSingle(byte[] value)
        {
            return BitConverter.ToSingle(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的单精度浮点数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由四个字节构成的单精度浮点数;</returns>
        public static float GetSingle(byte[] value, int startIndex)
        {
            return BitConverter.ToSingle(value, startIndex);
        }

        #endregion

        #region double

        private static int _doubleLength = 8;

        public static int DOUBLE_LENGTH { get { return _doubleLength; } }

        /// <summary>
        /// 以字节数组的形式返回指定的双精度浮点值;
        /// </summary>
        /// <param name="value">要转换的数字;</param>
        /// <returns>长度为8的字节数组;</returns>
        public static byte[] GetBytes(double value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 返回由字节数组中前八个字节转换来的双精度浮点数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <returns>由八个字节构成的双精度浮点数;</returns>
        public static double GetDouble(byte[] value)
        {
            return BitConverter.ToDouble(value, 0);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的八个字节转换来的双精度浮点数;
        /// </summary>
        /// <param name="value">字节数组;</param>
        /// <param name="startIndex">value内的起始位置;</param>
        /// <returns>由八个字节构成的双精度浮点数;</returns>
        public static double GetDouble(byte[] value, int startIndex)
        {
            return BitConverter.ToDouble(value, startIndex);
        }

        #endregion

        #region string

        /// <summary>
        /// 以UTF-8字节数组的形式返回指定的字符串;
        /// </summary>
        /// <param name="value">要转换的字符串;</param>
        /// <returns>UTF-8字节数组;</returns>
        public static byte[] GetBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// 返回由UTF-8字节数组转换来的字符串;
        /// </summary>
        /// <param name="value">UTF-8字节数组;</param>
        /// <returns>字符串;</returns>
        public static string GetString(byte[] value)
        {
            if (value == null)
            {
                throw new Exception("Value is invalid.");
            }
            return Encoding.UTF8.GetString(value, 0, value.Length);
        }

        #endregion

    }
}
