/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/09 01:52:28
** desc:  #####
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class UpdateEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/UpdateEditor #u", false, 3)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(SkillEditor), new Rect(0, 0, 600, 630), true, "UpdateEditor");
            window.Show();
        }
    }
}