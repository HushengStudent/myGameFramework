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
        private static Dictionary<int, bool> _luaProtoDict = new Dictionary<int, bool>();

        public static Dictionary<int, bool> LuaProtoDict { get { return _luaProtoDict; } }

        public static void Init()
        {
            _luaProtoDict.Clear();
        }
    }
}
