/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 12:51:59
** desc:  Lua引用cs对象;
*********************************************************************************/

using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class LuaCsRefUtility : EditorWindow
    {
        [MenuItem("myGameFramework/Utility/Lua Cs Ref Analysis", false, 0)]
        public static void GenerateAll()
        {
            var window = GetWindow(typeof(LuaCsRefUtility), false, "LuaCsRef Utility");
            window.Show();
        }

        private class ObjectRef
        {
            public Type _objectType;
            public int _objectCount;
        }

        private List<ObjectRef> GetTypeCountList()
        {
            ObjectTranslator objectTranslator = LuaState.GetTranslator(IntPtr.Zero);
            List<ObjectRef> result = new List<ObjectRef>();
            foreach (var each in objectTranslator.objectsBackMap)
            {
                Type t = each.Key.GetType();
                bool find = false;
                for (int i = 0; i < result.Count; i++)
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
                    ObjectRef objectRef = new ObjectRef();
                    objectRef._objectType = t;
                    objectRef._objectCount = 1;
                    result.Add(objectRef);
                }
            }
            result.Sort((x, y) => -x._objectCount.CompareTo(y._objectCount));
            return result;
        }

        void OnGUI()
        {
            if (LuaState.MainState == null) return;
            List<ObjectRef> typeCountList = GetTypeCountList();

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