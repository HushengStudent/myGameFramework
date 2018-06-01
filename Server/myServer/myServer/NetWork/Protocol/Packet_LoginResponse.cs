using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Packet_LoginResponse : Packet
{
    private LoginResponse data = new LoginResponse();

    public LoginResponse Data { get { return data; } }

    public override int GetPacketId()
    {
        return 10012;
    }

    public override void DeSerialize(MemoryStream stream)
    {
        data = ProtoBuf.Serializer.Deserialize<LoginResponse>(stream);
    }

    public override void Serialize(MemoryStream stream)
    {
        ProtoBuf.Serializer.Serialize<LoginResponse>(stream, data);
    }

    public override void Process()
    {
        Process_LoginResponse.Process(this);
    }
}
