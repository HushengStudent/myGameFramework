/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/09/17 00:39:06
** desc:  技能编辑器;
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class SkillEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/SkillEditor &#k", false, 2)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(SkillEditor), new Rect(0, 0, 1280, 720), true, "技能编辑器");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();


            EditorGUILayout.EndVertical();
        }
    }
}