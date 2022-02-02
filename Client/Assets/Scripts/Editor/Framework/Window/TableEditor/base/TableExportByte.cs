/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 16:05:31
** desc:  导表生成byte;
*********************************************************************************/

using Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.EditorModule.Window
{
    public class TableExportByte
    {
        public static Dictionary<int, TableFiledType> _tableTypeDict = new Dictionary<int, TableFiledType>();
        private static Dictionary<int, List<string>> _infoDict = new Dictionary<int, List<string>>();
        private static string _targetPath = $"{Application.dataPath.ToLower()}/Resources/Bin/Table/";
        private static string _fileName = string.Empty;

        public static void ExportByte(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            _infoDict.Clear();
            _tableTypeDict.Clear();
            _infoDict = TableReader.ReadCsvFile(path);

            var allBytes = new List<byte>();
            if (_infoDict.ContainsKey(2))
            {
                _fileName = Path.GetFileNameWithoutExtension(path);
                var filePath = _targetPath + _fileName + ".byte";
                var line = _infoDict[2];
                for (var i = 0; i < line.Count; i++)
                {
                    var target = line[i];
                    var temp = target.Split(":".ToArray());
                    var type = TableFiledType.STRING.ToString();
                    if (temp.Length < 2)
                    {
                        LogHelper.PrintWarning(string.Format("#配表未指定类型{0}行,{1}列#path:" + path, 2.ToString(), i.ToString()));
                        return;
                    }
                    else
                    {
                        type = temp[1];
                    }
                    _tableTypeDict[i] = TableReader.GetTableFiledType(type);
                }
                var col = line.Count;
                var dataCount = ConvertHelper.GetBytes(_infoDict.Count - 2);
                allBytes.AddRange(dataCount);
                var index = 3;
                while (_infoDict.ContainsKey(index))
                {
                    var info = _infoDict[index];
                    if (info.Count != col)
                    {
                        LogHelper.PrintWarning(string.Format("#配表未指定类型{0}行错误#path:" + path, index));
                        return;
                    }
                    for (var i = 0; i < info.Count; i++)
                    {
                        var tempByte = Export2Bytes(info[i], _tableTypeDict[i]);
                        allBytes.AddRange(tempByte);
                    }
                    index++;
                }
                FileHelper.Write2Bytes(filePath, allBytes.ToArray());
                EditorUtility.DisplayDialog("提示", "byte 导出成功！", "确认");
            }
        }

        public static byte[] Export2Bytes(string value, TableFiledType type)
        {
            var bytesList = new List<byte>();
            byte[] targetBytes;
            switch (type)
            {
                case TableFiledType.FLOAT:
                    targetBytes = ConvertHelper.GetBytes(float.Parse(value));
                    break;
                case TableFiledType.INT:
                    targetBytes = ConvertHelper.GetBytes(int.Parse(value));
                    break;
                case TableFiledType.BOOL:
                    targetBytes = ConvertHelper.GetBytes((int.Parse(value)) == 1);
                    break;
                case TableFiledType.STRING:
                default:
                    targetBytes = ConvertHelper.GetBytes(value);
                    var countBytes = ConvertHelper.GetBytes(targetBytes.Length);
                    bytesList.AddRange(countBytes);
                    break;
            }
            bytesList.AddRange(targetBytes);
            return bytesList.ToArray();
        }
    }
}