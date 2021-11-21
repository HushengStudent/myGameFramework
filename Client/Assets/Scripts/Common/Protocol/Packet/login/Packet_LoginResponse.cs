using Framework.NetModule;
using Protocol;
using System.IO;

public class Packet_LoginResponse : Packet
{
    public LoginResponse Data { get; private set; } = new LoginResponse();

    public override int GetPacketId()
    {
        return 10012;
    }

    public override void DeSerialize(MemoryStream stream)
    {
        Data = ProtoBuf.Serializer.Deserialize<LoginResponse>(stream);
    }

    public override void Serialize(MemoryStream stream)
    {
        ProtoBuf.Serializer.Serialize(stream, Data);
    }

    public override void Process()
    {
        Process_LoginResponse.Process(this);
    }
}
