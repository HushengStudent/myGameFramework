/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/20 22:13:33
** desc:  会话工具;
*********************************************************************************/

using System.IO;

namespace Framework
{
    public static class SessionUtil
    {
        public static void Serialize<T>(Session session, MemoryStream destination, T packet) where T : Packet
        {
            var idBytes = ConvertHelper.GetBytes(packet.GetPacketId());
            destination.Write(idBytes, 0, idBytes.Length);
            packet.Serialize(destination);
            ProtoHelper.ReturnPacket(packet);
        }

        public static Packet Deserialize(Session session, MemoryStream source, out object customErrorData)
        {
            customErrorData = null;
            var begin = source.Position;
            var buffer = new byte[4];
            source.Read(buffer, 0, sizeof(int));
            var id = ConvertHelper.GetInt32(buffer);
            var packet = ProtoHelper.GetPacket(id);
            packet.DeSerialize(source);
            return packet;
        }
    }
}