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
        private readonly static string _filePath =
            "Assets/LuaFramework/Lua/UI/Panel/";
        private readonly static string _luaCom =
            "    self.layout.Name = luaUIPanel.luaUIComArray[index]";

        #region Code Template

        private StringBuilder _ctrlBuilder = new StringBuilder()
            .AppendLine("---")
            .AppendLine("---Auto generated@Time.")
            .AppendLine("---Coding to do what u want to do.")
            .AppendLine("---")
            .AppendLine("")

            .AppendLine("local super = import(\"UI.BaseCtrl\")")
            .AppendLine("")
            .AppendLine("local NameCtrl = class(\"NameCtrl\", super)")
            .AppendLine("")
            .AppendLine("function NameCtrl:ctor()")
            .AppendLine("    super.ctor(self)")
            .AppendLine("    self:ctorEx()")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onInit(...)")
            .AppendLine("    super.onInit(self, ...)")
            .AppendLine("    local layout = import(\".NameLayout\").new()")
            .AppendLine("    self.layout = layout:BindLuaCom(self.go)")
            .AppendLine("    self:onInitEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onRefresh(...)")
            .AppendLine("    super.onRefresh(self, ...)")
            .AppendLine("    self:onRefreshEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onUpdate(interval)")
            .AppendLine("    super.onUpdate(self, interval)")
            .AppendLine("    self:onUpdateEx(interval)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onHide(...)")
            .AppendLine("    super.onHide(self, ...)")
            .AppendLine("    self:onHideEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onResume(...)")
            .AppendLine("    super.onResume(self, ...)")
            .AppendLine("    self:onResumeEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onUnInit(...)")
            .AppendLine("    super.onUnInit(self, ...)")
            .AppendLine("    self:onUnInitEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("-----------------------------///beautiful line///-----------------------------")

            .AppendLine("")
            .AppendLine("")
            .AppendLine("")
            .AppendLine("function NameCtrl:ctorEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onInitEx(...)")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onRefreshEx(...)")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onUpdateEx(interval)")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onHideEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onResumeEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameCtrl:onUnInitEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")
            .AppendLine("return NameCtrl");

        private StringBuilder _layoutBuilder = new StringBuilder()
            .AppendLine("---")
            .AppendLine("---Auto generated from prefab:Prefab.prefab@Time.")
            .AppendLine("---Don't coding.")
            .AppendLine("---")
            .AppendLine("")
            .AppendLine("local NameLayout = {}")
            .AppendLine("")
            .AppendLine("function NameLayout:BindLuaCom(go)")
            .AppendLine("")
            .AppendLine("    local luaUIPanel = go:GetComponent(\"LuaUIPanel\")")
            .AppendLine("    self.layout = {}")
            .AppendLine("#List#")
            .AppendLine("    return self.layout")
            .AppendLine("end")
            .AppendLine("")
            .AppendLine("return NameLayout")
            .AppendLine("");

        #endregion

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(5);
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Create Ctrl", GUILayout.Height(30)))
            {
                var ctrl = target as LuaUIPanel;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaUIPanelEditor]LuaUIPanel is null.");
                    return;
                }
                var name = ctrl.gameObject.name;
                var path = $"{_filePath}{name}/";
                var fileName = $"{path}{name}Ctrl.lua";
                var codeText = _ctrlBuilder.ToString().Replace("Name", name)
                    .Replace("Time", DateTime.Now.ToString());

                if (File.Exists(fileName))
                {
                    GUIUtility.systemCopyBuffer = codeText;
                    EditorUtility.DisplayDialog("Dialog", $"file:{name}Ctrl.lua save to clipboard!", "Ok");
                    return;
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                TextWriter tw = new StreamWriter(fileName);
                tw.Close();
                File.WriteAllText(fileName, codeText);

                EditorUtility.DisplayDialog("Dialog", $"file:{name}Ctrl.lua create success!", "Ok");
                AssetDatabase.Refresh();
            }

            GUILayout.Space(5);
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Update Layout", GUILayout.Height(30)))
            {
                var ctrl = target as LuaUIPanel;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaUIPanelEditor]LuaUIPanel is null.");
                    return;
                }
                var allCom = ctrl.gameObject.GetComponentsInChildren<LuaUICom>(true);
                for (int i = 0; i < allCom.Length; i++)
                {
                    DestroyImmediate(allCom[i]);
                }
                var allNode = ctrl.gameObject.GetComponentsInChildren<Transform>(true);
                var comDict = new Dictionary<LuaUICom, int>();
                var conList = new List<LuaUICom>();
                var nameList = new List<string>();
                for (int i = 0; i < allNode.Length; i++)
                {
                    var go = allNode[i].gameObject;
                    if (go.name.StartsWith("Com_"))
                    {
                        var com = go.AddComponent<LuaUICom>();
                        com.LuaUIPanel = ctrl;
                        com.LuaUIComName = $"_errorNameCom{i}";
                        var tempName = LuaUIEditorHelper.GetPrefabNodeName(go);
                        if (!string.IsNullOrWhiteSpace(tempName))
                        {
                            com.LuaUIComName = tempName;
                        }
                        conList.Add(com);
                        comDict[com] = conList.Count - 1;
                    }
                }
                var infoBuilder = new StringBuilder();
                foreach (var parent in comDict)
                {
                    var indexBuilder = new StringBuilder();
                    foreach (var son in comDict)
                    {
                        if (nameList.Contains(parent.Key.LuaUIComName))
                        {
                            continue;
                        }
                        if (parent.Key == son.Key)
                        {
                            continue;
                        }
                        if (parent.Key.LuaUIComName == son.Key.LuaUIComName)
                        {
                            indexBuilder.Append($"{son.Value} ");
                        }
                    }
                    var indexInfo = indexBuilder.ToString();
                    if (!string.IsNullOrWhiteSpace(indexInfo))
                    {
                        nameList.Add(parent.Key.LuaUIComName);

                        var log = $"repeat name:{parent.Key.LuaUIComName}:{parent.Value} {indexInfo}";
                        infoBuilder.AppendLine(log);
                    }
                }
                ctrl.luaUIComArray = conList.ToArray();

                var name = ctrl.gameObject.name;
                var path = $"{_filePath}{name}/";
                var fileName = $"{path}{name}Layout.lua";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                TextWriter tw = new StreamWriter(fileName);
                tw.Close();

                var codeText = _layoutBuilder.ToString().Replace("Name", name).Replace("Prefab", name)
                    .Replace("Time", DateTime.Now.ToString());

                var comBuilder = new StringBuilder();
                for (int i = 0; i < ctrl.luaUIComArray.Length; i++)
                {
                    var comName = ctrl.luaUIComArray[i].LuaUIComName;
                    var comIndex = i.ToString();
                    var str = _luaCom.Replace("Name", comName).Replace("index", comIndex);
                    comBuilder.AppendLine(str);
                }

                codeText = codeText.Replace("#List#", comBuilder.ToString());
                File.WriteAllText(fileName, codeText);

                var info = infoBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(info))
                {
                    EditorUtility.DisplayDialog("Dialog", info, "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("Dialog", $"file:{name}Layout.lua create success!", "Ok");
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUI.backgroundColor = Color.blue;
            GUI.backgroundColor = originalColor;
        }
    }
}
