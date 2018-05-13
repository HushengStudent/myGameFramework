/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/13 17:16:09
** desc:  #####
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
        private static Dictionary<int, List<string>> infoDict = new Dictionary<int, List<string>>();

        private static Dictionary<int, KeyValuePair<string, TableFiledType>> colDict = new Dictionary<int, KeyValuePair<string, TableFiledType>>();

        private static string targetPath = string.Empty;

        [MenuItem("MGame/TableTools/初始化配置表", false, 101)]
        public static void InitTable()
        {
            colDict.Clear();
            targetPath = string.Empty;
            bool autoSave = false;

            var window = GetWindow(typeof(TableInitCsv), false, "初始化配置表");
            window.Show();
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
            infoDict = TableReader.ReadCsvFile(path);
            if (infoDict.ContainsKey(2))
            {
                targetPath = path;
                List<string> line = infoDict[1];
                for (int i = 0; i < line.Count; i++)
                {
                    string target = line[i];
                    string[] temp = target.Split(":".ToArray());
                    string type = TableFiledType.STRING.ToString();
                    if (temp.Length < 2)
                    {
                        LogUtil.LogUtility.PrintWarning(string.Format("#配表未指定类型{0}行,{1}列#path:" + path, 2.ToString(), i.ToString()));
                        autoSave = true;
                        infoDict[1][i] = temp[0] + ":" + type;
                    }
                    else
                    {
                        type = temp[1];
                    }
                    colDict[i] = new KeyValuePair<string, TableFiledType>(temp[0], TableReader.GetTableFiledType(type));
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
            if (string.IsNullOrEmpty(targetPath))
            {
                LogUtil.LogUtility.PrintWarning("#未指定配表路径#path:" + targetPath);
                return;
            }
            int count = infoDict.Count;
            string[] info = new string[count];
            for (int i = 0; i < count; i++)
            {
                string target = string.Empty;
                List<string> list = infoDict[i];
                for (int j = 0; j < list.Count; j++)
                {
                    if (j != 0)
                        target = target + ",";
                    target = target + list[j];
                }
                info[i] = target;
            }
            File.WriteAllLines(targetPath, info, Encoding.UTF8);
            EditorUtility.DisplayDialog("提醒", "配置表已刷新并保存！", "确认");
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginScrollView(new Vector2(1000, 1000));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("save table", GUILayout.Width(150), GUILayout.Height(30)))
                Save();
            GUILayout.Space(20);
            if (GUILayout.Button("open table", GUILayout.Width(150), GUILayout.Height(30)))
                InitTable();
            EditorGUILayout.EndHorizontal();
            int count = colDict.Count;
            for (int i = 0; i < count; i++)
            {
                GUILayout.Space(6);
                GUILayout.Label("===================================");
                GUILayout.Space(6);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(10));
                GUILayout.Label(colDict[i].Key, GUILayout.Width(150));
                int index = EditorGUILayout.Popup((int)colDict[i].Value, TableReader._tableTypeOptions, GUILayout.Width(150));
                if ((int)colDict[i].Value != index)
                {
                    colDict[i] = new KeyValuePair<string, TableFiledType>(colDict[i].Key, (TableFiledType)index);
                    infoDict[1][i] = colDict[i].Key + ":" + colDict[i].Value.ToString();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}