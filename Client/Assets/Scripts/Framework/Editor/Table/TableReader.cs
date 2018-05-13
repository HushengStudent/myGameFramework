/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 16:12:55
** desc:  读表工具;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Framework
{
    public static class TableReader
    {
        public static Dictionary<TableFiledType, string> _tableType = new Dictionary<TableFiledType, string>()
        {
            {TableFiledType.INT,"int"},
            {TableFiledType.FLOAT,"float"},
            {TableFiledType.STRING,"string"}
        };

        public static string[] _tableTypeOptions = new string[] { "    string [字符串]", "    float [浮点数]", "    int [整数]" };

        private static string _tablePath = Application.dataPath.ToLower() + "/../../Table/";

        public static string TablePath { get { return _tablePath; } }

        public static Dictionary<int, List<string>> ReadCsvFile(string path)
        {
            Dictionary<int, List<string>> infoDict = new Dictionary<int, List<string>>();
            string[] info = File.ReadAllLines(path);
            if (info.Length < 3)
            {
                LogUtil.LogUtility.PrintError("#配置表错误#path:" + path);
                return null;
            }
            for (int i = 0; i < info.Length; i++)
            {
                List<string> line = StringSplit(info[i], ",");
                infoDict[i] = line;
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
            string[] paths = str.Split(splitChar.ToArray());
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

    public enum TableFiledType : int
    {
        STRING = 0, //字符串;
        FLOAT,      //浮点数;
        INT         //整数;
    }
}