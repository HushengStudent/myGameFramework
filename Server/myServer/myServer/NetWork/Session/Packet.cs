using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class Packet
{
    //TODO:协议ID优化为UShort,减少数据;
    public abstract int GetPacketId();

    public abstract void Serialize(MemoryStream stream);
    public abstract void DeSerialize(MemoryStream stream);

    public abstract void Process();
}
