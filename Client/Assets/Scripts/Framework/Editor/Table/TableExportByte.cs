/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 16:05:31
** desc:  导表生成byte;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class TableExportByte
    {
        public static void ExportByte(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            EditorUtility.DisplayDialog("提示", "byte 导出成功！", "确认");
        }
    }
}