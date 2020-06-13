/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/09/16 23:51:59
** desc:  场景编辑器;
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class SceneEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/SceneEditor &#s", false, 1)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(SceneEditor), new Rect(0, 0, 1280, 720), true, "场景编辑器");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.TextField("场景编辑器");

            var obj = AssetDatabase.LoadMainAssetAtPath("");
            var go = obj as GameObject;
            if (go)
            {
                var target = Instantiate(go);
                //操作;
                AssetDatabase.DeleteAsset("");
                PrefabUtility.CreatePrefab("", target);
                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndVertical();
        }
    }
}