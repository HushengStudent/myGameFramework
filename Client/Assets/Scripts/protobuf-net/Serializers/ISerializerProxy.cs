/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 12:43:06
** desc:  #####
*********************************************************************************/

#if !NO_RUNTIME

namespace ProtoBuf.Serializers
{
    interface ISerializerProxy
    {
        IProtoSerializer Serializer { get; }
    }
}
#endif