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
            if (EditorUtility.DisplayDialog("提示", "导出全部 cs 和 byte！", "确认", "取消"))
            {

            }
        }

        [MenuItem("MGame/TableTools/Export cs", false, 101)]
        public static void ExportCsharp()
        {
            if (EditorUtility.DisplayDialog("提示", "导出全部 cs！", "确认", "取消"))
            {

            }
        }

        [MenuItem("MGame/TableTools/Generate byte", false, 102)]
        public static void GenerateBytes()
        {
            if (EditorUtility.DisplayDialog("提示", "导出全部 byte！", "确认", "取消"))
            {

            }
        }

        [MenuItem("MGame/TableTools/Export Select cs", false, 201)]
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

        [MenuItem("MGame/TableTools/Generate Select byte", false, 202)]
        public static void GenerateSelectBytes()
        {

        }


        private static void ExportTable()
        {

        }
    }
}