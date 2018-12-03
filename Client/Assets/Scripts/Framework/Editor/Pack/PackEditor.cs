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
        [MenuItem("myGameFramework/Window/PackEditor #p", false, 4)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(PackEditor), new Rect(0, 0, 600, 630), true, "打包编辑器");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
          
            EditorGUILayout.EndVertical();
        }
    }
}