/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/07/07 11:48:33
** desc:   Lua UI模板;
*********************************************************************************/

using UnityEngine;

namespace Framework.UIModule
{
    public class LuaUITemplate : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public LuaUIPanel LuaUIPanel;

        [SerializeField]
        public string LuaUITemplateName;

        [SerializeField]
        public LuaUICom[] LuaUIComArray;
    }
}