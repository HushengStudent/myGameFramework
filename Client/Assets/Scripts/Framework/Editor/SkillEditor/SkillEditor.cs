/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/09/17 00:39:06
** desc:  ¼¼ÄÜ±à¼­Æ÷;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class SkillEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/SkillEditor #k", false, 2)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(SkillEditor), new Rect(0, 0, 600, 630), true, "SkillEditor");
            window.Show();
        }
    }
}