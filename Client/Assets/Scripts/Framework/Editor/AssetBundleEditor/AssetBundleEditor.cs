/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/28 23:02:47
** desc:  AssetBundle编辑器;
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class AssetBundleEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/AssetBundleEditor/AssetBundleViewer &#a", false, 9)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(AssetBundleEditor), new Rect(0, 0, 1280, 720), true, "AssetBundle Viewer");
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
