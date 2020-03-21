/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/17 23:41:41
** desc:  编辑器扩展;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomEditor(typeof(LuaUIPanel))]
    public class LuaUIPanelEditor : Editor
    {
        private readonly static string _filePath = "Assets/LuaFramework/Lua/UI/Panel/";

        private readonly static string _luaCom =
            "    self.panel.Name = luaUIPanel.luaUIComArray[index]";

        private StringBuilder _ctrlBuilder = new StringBuilder()
            .AppendLine("-- -")
            .AppendLine("---Auto generated.")
            .AppendLine("---Coding to do what u want to do.")
            .AppendLine("---")
            .AppendLine(" ")

            .AppendLine("local super = import(\"UI.BaseCtrl\")")
            .AppendLine(" ")
            .AppendLine("local NameCtrl = class(\"NameCtrl\", super)")
            .AppendLine(" ")
            .AppendLine("function NameCtrl:ctor()")
            .AppendLine("       super.ctor(self)")
            .AppendLine("       self:ctorEx()")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onInit(...)")
            .AppendLine("       super.onInit(self, ...)")
            .AppendLine("       local layout = import(\".NameLayout\").new()")
            .AppendLine("       self.layout = layout:BindLuaCom(self.go)")
            .AppendLine("       self:onInitEx(...)")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onRefresh(...)")
            .AppendLine("       super.onRefresh(self, ...)")
            .AppendLine("       self:onRefreshEx(...)")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onUpdate(interval)")
            .AppendLine("       super.onUpdate(self, interval)")
            .AppendLine("       self:onUpdateEx(interval)")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onHide(...)")
            .AppendLine("       super.onHide(self, ...)")
            .AppendLine("       self:onHideEx(...)")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onResume(...)")
            .AppendLine("       super.onResume(self, ...)")
            .AppendLine("       self:onResumeEx(...)")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onUnInit(...)")
            .AppendLine("       super.onUnInit(self, ...)")
            .AppendLine("       self:onUnInitEx(...)")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("-----------------------------///beautiful line///-----------------------------")

            .AppendLine(" ")
            .AppendLine(" ")
            .AppendLine(" ")
            .AppendLine("function NameCtrl:ctorEx()")
            .AppendLine(" ")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onInitEx(...)")
            .AppendLine(" ")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onRefreshEx(...)")
            .AppendLine(" ")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onUpdateEx(interval)")
            .AppendLine(" ")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onHideEx()")
            .AppendLine(" ")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onResumeEx()")
            .AppendLine(" ")
            .AppendLine("end")
            .AppendLine(" ")

            .AppendLine("function NameCtrl:onUnInitEx()")
            .AppendLine(" ")
            .AppendLine("end")
            .AppendLine(" ")
            .AppendLine("return NameCtrl");

        private StringBuilder _layoutBuilder = new StringBuilder()
            .AppendLine("---")
            .AppendLine("---Auto generated@Time.")
            .AppendLine("---Don't coding.")
            .AppendLine("---")
            .AppendLine(" ")
            .AppendLine("local NamePanel = {}")
            .AppendLine(" ")
            .AppendLine("function NamePanel:BindLuaCom(go)")
            .AppendLine(" ")
            .AppendLine("    local luaUIPanel = go:GetComponent(\"LuaUIPanel\")")
            .AppendLine("    self.layout = {}")
            .AppendLine("#List#")
            .AppendLine("    return self.layout")
            .AppendLine("end")
            .AppendLine(" ")
            .AppendLine("return NamePanel")
            .AppendLine(" ");

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("\r\nSerialize Prefab\r\n"))
            {
                LuaUIPanel ctrl = target as LuaUIPanel;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaUIPanelEditor]LuaUIPanel is null.");
                    return;
                }
                LuaUICom[] comArray = ctrl.gameObject.GetComponentsInChildren<LuaUICom>(true);
                Dictionary<string, List<int>> repeatNameDict = new Dictionary<string, List<int>>();
                for (int i = 0; i < comArray.Length; i++)
                {
                    comArray[i].LuaUIPanel = ctrl;
                    if (string.IsNullOrEmpty(comArray[i].LuaUIComName))
                    {
                        comArray[i].LuaUIComName = CheckInvalidFormatName(comArray[i].gameObject.name);
                    }
                    var name = comArray[i].LuaUIComName;
                    for (int j = i + 1; j < comArray.Length; j++)
                    {
                        var str = comArray[j].LuaUIComName;
                        if (name == str)
                        {
                            List<int> list;
                            if (!repeatNameDict.TryGetValue(name, out list))
                            {
                                list = new List<int>();
                                repeatNameDict[name] = list;
                                list.Add(j);
                            }
                            list.Add(i);
                        }
                    }
                }
                if (repeatNameDict.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (var temp in repeatNameDict)
                    {
                        if (temp.Value.Count > 0)
                        {
                            temp.Value.Sort();
                            for (int i = 0; i < temp.Value.Count; i++)
                            {
                                builder.Append(temp.Value[i].ToString() + ",");
                            }
                            builder.Append("name repeat:" + temp.Key);
                        }
                        builder.AppendLine(" ");
                    }
                    EditorUtility.DisplayDialog("Dialog", builder.ToString(), "ok");
                    return;
                }
                ctrl.luaUIComArray = new LuaUICom[comArray.Length];
                for (int i = 0; i < ctrl.luaUIComArray.Length; i++)
                {
                    ctrl.luaUIComArray[i] = comArray[i];
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("\r\nCreate Ctrl\r\n"))
            {
                LuaUIPanel ctrl = target as LuaUIPanel;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaUIPanelEditor]LuaUIPanel is null.");
                    return;
                }
                string name = ctrl.gameObject.name;
                string fileName = $"{_filePath}{name}Ctrl.lua";
                string code = _ctrlBuilder.ToString().Replace("Name", name);
                if (File.Exists(fileName))
                {
                    GUIUtility.systemCopyBuffer = code;
                    EditorUtility.DisplayDialog("Dialog", $"file:{name}Ctrl.lua save to clipboard!", "Ok");
                    return;
                }
                TextWriter tw = new StreamWriter(fileName);
                tw.Close();
                File.WriteAllText(fileName, code);
                EditorUtility.DisplayDialog("Dialog", $"file:{name}Ctrl.lua create success!", "Ok");
                AssetDatabase.Refresh();
            }

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("\r\nUpdate Layout\r\n"))
            {
                LuaUIPanel ctrl = target as LuaUIPanel;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaUIPanelEditor]LuaUIPanel is null.");
                    return;
                }
                string name = ctrl.gameObject.name;
                string fileName = $"{_filePath}{name}Layout.lua";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                TextWriter tw = new StreamWriter(fileName);
                tw.Close();

                string codeText = _layoutBuilder.ToString();
                codeText = codeText.Replace("Name", name).Replace("Time", DateTime.Now.ToString());

                StringBuilder comBuilder = new StringBuilder();
                for (int i = 0; i < ctrl.luaUIComArray.Length; i++)
                {
                    var comName = ctrl.luaUIComArray[i].LuaUIComName;
                    var comIndex = i.ToString();
                    var str = _luaCom.Replace("Name", comName).Replace("index", comIndex);
                    comBuilder.AppendLine(str);
                }

                codeText = codeText.Replace("#List#", comBuilder.ToString());
                File.WriteAllText(fileName, codeText);
                EditorUtility.DisplayDialog("Dialog", $"file:{name}Layout.lua create success!", "Ok");
                AssetDatabase.Refresh();
            }
            GUI.backgroundColor = Color.blue;
            GUI.backgroundColor = originalColor;
        }

        private string CheckInvalidFormatName(string str)
        {
            return str.Replace(" ", "").Replace(".", "").Replace("。", "")
                .Replace("(", "").Replace("（", "").Replace(")", "").Replace("）", "")
                .Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "")
                .Replace("【", "").Replace("】", "");
        }
    }
}
