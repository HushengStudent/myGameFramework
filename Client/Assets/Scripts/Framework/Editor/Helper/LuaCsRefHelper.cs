/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 12:51:59
** desc:  Lua引用cs对象;
*********************************************************************************/

using LuaInterface;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class LuaCsRefHelper : EditorWindow
    {
        [MenuItem("myGameFramework/Helper/Lua Cs Ref Analysis", false, 0)]
        public static void GenerateAll()
        {
            var window = GetWindow(typeof(LuaCsRefHelper), false, "LuaCsRef Helper");
            window.Show();
        }

        private class ObjectRef
        {
            public Type _objectType;
            public int _objectCount;
        }

        private List<ObjectRef> GetTypeCountList()
        {
            var objectTranslator = LuaState.GetTranslator(IntPtr.Zero);
            var result = new List<ObjectRef>();
            foreach (var each in objectTranslator.objectsBackMap)
            {
                var t = each.Key.GetType();
                var find = false;
                for (var i = 0; i < result.Count; i++)
                {
                    if (result[i]._objectType == t)
                    {
                        result[i]._objectCount++;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    var objectRef = new ObjectRef
                    {
                        _objectType = t,
                        _objectCount = 1
                    };
                    result.Add(objectRef);
                }
            }
            result.Sort((x, y) => -x._objectCount.CompareTo(y._objectCount));
            return result;
        }

        void OnGUI()
        {
            if (LuaState.MainState == null)
            {
                return;
            }
            var typeCountList = GetTypeCountList();

            EditorGUILayout.BeginVertical();

            foreach (var each in typeCountList)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(each._objectType.ToString(), GUILayout.Width(250));
                GUILayout.Label(each._objectCount.ToString());

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}