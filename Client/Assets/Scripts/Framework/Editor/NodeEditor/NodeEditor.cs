/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/04/05 22:37:11
** desc:  �ڵ�༭��;
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
    public class NodeEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/NodeEditor &#n", false, 6)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(NodeEditor), new Rect(0, 0, 600, 630), true, "�ڵ�༭��");
            window.Show();
        }

        void OnGUI()
        {

        }
    }
}