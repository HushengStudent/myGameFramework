using Framework;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoginResponsePacket : Packet
{
    public LoginResponse data = new LoginResponse();

    public override int GetPacketId()
    {
        return 10012;
    }

    public override void DeSerialize(MemoryStream stream)
    {
        ProtoBuf.Serializer.Serialize<LoginResponse>(stream, data);
    }

    public override void Serialize(MemoryStream stream)
    {
        data = ProtoBuf.Serializer.Deserialize<LoginResponse>(stream);
    }

    public override void Process()
    {

    }
}
