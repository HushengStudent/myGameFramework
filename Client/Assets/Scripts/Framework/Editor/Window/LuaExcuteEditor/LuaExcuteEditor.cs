/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 22:01:07
** desc:  Lua���Թ���;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class LuaExcuteEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/LuaExcuteEditor &#e", false, 5)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(LuaExcuteEditor), new Rect(0, 0, 640, 720), true, "Lua Excute");
            window.Show();
        }

        private string _luaText = string.Empty;

        void OnGUI()
        {
            var color = GUI.backgroundColor;
            EditorGUILayout.BeginVertical(GUI.skin.box);
            _luaText = EditorGUILayout.TextField(_luaText, GUILayout.Height(300));
            GUILayout.Space(20);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Excute", GUILayout.Height(30)))
            {
                if (Application.isPlaying && LuaMgr.Instance)
                {
                    LuaMgr.Instance.Dostring(_luaText);
                }
            }
            GUI.backgroundColor = color;
            EditorGUILayout.EndVertical();
        }
    }
}