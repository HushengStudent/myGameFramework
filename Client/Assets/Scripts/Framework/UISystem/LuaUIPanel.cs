/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/11 00:53:47
** desc:  Lua UI面板;
*********************************************************************************/

using UnityEngine;

namespace Framework.UIModule
{
    public class LuaUIPanel : MonoBehaviour
    {
        [SerializeField]
        public LuaUICom[] LuaUIComArray;

        [SerializeField]
        public LuaUITemplate[] LuaUITemplateArray;
    }
}
