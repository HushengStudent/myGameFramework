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
        private string luaName;

        protected void Awake()
        {
            luaName = name + "Ctrl";
            LuaUtility.CallMethod(luaName, "Awake", gameObject);
        }

        protected void Start()
        {
            LuaUtility.CallMethod(luaName, "Start", gameObject);
        }

        protected void OnEnable()
        {
            LuaUtility.CallMethod(luaName, "OnEnable", gameObject);
        }

        protected void OnDisable()
        {
            LuaUtility.CallMethod(luaName, "OnDisable", gameObject);
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
