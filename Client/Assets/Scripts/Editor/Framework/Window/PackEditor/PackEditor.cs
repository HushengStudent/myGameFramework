/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/21 23:40:35
** desc:  打包编辑器;
* 打包流程:
* 1.打AssetBundle;
* 2.压缩AssetBundle;
* 3.拷贝AssetBundle压缩包;
* 4.打包;
* 
* 
*********************************************************************************/

using Framework.EditorModule.AssetBundle;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Framework.EditorModule.Window
{
    public class PackEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/PackEditor &#b", false, 4)]
        static void ShowWindow()
        {
            var window = GetWindowWithRect(typeof(PackEditor), new Rect(0, 0, 1280, 720), true, "打包编辑器");
            window.Show();
        }

        private enum BuildPlatform
        {
            Android = BuildTarget.Android,
            IOS = BuildTarget.iOS,
            Windows = BuildTarget.StandaloneWindows64,
        }

        private static BuildPlatform _buildPlatform = BuildPlatform.Windows;
        private static BuildOptions _buildOptions =
            BuildOptions.AllowDebugging | BuildOptions.Development;
        private static string _locationPathName = string.Empty;
        private static bool _isCompletePack = true;
        private static bool _isBuildAssetBundle = true;
        private static bool _isRelease = false;

        private void OnEnable()
        {
            _locationPathName = Application.dataPath + "/../BuildResult";
            _isCompletePack = true;
            _isBuildAssetBundle = true;
            _isRelease = false;
        }

        void OnGUI()
        {
            var style = new GUIStyle
            {
                fontSize = 18
            };
            var color = GUI.backgroundColor;

            //搜索框;
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            var searchString = "";
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("File", EditorStyles.toolbarDropDown, GUILayout.Width(50)))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("设置"), false, () =>
                {
                    //LogHelper.PrintError("File->设置");
                });
                menu.ShowAsContext();
            }

            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                GUILayout.Space(5);
                using (var vv = new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("打包", style);
                    GUILayout.Space(5);
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("选择目标平台:", style);
                        GUILayout.Space(15);
                        _buildPlatform = (BuildPlatform)EditorGUILayout.EnumPopup(_buildPlatform);
                    }
                    GUILayout.Space(5);
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("打包选项:", style);
                        GUILayout.Space(15);
                        _buildOptions = (BuildOptions)EditorGUILayout.EnumFlagsField(_buildOptions);
                    }
                    GUILayout.Space(5);
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("是否打Release包:", style);
                        GUILayout.Space(15);
                        bool curValue = _isRelease;
                        _isRelease = EditorGUILayout.Toggle(_isRelease);
                        if (_isRelease)
                        {
                            _buildOptions = BuildOptions.None;
                        }
                        else
                        {
                            if (curValue)
                            {
                                _buildOptions = BuildOptions.AllowDebugging | BuildOptions.Development;
                            }
                        }
                    }
                    GUILayout.Space(5);
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("打包资源完整完整包体:", style);
                        GUILayout.Space(15);
                        _isCompletePack = EditorGUILayout.Toggle(_isCompletePack);
                    }
                    GUILayout.Space(5);
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("重新打包AssetBundle:", style);
                        GUILayout.Space(15);
                        _isBuildAssetBundle = EditorGUILayout.Toggle(_isBuildAssetBundle);
                        if (_isRelease)
                        {
                            _isBuildAssetBundle = true;
                        }
                    }
                    GUILayout.Space(20);
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("开始打包", GUILayout.Height(30)))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确认开始打包?", "确认"))
                        {
                            _locationPathName = EditorUtility.OpenFolderPanel("选择输出文件夹", _locationPathName, "");
                            EditorApplication.delayCall += () =>
                            {
                                CommandLineBuild();
                            };
                            Close();
                        }
                    }
                    GUI.backgroundColor = color;
                }
                GUILayout.Space(10);
                using (var vv = new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("打热更包", style);
                    GUILayout.Space(5);
                }
            }
        }

        public static void CommandLineBuild()
        {
            Debug.Log("[PackEditor]start build.");
            if (string.IsNullOrEmpty(_locationPathName))
            {
                return;
            }
            if (Directory.Exists(_locationPathName))
            {
                Directory.Delete(_locationPathName, true);
            }
            Directory.CreateDirectory(_locationPathName);

            var extensionName = string.Empty;
            SwitchActiveBuildTarget(() =>
            {
                BuildAssetBundle();
                BuildPlayer();
            });
        }

        private class ActiveBuildTargetListener : IActiveBuildTargetChanged
        {
            public int callbackOrder { get { return 0; } }
            public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
            {
                Debug.Log("[PackEditor]Switched build target to " + newTarget);
                BuildAssetBundle();
                BuildPlayer();
            }
        }

        private static void SwitchActiveBuildTarget(Action action)
        {
            Debug.Log("[PackEditor]switch build target.");
            var callback = action;
            switch (_buildPlatform)
            {
                case BuildPlatform.Android:
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                    {
                        new ActiveBuildTargetListener();
                        callback = null;
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    }
                    break;
                case BuildPlatform.Windows:
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
                    {
                        new ActiveBuildTargetListener();
                        callback = null;
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                    }
                    break;
                default:
                    break;
            }
            callback?.Invoke();
        }

        private static void BuildAssetBundle()
        {
            Debug.Log("[PackEditor]build assetBundle.");
            if (_isBuildAssetBundle)
            {
                AssetBundleGenerate.CommandLineBuildAll();
            }
            if (_isCompletePack)
            {
                ExportABPackage.CopyAssetBundle(FilePathHelper.StreamingAssetsPath);
            }
            else
            {
                if (File.Exists(ExportABPackage.ZipStreamingAssetsPath))
                {
                    File.Delete(ExportABPackage.ZipStreamingAssetsPath);
                }
            }
        }

        private static void BuildPlayer()
        {
            Debug.Log("[PackEditor]build player.");
            var extensionName = string.Empty;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    extensionName = ".apk";
                    break;
                case BuildTarget.StandaloneWindows64:
                    extensionName = ".exe";
                    break;
                default:
                    break;
            }

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = $"{_locationPathName}/{PlayerSettings.productName}{extensionName}",
                target = (BuildTarget)_buildPlatform,
                options = _buildOptions
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                LogHelper.PrintError($"打包成功.");
            }
            else
            {
                LogHelper.PrintError($"打包失败:{report.summary.totalErrors}.");
            }
        }
    }
}