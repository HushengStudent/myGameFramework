/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/05 23:04:45
** desc:  luaЭ��ע��;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework
{
    public static class LuaProtoRegister
    {
        public static Dictionary<int, bool> LuaProtoDict { get; } = new Dictionary<int, bool>();

        public static void Init()
        {
            LuaProtoDict.Clear();
        }
    }
}
