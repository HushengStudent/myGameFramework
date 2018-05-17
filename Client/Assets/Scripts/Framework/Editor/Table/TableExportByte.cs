/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 16:05:31
** desc:  导表生成byte;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class TableExportByte
    {
        public static Dictionary<int, TableFiledType> _tableTypeDict = new Dictionary<int, TableFiledType>();

        private static Dictionary<int, List<string>> _infoDict = new Dictionary<int, List<string>>();

        private static string _targetPath = Application.dataPath.ToLower() + "/Bundles/Single/Table/";

        private static string _fileName = string.Empty;

        public static void ExportByte(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            _infoDict.Clear();
            _tableTypeDict.Clear();
            _infoDict = TableReader.ReadCsvFile(path);
            if (_infoDict.ContainsKey(2))
            {
                _fileName = Path.GetFileNameWithoutExtension(path);
                _targetPath = _targetPath + _fileName + ".byte";
                List<string> line = _infoDict[2];
                for (int i = 0; i < line.Count; i++)
                {
                    string target = line[i];
                    string[] temp = target.Split(":".ToArray());
                    string type = TableFiledType.STRING.ToString();
                    if (temp.Length < 2)
                    {
                        LogUtil.LogUtility.PrintWarning(string.Format("#配表未指定类型{0}行,{1}列#path:" + path, 2.ToString(), i.ToString()));
                        return;
                    }
                    else
                    {
                        type = temp[1];
                    }
                    _tableTypeDict[i] = TableReader.GetTableFiledType(type);
                }
                int col = line.Count;

                MemoryStream ms = new MemoryStream();
                ms.Position = 0;
                ms.SetLength(0);
                byte[] dataCount = ConverterUtility.GetBytes(_infoDict.Count - 2);
                ms.Write(dataCount, 0, dataCount.Length);
                int index = 3;
                while (_infoDict.ContainsKey(index))
                {
                    List<string> info = _infoDict[index];
                    if (info.Count != col)
                    {
                        LogUtil.LogUtility.PrintWarning(string.Format("#配表未指定类型{0}行错误#path:" + path, index));
                        return;
                    }
                    for (int i = 0; i < info.Count; i++)
                    {
                        byte[] tempByte = Export2Bytes(info[i], _tableTypeDict[i]);
                        ms.Write(tempByte, 0, tempByte.Length);
                    }
                    index++;
                }
                FileUtility.Write2Bytes(_targetPath, ms.GetBuffer());

                EditorUtility.DisplayDialog("提示", "byte 导出成功！", "确认");
            }
        }

        public static byte[] Export2Bytes(string value, TableFiledType type)
        {
            MemoryStream ms = new MemoryStream();
            byte[] targetBytes;
            switch (type)
            {
                case TableFiledType.FLOAT:
                    targetBytes = ConverterUtility.GetBytes(float.Parse(value));
                    break;
                case TableFiledType.INT:
                    targetBytes = ConverterUtility.GetBytes(int.Parse(value));
                    break;
                case TableFiledType.BOOL:
                    targetBytes = ConverterUtility.GetBytes((int.Parse(value)) == 1);
                    break;
                case TableFiledType.STRING:
                default:
                    targetBytes = ConverterUtility.GetBytes(value);
                    byte[] countBytes = ConverterUtility.GetBytes((short)targetBytes.Length);
                    ms.Write(countBytes, 0, countBytes.Length);
                    break;
            }
            ms.Write(targetBytes, 0, targetBytes.Length);
            return ms.GetBuffer();
        }
    }
}