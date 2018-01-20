/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 23:20:54
** desc:  UI×é¼þ
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class LuaBehaviour : MonoBehaviour
    {
        private string luaName ="";

        protected void Awake()
        {
            luaName = name + "Ctrl";
            LuaUtility.CallLuaTableMethod(luaName, "Awake", gameObject);
        }

        protected void Start()
        {
            LuaUtility.CallLuaTableMethod(luaName, "Start");
        }

        protected void OnEnable()
        {
            LuaUtility.CallLuaTableMethod(luaName, "OnEnable");
        }

        protected void OnDisable()
        {
            LuaUtility.CallLuaTableMethod(luaName, "OnDisable");
        }

        protected void OnDestroy()
        {
            LuaUtility.ClearMemory();
            /*
                        Debug.Log("~" + name + " was destroy!");

                        ClearClick();
            #if ASYNC_MODE
                        string abName = name.ToLower().Replace("panel", "");
                        ResManager.UnloadAssetBundle(abName + AppConst.ExtName);
            #endif

            */
        }
    }
}
