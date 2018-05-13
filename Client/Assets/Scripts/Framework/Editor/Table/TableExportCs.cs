/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 16:04:39
** desc:  导表生成c#;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public static class TableExportCs
    {
        private static Dictionary<int, List<string>> infoDict = new Dictionary<int, List<string>>();

        private static string targetPath = Application.dataPath.ToLower() + "/Scripts/Common/Table";

        public static void ExportCs(string path)
        {
            infoDict.Clear();
            infoDict = TableReader.ReadCsvFile(path);
            if (infoDict.ContainsKey(1))
            {
                targetPath = targetPath + Path.GetFileNameWithoutExtension(path) + ".cs";
                List<string> line = infoDict[1];
                for (int i = 0; i < line.Count; i++)
                {
                    string target = line[i];
                    string[] temp = target.Split(":".ToArray());
                    string type = TableFiledType.STRING.ToString();
                    if (temp.Length < 2)
                    {
                        LogUtil.LogUtility.PrintWarning(string.Format("#配表未指定类型{0}行,{1}列#path:" + path, 2.ToString(), i.ToString()));
                    }
                    else
                    {
                        type = temp[1];
                    }
                    //colDict[i] = new KeyValuePair<string, TableFiledType>(temp[0], TableReader.GetTableFiledType(type));
                }
            }
        }
    }
}