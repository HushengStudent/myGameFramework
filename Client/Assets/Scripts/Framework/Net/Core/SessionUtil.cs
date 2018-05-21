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
        public static void Serialize<T>(Session session, MemoryStream destination, T packet) where T : Packet
        {
            byte[] idBytes = ConverterUtility.GetBytes(packet.GetPacketId());//TODO:优化为UShort,减少数据;
            destination.Write(idBytes, 0, idBytes.Length);
            packet.Serialize(destination);
        }

        public static Packet Deserialize(Session session, MemoryStream source, out object customErrorData)
        {
            customErrorData = null;
            long begin = source.Position;
            byte[] buffer = new byte[4];
            source.Read(buffer, 0, sizeof(int));
            //TODO:根据id拿到协议反序列化;
            int id = ConverterUtility.GetInt32(buffer);
            source.Position += sizeof(int);
            return ProtoBuf.Serializer.Deserialize<Packet>(source);
        }
    }
}