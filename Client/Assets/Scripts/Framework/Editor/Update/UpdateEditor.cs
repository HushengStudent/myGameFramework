/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/09 01:52:28
** desc:  热更包导出编辑器;
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
    public class UpdateEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/UpdateEditor &#u", false, 3)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(UpdateEditor), new Rect(0, 0, 600, 630), true, "热更包导出编辑器");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();


            EditorGUILayout.EndVertical();
        }
    }
}