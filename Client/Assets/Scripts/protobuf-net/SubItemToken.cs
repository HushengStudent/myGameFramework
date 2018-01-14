/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 12:43:06
** desc:  #####
*********************************************************************************/


namespace ProtoBuf
{
    /// <summary>
    /// Used to hold particulars relating to nested objects. This is opaque to the caller - simply
    /// give back the token you are given at the end of an object.
    /// </summary>
    public struct SubItemToken
    {
        internal readonly int value;
        internal SubItemToken(int value) {
            this.value = value;
        }
    }
}
