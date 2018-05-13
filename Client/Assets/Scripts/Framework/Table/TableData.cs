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

        public abstract void Decode(byte[] byteArr, ref int bytePos);

        protected static void ReadUShort(ref byte[] byteArray, ref int mByteOffset, out ushort outputValue)
        {
            outputValue = ConverterUtility.GetUInt16(byteArray, mByteOffset);
            mByteOffset += ConverterUtility.USHORT_LENGTH;
        }

        protected static void ReadShort(ref byte[] byteArray, ref int mByteOffset, out short outputValue)
        {
            outputValue = ConverterUtility.GetInt16(byteArray, mByteOffset);
            mByteOffset += ConverterUtility.SHORT_LENGTH;
        }

        protected static void ReadInt32(ref byte[] byteArray, ref int mByteOffset, out int outputValue)
        {
            outputValue = ConverterUtility.GetInt32(byteArray, mByteOffset);
            mByteOffset += ConverterUtility.INT_LENGTH;
        }
        protected static void ReadUInt32(ref byte[] byteArray, ref int mByteOffset, out uint outputValue)
        {
            outputValue = ConverterUtility.GetUInt32(byteArray, mByteOffset);
            mByteOffset += ConverterUtility.UINT_LENGTH;
        }

        protected static void ReadInt64(ref byte[] byteArray, ref int mByteOffset, out long outputValue)
        {
            outputValue = ConverterUtility.GetInt64(byteArray, mByteOffset);
            mByteOffset += ConverterUtility.LONG_LENGTH;
        }
        protected static void ReadUInt64(ref byte[] byteArray, ref int mByteOffset, out ulong outputValue)
        {
            outputValue = ConverterUtility.GetUInt64(byteArray, mByteOffset);
            mByteOffset += ConverterUtility.ULONG_LENGTH;
        }

        protected static void ReadInt8(ref byte[] byteArray, ref int mByteOffset, out byte outputValue)
        {
            outputValue = byteArray[mByteOffset];
            mByteOffset += 1;
        }

        protected static void ReadString(ref byte[] byteArray, ref int mByteOffset, out string str)
        {
            ushort len;
            //第一位为长度;
            ReadUShort(ref byteArray, ref mByteOffset, out len);
            str = Encoding.UTF8.GetString(byteArray, mByteOffset, len);
            mByteOffset += len;
        }

    }
}