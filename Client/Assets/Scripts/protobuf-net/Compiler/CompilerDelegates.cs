/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 12:43:07
** desc:  #####
*********************************************************************************/

#if FEAT_COMPILER
namespace ProtoBuf.Compiler
{
    internal delegate void ProtoSerializer(object value, ProtoWriter dest);
    internal delegate object ProtoDeserializer(object value, ProtoReader source);
}
#endif