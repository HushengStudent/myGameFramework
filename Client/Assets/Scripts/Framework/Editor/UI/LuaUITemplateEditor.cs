/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/07/07 11:52:12
** desc:  Lua UI±à¼­Æ÷;
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
    [CustomEditor(typeof(LuaUITemplate))]
    public class LuaUITemplateEditor : Editor
    {
        #region Code Template

        private StringBuilder _templateBuilder = new StringBuilder()
            .AppendLine("---")
            .AppendLine("---Auto generated@Time.")
            .AppendLine("---Coding to do what u want to do.")
            .AppendLine("---")
            .AppendLine("")

            .AppendLine("local super = import(\"UI.BaseTemplate\")")
            .AppendLine("")
            .AppendLine("local NameTemplate = class(\"NameTemplate\", super)")
            .AppendLine("")
            .AppendLine("function NameTemplate:ctor()")
            .AppendLine("    super.ctor(self)")
            .AppendLine("    self:ctorEx()")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameTemplate:onInit(...)")
            .AppendLine("    super.onInit(self, ...)")
            .AppendLine("    local layout = import(\".NameLayout\").new()")
            .AppendLine("    self.layout = layout:BindLuaCom(self.go)")
            .AppendLine("    self:onInitEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameTemplate:onUnInit(...)")
            .AppendLine("    super.onUnInit(self, ...)")
            .AppendLine("    self:onUnInitEx(...)")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("-----------------------------///beautiful line///-----------------------------")

            .AppendLine("")
            .AppendLine("")
            .AppendLine("")
            .AppendLine("function NameTemplate:ctorEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameTemplate:onInitEx(...)")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")

            .AppendLine("function NameTemplate:onUnInitEx()")
            .AppendLine("")
            .AppendLine("end")
            .AppendLine("")
            .AppendLine("return NameTemplate");

        private StringBuilder _layoutBuilder = new StringBuilder()
            .AppendLine("---")
            .AppendLine("---Auto generated from prefab@Time.")
            .AppendLine("---Don't coding.")
            .AppendLine("---")
            .AppendLine("")
            .AppendLine("local NameTemplateLayout = {}")
            .AppendLine("")
            .AppendLine("function NameTemplateLayout:BindLuaCom(go)")
            .AppendLine("")
            .AppendLine("    local component = go:GetComponent(\"LuaUITemplate\")")
            .AppendLine("    self.layout = {}")
            .AppendLine("")
            .AppendLine("#List#")
            .AppendLine("    return self.layout")
            .AppendLine("end")
            .AppendLine("")
            .AppendLine("return NameTemplateLayout")
            .AppendLine("");

        #endregion

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var originalColor = GUI.backgroundColor;
            GUILayout.Space(5);
            GUI.backgroundColor = Color.yellow;

            var luaUITemplate = target as LuaUITemplate;
            if (null == luaUITemplate)
            {
                LogHelper.PrintError("[LuaUITemplateEditor]LuaUITemplate is null.");
                return;
            }
            var luaUIPanel = luaUITemplate.transform.GetComponentInParent<LuaUIPanel>();
            if (null == luaUIPanel)
            {
                LogHelper.PrintError("[LuaUITemplateEditor]LuaUIPanel is null.");
                return;
            }

            var panelName = luaUIPanel.gameObject.name;
            var panelPath = $"{LuaUIEditorHelper.FilePath}{panelName}/";
            var templateName = $"{panelName}{luaUITemplate.gameObject.name}";

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Space(5);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Create Template", GUILayout.Height(26)))
            {
                var fileName = $"{panelPath}{templateName}Template.lua";
                var codeText = _templateBuilder.ToString().Replace("Name", templateName)
                    .Replace("Time", DateTime.Now.ToString());

                if (File.Exists(fileName))
                {
                    GUIUtility.systemCopyBuffer = codeText;
                    EditorUtility.DisplayDialog("Dialog", $"file:{templateName}Template.lua save to clipboard!", "Ok");
                    return;
                }
                if (!Directory.Exists(panelPath))
                {
                    Directory.CreateDirectory(panelPath);
                }
                var tw = new StreamWriter(fileName);
                tw.Close();
                File.WriteAllText(fileName, codeText);

                EditorUtility.DisplayDialog("Dialog", $"file:{templateName}Template.lua create success!", "Ok");

                AssetDatabase.Refresh();
            }

            GUILayout.Space(5);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Update Layout", GUILayout.Height(26)))
            {
                var allCom = luaUITemplate.gameObject.GetComponentsInChildren<LuaUICom>(true);
                for (var i = 0; i < allCom.Length; i++)
                {
                    DestroyImmediate(allCom[i]);
                }

                //LuaUICom
                var allNode = luaUITemplate.gameObject.GetComponentsInChildren<Transform>(true);
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
                        com.LuaUITemplate = luaUITemplate;
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
                luaUITemplate.LuaUIComArray = conList.ToArray();

                //
                var fileName = $"{panelPath}{templateName}TemplateLayout.lua";
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

                var codeText = _layoutBuilder.ToString().Replace("Name", templateName)
                    .Replace("Time", DateTime.Now.ToString());

                var codeBuilder = new StringBuilder();
                for (var i = 0; i < luaUITemplate.LuaUIComArray.Length; i++)
                {
                    var name = luaUITemplate.LuaUIComArray[i].LuaUIComName;
                    var index = i.ToString();
                    codeBuilder.AppendLine(LuaUIEditorHelper.LuaCom.Replace("Name", name)
                        .Replace("index", index));
                }

                codeText = codeText.Replace("#List#", codeBuilder.ToString());
                File.WriteAllText(fileName, codeText);

                var info = infoBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(info))
                {
                    var title = "!!!Error>>>>>>>>>>>>>>>\r\n\r\n";
                    EditorUtility.DisplayDialog("Dialog", $"{title}{info}", "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("Dialog", $"file:{templateName}TemplateLayout.lua create success!", "Ok");
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = originalColor;
        }
    }
}