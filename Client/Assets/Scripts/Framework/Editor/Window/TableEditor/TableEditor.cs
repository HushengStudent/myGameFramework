/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/04/05 22:42:02
** desc:  ���༭��;
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class TableEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/TableEditor &#t", false, 7)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(TableEditor), new Rect(0, 0, 1280, 720), true, "���༭��");
            window.Show();
        }

        void OnGUI()
        {

        }
    }
}