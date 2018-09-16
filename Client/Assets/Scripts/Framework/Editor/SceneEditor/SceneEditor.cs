/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/09/16 23:51:59
** desc:  ³¡¾°±à¼­Æ÷;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class SceneEditor : EditorWindow
    {

        [MenuItem("myGameFramework/Window/SceneEditor", false, 1)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(SceneEditor), new Rect(0, 0, 600, 630), true, "SceneEditor");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.TextField("³¡¾°±à¼­Æ÷");


            EditorGUILayout.EndVertical();
        }
    }
}