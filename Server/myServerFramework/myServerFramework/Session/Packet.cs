/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/08 00:05:29
** desc:  网络协议包;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace myServerFramework
{
    public abstract class Packet
    {
        //TODO:协议ID优化为UShort,减少数据;
        public abstract int GetPacketId();

        public abstract void Serialize(MemoryStream stream);
        public abstract void DeSerialize(MemoryStream stream);

        public abstract void Process();
    }
}
