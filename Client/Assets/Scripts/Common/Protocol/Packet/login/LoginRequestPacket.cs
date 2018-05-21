using Framework;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoginRequestPacket : Packet
{
    public LoginRequest data = new LoginRequest();

    public override int GetPacketId()
    {
        return 10011;
    }

    public override void DeSerialize(MemoryStream stream)
    {
        ProtoBuf.Serializer.Serialize<LoginRequest>(stream, data);
    }

    public override void Serialize(MemoryStream stream)
    {
        data = ProtoBuf.Serializer.Deserialize<LoginRequest>(stream);
    }

    public override void Process()
    {

    }

}
