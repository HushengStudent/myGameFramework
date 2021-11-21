using Framework.NetModule;
using Protocol;
using System.IO;

public class Packet_LoginRequest : Packet
{
    public LoginRequest Data { get; private set; } = new LoginRequest();

    public override int GetPacketId()
    {
        return 10011;
    }

    public override void DeSerialize(MemoryStream stream)
    {
        Data = ProtoBuf.Serializer.Deserialize<LoginRequest>(stream);
    }

    public override void Serialize(MemoryStream stream)
    {
        ProtoBuf.Serializer.Serialize(stream, Data);
    }

    public override void Process()
    {
        Process_LoginRequest.Process(this);
    }
}
