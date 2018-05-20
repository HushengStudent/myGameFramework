/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/20 22:13:33
** desc:  会话工具;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class SessionUtil
    {
        public static void Serialize<T>(Session session, Stream destination, T packet) where T : Packet
        {
            byte[] idBytes = ConverterUtility.GetBytes(packet.Id);//TODO:优化为UShort,减少数据;
            destination.Write(idBytes, 0, idBytes.Length);
            ProtoBuf.Serializer.Serialize<T>(destination, packet);
        }

        public static Packet Deserialize(Session session, Stream source, out object customErrorData)
        {
            customErrorData = null;
            long begin = source.Position;
            byte[] buffer = new byte[4];
            source.Read(buffer, 0, sizeof(int));
            int id = ConverterUtility.GetInt32(buffer);
            source.Position += sizeof(int);
            return ProtoBuf.Serializer.Deserialize<Packet>(source);
        }
    }
}