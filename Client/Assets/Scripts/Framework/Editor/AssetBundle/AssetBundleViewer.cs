/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/28 23:02:47
** desc:  AssetBundle查看工具
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class AssetBundleViewer : EditorWindow
    {
        [MenuItem("MGame/AssetBundleTools/AssetBundle Viewer", false, 20)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(AssetBundleViewer), new Rect(0, 0, 600, 630), true, "AssetBundle Viewer");
            window.Show();
        }

        private void OnEnable()
        {

        }

        private void Update()
        {

        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.EndVertical();
        }
    }
}
