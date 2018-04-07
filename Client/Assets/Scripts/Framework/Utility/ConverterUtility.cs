/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/07 23:11:53
** desc:  类型转换
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class ConverterUtility
    {
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
    }
}
