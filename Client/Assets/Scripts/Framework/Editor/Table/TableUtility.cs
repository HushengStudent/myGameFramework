/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/22 14:26:54
** desc:  导表工具;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class TableUtility
    {
        [MenuItem("MGame/TableTools/Generate all", false, 0)]
        public static void GenerateAll()
        {

        }

        [MenuItem("MGame/TableTools/Export cs", false, 10)]
        public static void ExportCsharp()
        {

        }

        [MenuItem("MGame/TableTools/Generate byte", false, 20)]
        public static void GenerateBytes()
        {

        }

        [MenuItem("MGame/TableTools/Export Select cs", false, 11)]
        public static void ExportSelectCsharp()
        {
            string path = string.Empty;
            try
            {
                path = EditorUtility.OpenFilePanel("选择配置表", TableReader.TablePath, "csv");
            }
            catch (Exception e)
            {
                LogUtil.LogUtility.PrintWarning(e.ToString());
            }
            if (string.IsNullOrEmpty(path))
                return;
            TableExportCs.ExportCs(path);
        }

        [MenuItem("MGame/TableTools/Generate Select byte", false, 21)]
        public static void GenerateSelectBytes()
        {

        }


        private static void ExportTable()
        {

        }
    }
}