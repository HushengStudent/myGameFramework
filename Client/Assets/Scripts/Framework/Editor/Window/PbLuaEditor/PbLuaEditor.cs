/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:22:03
** desc:  协议生成编辑器;
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
    public class PbLuaEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/PbLuaEditor &#p", false, 8)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(PbLuaEditor), new Rect(0, 0, 1280, 720), true, "协议生成编辑器");
            window.Show();
        }

        void OnGUI()
        {

        }
    }
}
