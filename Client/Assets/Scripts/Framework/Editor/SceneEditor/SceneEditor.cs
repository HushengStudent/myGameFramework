/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/09/16 23:51:59
** desc:  场景编辑器;
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
    public class SceneEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/SceneEditor #s", false, 1)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(SceneEditor), new Rect(0, 0, 600, 630), true, "场景编辑器");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.TextField("场景编辑器");

            Object obj = AssetDatabase.LoadMainAssetAtPath("");
            GameObject go = obj as GameObject;
            if (go)
            {
                GameObject target = GameObject.Instantiate(go);
                //操作;
                AssetDatabase.DeleteAsset("");
                PrefabUtility.CreatePrefab("", target);
                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndVertical();
        }
    }
}