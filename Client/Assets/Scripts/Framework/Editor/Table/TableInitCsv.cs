/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 17:16:09
** desc:  配置表初始化;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class TableInitCsv : EditorWindow
    {
        public static string[] _tableTypeOptions = new string[] { "    string [字符串]", "    float [浮点数]", "    int [整数]", "    bool [Boolean]" };
        private static Dictionary<int, List<string>> _infoDict = new Dictionary<int, List<string>>();
        private static Dictionary<int, KeyValuePair<string, TableFiledType>> _colDict = new Dictionary<int, KeyValuePair<string, TableFiledType>>();
        private static string _targetPath = string.Empty;

        [MenuItem("myGameFramework/TableTools/初始化配置表", false, 301)]
        public static void InitTable()
        {
            _colDict.Clear();
            _infoDict.Clear();
            _targetPath = string.Empty;

            bool autoSave = false;
            var window = GetWindow(typeof(TableInitCsv), false, "初始化配置表");
            window.Show();
            string path = string.Empty;
            try
            {
                path = EditorUtility.OpenFilePanel("选择配置表", TableConfig.TablePath, "csv");
            }
            catch (Exception e)
            {
                LogUtil.LogUtility.PrintWarning(e.ToString());
            }
            if (string.IsNullOrEmpty(path))
                return;
            _infoDict = TableReader.ReadCsvFile(path);
            if (_infoDict.ContainsKey(2))
            {
                _targetPath = path;
                List<string> line = _infoDict[2];
                for (int i = 0; i < line.Count; i++)
                {
                    string target = line[i];
                    string[] temp = target.Split(":".ToArray());
                    string type = TableFiledType.STRING.ToString();
                    if (temp.Length < 2)
                    {
                        LogUtil.LogUtility.PrintWarning(string.Format("#配表未指定类型{0}行,{1}列#path:" + path, 2.ToString(), i.ToString()));
                        autoSave = true;
                        _infoDict[2][i] = temp[0] + ":" + type;
                    }
                    else
                    {
                        type = temp[1];
                    }
                    _colDict[i] = new KeyValuePair<string, TableFiledType>(temp[0], TableReader.GetTableFiledType(type));
                }
                if (autoSave)
                    Save();
            }
        }

        /// <summary>
        /// 保存csv;
        /// </summary>
        private static void Save()
        {
            if (string.IsNullOrEmpty(_targetPath))
            {
                LogUtil.LogUtility.PrintWarning("#未指定配表路径#path:" + _targetPath);
                return;
            }
            int count = _infoDict.Count;
            string[] info = new string[count];
            for (int i = 0; i < count; i++)
            {
                string target = string.Empty;
                List<string> list = _infoDict[i + 1];
                for (int j = 0; j < list.Count; j++)
                {
                    if (j != 0)
                        target = target + ",";
                    target = target + list[j];
                }
                info[i] = target;
            }
            File.WriteAllLines(_targetPath, info, Encoding.UTF8);
            EditorUtility.DisplayDialog("提示", "配置表已刷新并保存！", "确认");
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginScrollView(new Vector2(1000, 1000));
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("open table", GUILayout.Width(120), GUILayout.Height(30)))
                InitTable();
            GUILayout.Space(10);
            if (GUILayout.Button("save table", GUILayout.Width(120), GUILayout.Height(30)))
                Save();
            GUILayout.Space(10);
            if (GUILayout.Button("export cs", GUILayout.Width(120), GUILayout.Height(30)))
                TableExportCs.ExportCs(_targetPath);
            GUILayout.Space(10);
            if (GUILayout.Button("generate byte", GUILayout.Width(120), GUILayout.Height(30)))
                TableExportByte.ExportByte(_targetPath);

            EditorGUILayout.EndHorizontal();
            int count = _colDict.Count;
            for (int i = 0; i < count; i++)
            {
                GUILayout.Space(6);
                GUILayout.Label("===================================");
                GUILayout.Space(6);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(10));
                GUILayout.Label(_colDict[i].Key, GUILayout.Width(150));
                int index = EditorGUILayout.Popup((int)_colDict[i].Value, _tableTypeOptions, GUILayout.Width(150));
                if ((int)_colDict[i].Value != index)
                {
                    _colDict[i] = new KeyValuePair<string, TableFiledType>(_colDict[i].Key, (TableFiledType)index);
                    _infoDict[2][i] = _colDict[i].Key + ":" + _colDict[i].Value.ToString();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}