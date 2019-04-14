/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/21 23:40:35
** desc:  打包编辑器;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class PackEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/PackEditor &#b", false, 4)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(PackEditor), new Rect(0, 0, 1280, 720), true, "打包编辑器");
            window.Show();
        }

        private enum BuildPlatform
        {
            Android,
            IOS,
            Windows,
        }

        private BuildPlatform platform = BuildPlatform.Windows;

        void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 18;
            var color = GUI.backgroundColor;

            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                GUILayout.Space(5);
                using (var vv = new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("打包", style);
                    GUILayout.Space(5);
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("选择目标平台:", style);
                        GUILayout.Space(15);
                        this.platform = (BuildPlatform)EditorGUILayout.EnumPopup(this.platform);
                    }
                    GUILayout.Space(20);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("开始打包", GUILayout.Height(30)))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确认开始打包?", "确认"))
                        {

                        }
                    }
                    GUI.backgroundColor = color;
                }
                GUILayout.Space(10);
                using (var vv = new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("打热更包", style);
                    GUILayout.Space(5);
                }
            }
        }
    }
}