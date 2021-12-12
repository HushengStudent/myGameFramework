/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/17 23:41:41
** desc:  Lua UI编辑器;
*********************************************************************************/

using Framework.UIModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    [CustomEditor(typeof(LuaUIPanel))]
    public class LuaUIPanelEditor : Editor
    {
        #region Code Template

        private StringBuilder _panelBuilder = new StringBuilder()
            .AppendLine("---")
            .AppendLine("---Auto generated@Time.")
            .AppendLine("---Coding to do what u want to do.")
            .AppendLine("---")
            .AppendLine("")

            .AppendLine("local super = import(\"UI.BasePanel\")")
            .AppendLine("")
            .AppendLine("local NamePanel = class(\"NamePanel\", super)")
            .AppendLine("")
            .AppendLine("function NamePanel:ctor()")
            .AppendLine("    super.ctor(self)")
            .AppendLine("    self:ctorEx()")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onInit(...)")
            .AppendLine("    super.onInit(self, ...)")
            .AppendLine("    local layout = import(\".NameLayout\").new()")
            .AppendLine("    self.layout = layout:BindLuaCom(self.go)")
            .AppendLine("    self:onInitEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onRefresh(...)")
            .AppendLine("    super.onRefresh(self, ...)")
            .AppendLine("    self:onRefreshEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onUpdate(interval)")
            .AppendLine("    super.onUpdate(self, interval)")
            .AppendLine("    self:onUpdateEx(interval)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onHide(...)")
            .AppendLine("    super.onHide(self, ...)")
            .AppendLine("    self:onHideEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onResume(...)")
            .AppendLine("    super.onResume(self, ...)")
            .AppendLine("    self:onResumeEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onUnInit(...)")
            .AppendLine("    super.onUnInit(self, ...)")
            .AppendLine("    self:onUnInitEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("-----------------------------///beautiful line///-----------------------------")

            .AppendLine("")
            .AppendLine("")
            .AppendLine("")
            .AppendLine("function NamePanel:ctorEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onInitEx(...)")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onRefreshEx(...)")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onUpdateEx(interval)")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onHideEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onResumeEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NamePanel:onUnInitEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")
            .AppendLine("return NamePanel");

        private StringBuilder _layoutBuilder = new StringBuilder()
            .AppendLine("---")
            .AppendLine("---Auto generated from prefab:Prefab.prefab@Time.")
            .AppendLine("---Don't coding.")
            .AppendLine("---")
            .AppendLine("")
            .AppendLine("local NamePanelLayout = {}")
            .AppendLine("")
            .AppendLine("function NamePanelLayout:BindLuaCom(go)")
            .AppendLine("")
            .AppendLine("    local component = go:GetComponent(\"LuaUIPanel\")")
            .AppendLine("    self.layout = {}")
            .AppendLine("")
            .AppendLine("#List#")
            .AppendLine("    return self.layout")
            .AppendLine("end")
            .AppendLine("")
            .AppendLine("return NamePanelLayout")
            .AppendLine("");

        #endregion

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var originalColor = GUI.backgroundColor;
            GUILayout.Space(5);
            GUI.backgroundColor = Color.yellow;

            var luaUIPanel = target as LuaUIPanel;
            if (null == luaUIPanel)
            {
                LogHelper.PrintError("[LuaUIPanelEditor]LuaUIPanel is null.");
                return;
            }
            var panelName = luaUIPanel.gameObject.name;
            var panelPath = $"{LuaUIEditorHelper.FilePath}{panelName}/";

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Space(5);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Create Panel", GUILayout.Height(26)))
            {
                var fileName = $"{panelPath}{panelName}Panel.lua";
                var codeText = _panelBuilder.ToString().Replace("Name", panelName)
                    .Replace("Time", DateTime.Now.ToString());

                if (File.Exists(fileName))
                {
                    GUIUtility.systemCopyBuffer = codeText;
                    EditorUtility.DisplayDialog("Dialog", $"file:{panelName}Panel.lua save to clipboard!", "Ok");
                    return;
                }
                if (!Directory.Exists(panelPath))
                {
                    Directory.CreateDirectory(panelPath);
                }
                var tw = new StreamWriter(fileName);
                tw.Close();
                File.WriteAllText(fileName, codeText);

                EditorUtility.DisplayDialog("Dialog", $"file:{panelName}Panel.lua create success!", "Ok");

                AssetDatabase.Refresh();
            }

            GUILayout.Space(5);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Update Layout", GUILayout.Height(26)))
            {
                var allNode = RecursiveGetPanelComNode(luaUIPanel.gameObject.transform);
                for (var i = 0; i < allNode.Length; i++)
                {
                    var com = allNode[i].GetComponent<LuaUICom>();
                    if (com)
                    {
                        DestroyImmediate(com);
                    }
                }

                //LuaUICom
                var comDict = new Dictionary<LuaUICom, int>();
                var conList = new List<LuaUICom>();
                var nameList = new List<string>();
                for (var i = 0; i < allNode.Length; i++)
                {
                    var go = allNode[i].gameObject;
                    if (go.name.StartsWith(LuaUIEditorHelper.Header))
                    {
                        var com = go.AddComponent<LuaUICom>();
                        com.LuaUIPanel = luaUIPanel;
                        com.LuaUIComName = $"_errorNameCom{i}";
                        var tempName = LuaUIEditorHelper.GetLuaUIComName(go);
                        if (!string.IsNullOrWhiteSpace(tempName))
                        {
                            com.LuaUIComName = tempName;
                        }
                        conList.Add(com);
                        comDict[com] = conList.Count - 1;
                    }
                }
                var logBuilder = new StringBuilder();
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
                        logBuilder.AppendLine(log);
                    }
                }
                luaUIPanel.LuaUIComArray = conList.ToArray();

                //LuaUITemplate
                var allTemplate = luaUIPanel.gameObject.GetComponentsInChildren<LuaUITemplate>(true);
                var templateDict = new Dictionary<LuaUITemplate, int>();
                var templateList = new List<LuaUITemplate>();
                var templateNameList = new List<string>();
                for (var i = 0; i < allTemplate.Length; i++)
                {
                    var template = allTemplate[i];
                    var templateName = template.gameObject.name;
                    template.LuaUIPanel = luaUIPanel;
                    var tempName = $"{luaUIPanel.gameObject.name}{templateName}Template";
                    template.LuaUITemplateName = $"_{LuaUIEditorHelper.CheckName(tempName)}";
                    templateList.Add(template);
                    templateDict[template] = templateList.Count - 1;
                }
                foreach (var parent in templateDict)
                {
                    var indexBuilder = new StringBuilder();
                    foreach (var son in templateDict)
                    {
                        if (templateNameList.Contains(parent.Key.LuaUITemplateName))
                        {
                            continue;
                        }
                        if (parent.Key == son.Key)
                        {
                            continue;
                        }
                        if (parent.Key.LuaUITemplateName == son.Key.LuaUITemplateName)
                        {
                            indexBuilder.Append($"{son.Value} ");
                        }
                    }
                    var indexInfo = indexBuilder.ToString();
                    if (!string.IsNullOrWhiteSpace(indexInfo))
                    {
                        templateNameList.Add(parent.Key.LuaUITemplateName);

                        var log = $"repeat name:{parent.Key.LuaUITemplateName}:{parent.Value} {indexInfo}";
                        logBuilder.AppendLine(log);
                    }
                }
                luaUIPanel.LuaUITemplateArray = templateList.ToArray();

                //
                var fileName = $"{panelPath}{panelName}PanelLayout.lua";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                if (!Directory.Exists(panelPath))
                {
                    Directory.CreateDirectory(panelPath);
                }
                var tw = new StreamWriter(fileName);
                tw.Close();

                var codeText = _layoutBuilder.ToString().Replace("Name", panelName)
                    .Replace("Prefab", panelName).Replace("Time", DateTime.Now.ToString());

                var codeBuilder = new StringBuilder();
                for (var i = 0; i < luaUIPanel.LuaUIComArray.Length; i++)
                {
                    var name = luaUIPanel.LuaUIComArray[i].LuaUIComName;
                    var index = i.ToString();
                    codeBuilder.AppendLine(LuaUIEditorHelper.LuaCom.Replace("Name", name)
                        .Replace("index", index));
                }
                codeBuilder.AppendLine("");
                for (var i = 0; i < luaUIPanel.LuaUITemplateArray.Length; i++)
                {
                    var name = luaUIPanel.LuaUITemplateArray[i].LuaUITemplateName;
                    var index = i.ToString();
                    codeBuilder.AppendLine(LuaUIEditorHelper.LuaTemplate.Replace("Name", name)
                        .Replace("index", index));
                }

                codeText = codeText.Replace("#List#", codeBuilder.ToString());
                File.WriteAllText(fileName, codeText);

                var logInfo = logBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(logInfo))
                {
                    var title = "!!!Error>>>>>>>>>>>>>>>\r\n\r\n";
                    EditorUtility.DisplayDialog("Dialog", $"{title}{logInfo}", "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("Dialog", $"file:{panelName}PanelLayout.lua create success!", "Ok");
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = originalColor;
        }

        public Transform[] RecursiveGetPanelComNode(Transform root)
        {
            var transList = new List<Transform>();
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (!child.GetComponent<LuaUITemplate>())
                {
                    transList.Add(child);
                    transList.AddRange(RecursiveGetPanelComNode(child));
                }
            }
            return transList.ToArray();
        }
    }
}
