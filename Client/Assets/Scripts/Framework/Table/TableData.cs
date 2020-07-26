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
            outputValue = ConvertHelper.GetUInt16(byteArr, byteOffset);
            byteOffset += ConvertHelper.USHORT_LENGTH;
        }

        protected static void ReadShort(ref byte[] byteArr, ref int byteOffset, out short outputValue)
        {
            outputValue = ConvertHelper.GetInt16(byteArr, byteOffset);
            byteOffset += ConvertHelper.SHORT_LENGTH;
        }

        protected static void ReadBoolean(ref byte[] byteArr, ref int byteOffset, out bool outputValue)
        {
            outputValue = ConvertHelper.GetBoolean(byteArr, byteOffset);
            byteOffset += ConvertHelper.BOOL_LENGTH;
        }

        protected static void ReadInt32(ref byte[] byteArr, ref int byteOffset, out int outputValue)
        {
            outputValue = ConvertHelper.GetInt32(byteArr, byteOffset);
            byteOffset += ConvertHelper.INT_LENGTH;
        }
        protected static void ReadUInt32(ref byte[] byteArr, ref int byteOffset, out uint outputValue)
        {
            outputValue = ConvertHelper.GetUInt32(byteArr, byteOffset);
            byteOffset += ConvertHelper.UINT_LENGTH;
        }

        protected static void ReadInt64(ref byte[] byteArr, ref int byteOffset, out long outputValue)
        {
            outputValue = ConvertHelper.GetInt64(byteArr, byteOffset);
            byteOffset += ConvertHelper.LONG_LENGTH;
        }
        protected static void ReadUInt64(ref byte[] byteArr, ref int byteOffset, out ulong outputValue)
        {
            outputValue = ConvertHelper.GetUInt64(byteArr, byteOffset);
            byteOffset += ConvertHelper.ULONG_LENGTH;
        }

        protected static void ReadFloat(ref byte[] byteArr, ref int byteOffset, out float outputValue)
        {
            outputValue = ConvertHelper.GetSingle(byteArr, byteOffset);
            byteOffset += ConvertHelper.FLOAT_LENGTH;
        }

        protected static void ReadInt8(ref byte[] byteArr, ref int byteOffset, out byte outputValue)
        {
            outputValue = byteArr[byteOffset];
            byteOffset += 1;
        }

        protected static void ReadString(ref byte[] byteArr, ref int byteOffset, out string str)
        {
            //第一位为长度;
            ReadInt32(ref byteArr, ref byteOffset, out int len);
            str = Encoding.UTF8.GetString(byteArr, byteOffset, len);
            byteOffset += len;
        }
    }
}