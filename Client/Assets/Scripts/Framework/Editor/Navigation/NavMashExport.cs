/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/11 23:17:43
** desc:  导航网格导出;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class NavMashExport
    {
        private static string _rootPath = Application.dataPath + "/ArtRes/";

        [MenuItem("MGame/Navigation/Export NavMash", false, 0)]
        public static void Export()
        {
            GameObject target = Selection.activeObject as GameObject;
            if (!target.name.Contains("NavMesh"))
            {
                return;
            }
        }
    }
}
