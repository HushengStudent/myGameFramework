/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 20:56:49
** desc:  配置表基类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework
{
    public abstract class TableData
    {
        //主键key;
        public abstract int Key { get; }

        public abstract void Decode(byte[] byteArr, ref int bytePos);

        protected static void ReadUShort(ref byte[] byteArr, ref int byteOffset, out ushort outputValue)
        {
            outputValue = ConverterUtility.GetUInt16(byteArr, byteOffset);
            byteOffset += ConverterUtility.USHORT_LENGTH;
        }

        protected static void ReadShort(ref byte[] byteArr, ref int byteOffset, out short outputValue)
        {
            outputValue = ConverterUtility.GetInt16(byteArr, byteOffset);
            byteOffset += ConverterUtility.SHORT_LENGTH;
        }

        protected static void ReadBoolean(ref byte[] byteArr, ref int byteOffset, out bool outputValue)
        {
            outputValue = ConverterUtility.GetBoolean(byteArr, byteOffset);
            byteOffset += ConverterUtility.BOOL_LENGTH;
        }

        protected static void ReadInt32(ref byte[] byteArr, ref int byteOffset, out int outputValue)
        {
            outputValue = ConverterUtility.GetInt32(byteArr, byteOffset);
            byteOffset += ConverterUtility.INT_LENGTH;
        }
        protected static void ReadUInt32(ref byte[] byteArr, ref int byteOffset, out uint outputValue)
        {
            outputValue = ConverterUtility.GetUInt32(byteArr, byteOffset);
            byteOffset += ConverterUtility.UINT_LENGTH;
        }

        protected static void ReadInt64(ref byte[] byteArr, ref int byteOffset, out long outputValue)
        {
            outputValue = ConverterUtility.GetInt64(byteArr, byteOffset);
            byteOffset += ConverterUtility.LONG_LENGTH;
        }
        protected static void ReadUInt64(ref byte[] byteArr, ref int byteOffset, out ulong outputValue)
        {
            outputValue = ConverterUtility.GetUInt64(byteArr, byteOffset);
            byteOffset += ConverterUtility.ULONG_LENGTH;
        }

        protected static void ReadFloat(ref byte[] byteArr, ref int byteOffset, out float outputValue)
        {
            outputValue = ConverterUtility.GetSingle(byteArr, byteOffset);
            byteOffset += ConverterUtility.FLOAT_LENGTH;
        }

        protected static void ReadInt8(ref byte[] byteArr, ref int byteOffset, out byte outputValue)
        {
            outputValue = byteArr[byteOffset];
            byteOffset += 1;
        }

        protected static void ReadString(ref byte[] byteArr, ref int byteOffset, out string str)
        {
            ushort len;
            //第一位为长度;
            ReadUShort(ref byteArr, ref byteOffset, out len);
            str = Encoding.UTF8.GetString(byteArr, byteOffset, len);
            byteOffset += len;
        }
    }
}