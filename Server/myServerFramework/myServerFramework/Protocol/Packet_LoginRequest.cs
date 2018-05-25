using myServerFramework;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Packet_LoginRequest : Packet
{
    private LoginRequest data = new LoginRequest();

    public LoginRequest Data { get { return data; } }

    public override int GetPacketId()
    {
        return 10011;
    }

    public override void DeSerialize(MemoryStream stream)
    {
        data = ProtoBuf.Serializer.Deserialize<LoginRequest>(stream);
    }

    public override void Serialize(MemoryStream stream)
    {
        ProtoBuf.Serializer.Serialize<LoginRequest>(stream, data);
    }

    public override void Process()
    {
        Process_LoginRequest.Process(this);
    }
}
