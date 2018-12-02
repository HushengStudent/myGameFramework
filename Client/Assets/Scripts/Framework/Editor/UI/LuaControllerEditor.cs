/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/17 23:41:41
** desc:  编辑器扩展;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomEditor(typeof(LuaController))]
    public class LuaControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            if (GUILayout.Button("\r\n" + "更新Prefab信息"))
            {
                LuaController ctrl = target as LuaController;
                if (null == ctrl) return;
                LuaComponent[] componentArray = ctrl.gameObject.GetComponentsInChildren<LuaComponent>();
                List<string> nameList = new List<string>();
                for (int i = 0; i < componentArray.Length; i++)
                {
                    if (string.IsNullOrEmpty(componentArray[i].ComponentName))
                    {
                        componentArray[i].ComponentName =
                        componentArray[i].gameObject.name.Replace(" ", "").Replace("(", "").Replace("（", "").Replace(")", "").Replace("）", "").Replace("{", "").Replace("}", "").Replace(".", "");
                        if (nameList.Contains(componentArray[i].ComponentName))
                        {
                            LogHelper.PrintError("[LuaControllerEditor]名字重复：" + componentArray[i].ComponentName);
                        }
                        nameList.Add(componentArray[i].ComponentName);
                    }
                    else
                    {
                        if (nameList.Contains(componentArray[i].ComponentName))
                        {
                            LogHelper.PrintError("[LuaControllerEditor]名字重复：" + componentArray[i].ComponentName);
                        }
                        nameList.Add(componentArray[i].ComponentName);
                    }
                }
                nameList.Clear();
                ctrl.componentArray = new LuaComponent[componentArray.Length];
                for (int i = 0; i < ctrl.componentArray.Length; i++)
                {
                    ctrl.componentArray[i] = componentArray[i];
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("\r\n" + "创建Controller"))
            {
                LuaController ctrl = target as LuaController;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaControllerEditor]LuaController is null.");
                    return;
                }
                string name = ctrl.gameObject.name;
                string fileName = ctrlPath + name + "Ctrl.lua";
                if (File.Exists(fileName))
                {
                    EditorUtility.DisplayDialog("Lua文件生成提示", "Lua文件： " + name + "Ctrl.lua" + " 已存在！", "确认");
                    return;
                }
                TextWriter tw = new StreamWriter(fileName);
                tw.Close();
                string allText = templateLuaCtrl.Replace("ModuleName", name);
                File.WriteAllText(fileName, allText);
                EditorUtility.DisplayDialog("Lua文件生成提示", "Lua文件： " + name + "Ctrl.lua" + " 生成成功！", "确认");
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("\r\n" + "更新 Panel"))
            {
                LuaController ctrl = target as LuaController;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaControllerEditor]LuaController is null.");
                    return;
                }
                string name = ctrl.gameObject.name;
                string fileName = panelPath + name + "Panel.lua";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                TextWriter tw = new StreamWriter(fileName);
                tw.Close();
                string tempStr = templateLuaPanel.Replace("ModuleName", name);
                string comStr = "";
                for (int i = 0; i < ctrl.componentArray.Length; i++)
                {
                    comStr = comStr + luaCom.Replace("ComName", ctrl.componentArray[i].ComponentName).Replace("index", i.ToString());
                }
                string allText = tempStr.Replace("#List#", comStr);
                File.WriteAllText(fileName, allText);
                EditorUtility.DisplayDialog("Lua文件生成提示", "Lua文件： " + name + "Panel.lua" + " 生成成功！", "确认");
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("\r\n" + "创建 Data"))
            {
                LuaController ctrl = target as LuaController;
                if (null == ctrl)
                {
                    LogHelper.PrintError("[LuaControllerEditor]LuaController is null.");
                    return;
                }
                string name = ctrl.gameObject.name;
                string fileName = dataPath + name + "Data.lua";
                if (File.Exists(fileName))
                {
                    EditorUtility.DisplayDialog("Lua文件生成提示", "Lua文件： " + name + "Data.lua" + " 已存在！", "确认");
                    return;
                }
                TextWriter tw = new StreamWriter(fileName);
                tw.Close();
                string allText = templateLuaData.Replace("ModuleName", name);
                File.WriteAllText(fileName, allText);
                EditorUtility.DisplayDialog("Lua文件生成提示", "Lua文件： " + name + "Data.lua" + " 生成成功！", "确认");
                AssetDatabase.Refresh();
            }
        }

        private string templateLuaCtrl =
            "---\r\n" +
            "---This code was generated by a tool.\r\n" +
            "---To coding to do what u want to do.\r\n" +
            "---\r\n" +
            " \r\n" +
            "require \"Panel.View.ModuleNamePanel\"\r\n" +
            "require \"Panel.Data.ModuleNameData\"\r\n" +
            " \r\n" +
            "ModuleNameCtrl = class(\"ModuleNameCtrl\",BaseCtrl)\r\n" +
            " \r\n" +
            "function ModuleNameCtrl:ctor()\r\n" +
            " \r\n" +
            "end\r\n" +
            " \r\n" +
            "function ModuleNameCtrl:Awake(msg)\r\n" +
            "       log(\"--->>>ModuleNameCtrl Awake be called.\")\r\n" +
            "       local l_panel = ModuleNamePanel.new()\r\n" +
            "       self.panel = l_panel:BindLuaComponent(msg[0])\r\n" +
            "end\r\n" +
            " \r\n" +
            "function ModuleNameCtrl:Start()\r\n" +
            "       log(\"--->>>ModuleNameCtrl Start be called.\")\r\n" +
            " \r\n" +
            "end\r\n" +
            " \r\n" +
            "function ModuleNameCtrl:OnEnable()\r\n" +
            "       log(\"--->>>ModuleNameCtrl OnEnable be called.\")\r\n" +
            " \r\n" +
            "end\r\n" +
            " \r\n" +
            "function ModuleNameCtrl:OnDisable()\r\n" +
            "       log(\"--->>>ModuleNameCtrl OnDisable be called.\")\r\n" +
            " \r\n" +
            "end\r\n" +
            " \r\n" +
            "function ModuleNameCtrl:OnDestroy()\r\n" +
            "       log(\"--->>>ModuleNameCtrl OnDestroy be called.\")\r\n" +
            " \r\n" +
            "end\r\n" +
            "-----------------------------超华丽的分割线-----------------------------\r\n";


        private string templateLuaPanel =
            "---\r\n" +
            "---This code was generated by a tool.\r\n" +
            "---Forbid To coding.\r\n" +
            "---\r\n" +
            " \r\n" +
            "ModuleNamePanel = class(\"ModuleNamePanel\")\r\n" +
            " \r\n" +
            "function ModuleNamePanel:ctor()\r\n" +
            " \r\n" +
            "end\r\n" +
            " \r\n" +
            "function ModuleNamePanel:BindLuaComponent(gameObject)\r\n" +
            "       local l_ctrl = gameObject:GetComponent(\"LuaController\")\r\n" +
            "       self.ComponentList = {}\r\n" +
            "#List#" +
            "       return self.ComponentList\r\n" +
            "end\r\n";


        private string luaCom = "       self.ComponentList.ComName = l_ctrl.componentArray[index]\r\n";

        private string templateLuaData =
            "---\r\n" +
            "---This code was generated by a tool.\r\n" +
            "---To coding to do what u want to do.\r\n" +
            "---\r\n" +
            " \r\n" +
            "ModuleNameData = {} \r\n" +
            " \r\n" +
            "function ModuleNameData.SerializNetData(msg)\r\n" +
            "       log(\"--->>>ModuleNameData SerializNetData.\")\r\n" +
            " \r\n" +
            "end\r\n" +
            " \r\n" +
            "function ModuleNameData.Clear()\r\n" +
            "       log(\"--->>>Clear ModuleNameData.\")\r\n" +
            " \r\n" +
            "end\r\n";

        private string ctrlPath = "C:/Users/husheng/Desktop/MyProject/4GameFramework/myGameFramework/Client/Assets" + "/LuaFramework/Lua/Panel/Controller/";
        private string dataPath = "C:/Users/husheng/Desktop/MyProject/4GameFramework/myGameFramework/Client/Assets" + "/LuaFramework/Lua/Panel/Data/";
        private string panelPath = "C:/Users/husheng/Desktop/MyProject/4GameFramework/myGameFramework/Client/Assets" + "/LuaFramework/Lua/Panel/View/";

        /*
        [InitializeOnLoadMethod]
        static void StartInitializeOnLoadMethod()
        {
            PrefabUtility.prefabInstanceUpdated = delegate (GameObject instance)
            {
                LuaController ctrl = instance.GetComponent<LuaController>();
                if (null == ctrl) return;
                LuaComponent[] componentArray = instance.GetComponentsInChildren<LuaComponent>();
                for (int i = 0; i < componentArray.Length; i++)
                {
                    if (string.IsNullOrEmpty(componentArray[i].ComponentName))
                    {
                        componentArray[i].ComponentName = componentArray[i].gameObject.name;
                    }
                }
                ctrl.componentArray = new LuaComponent[componentArray.Length];
                for (int i = 0; i < ctrl.componentArray.Length; i++)
                {
                    ctrl.componentArray[i] = componentArray[i];
                }
                AssetDatabase.SaveAssets();
            };
        }
        */

    }
}
