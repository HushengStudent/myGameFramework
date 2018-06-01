using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class SessionUtil
{
    public static void Serialize<T>(Session session, MemoryStream destination, T packet) where T : Packet
    {
        byte[] idBytes = ConverterUtility.GetBytes(packet.GetPacketId());
        destination.Write(idBytes, 0, idBytes.Length);
        packet.Serialize(destination);
        ProtoRegister.ReturnPacket(packet);
    }

    public static Packet Deserialize(Session session, MemoryStream source, out object customErrorData)
    {
        customErrorData = null;
        long begin = source.Position;
        byte[] buffer = new byte[4];
        source.Read(buffer, 0, sizeof(int));
        int id = ConverterUtility.GetInt32(buffer);
        Packet packet = ProtoRegister.GetPacket(id);
        packet.DeSerialize(source);
        return packet;
    }
}