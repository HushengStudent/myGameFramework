/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/04/05 22:42:02
** desc:  ���༭��;
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
    public class TableEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/TableEditor &#t", false, 7)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(TableEditor), new Rect(0, 0, 600, 630), true, "���༭��");
            window.Show();
        }

        void OnGUI()
        {

        }
    }
}