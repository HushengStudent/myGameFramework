/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 16:12:55
** desc:  读表工具;
*********************************************************************************/

using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.EditorModule.Window
{
    public static class TableReader
    {
        public static Dictionary<int, List<string>> ReadCsvFile(string path)
        {
            var infoDict = new Dictionary<int, List<string>>();
            var info = FileHelper.ReadFromTxt(path);
            if (info.Length < 3)
            {
                LogHelper.PrintError("#配置表错误#path:" + path);
                return null;
            }
            var index = 1;
            for (var i = 0; i < info.Length; i++)
            {
                var line = StringSplit(info[i], ",");
                if (line == null || line.Count < 1)
                {
                    continue;
                }
                infoDict[index] = line;
                index++;
            }
            return infoDict;
        }

        /// <summary>
        /// 字符串分割;
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static List<string> StringSplit(string str, string splitChar)
        {
            var paths = str.Split(splitChar.ToArray());
            return paths.ToList();
        }

        /// <summary>
        /// 字符串转枚举;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TableFiledType GetTableFiledType(string str)
        {
            return (TableFiledType)Enum.Parse(typeof(TableFiledType), str);
        }
    }
}