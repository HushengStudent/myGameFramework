/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/09 01:52:28
** desc:  热更包导出编辑器;
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class UpdateEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/UpdateEditor &#u", false, 3)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(UpdateEditor), new Rect(0, 0, 1280, 720), true, "热更包导出编辑器");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();


            EditorGUILayout.EndVertical();
        }
    }
}