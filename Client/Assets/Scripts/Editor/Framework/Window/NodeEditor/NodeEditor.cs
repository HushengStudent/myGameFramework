/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/04/05 22:37:11
** desc:  节点编辑器;
*********************************************************************************/

using UnityEditor;
using UnityEngine;

namespace Framework.EditorModule.Window
{
    public class NodeEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/NodeEditor &#n", false, 6)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(NodeEditor), new Rect(0, 0, 1280, 720), true, "节点编辑器");
            window.Show();
        }

        void OnGUI()
        {

        }
    }
}