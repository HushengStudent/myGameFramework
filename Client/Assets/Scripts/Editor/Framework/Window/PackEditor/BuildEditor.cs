/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/21 23:40:35
** desc:  打包编辑器;
*********************************************************************************/

using Framework.EditorModule.AssetBundle;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Framework.EditorModule.Window
{
    public class BuildEditor : EditorWindow
    {
        [MenuItem("myGameFramework/Window/BuildEditor &#b", false, 4)]
        static void ShowWindow()
        {
            var window = GetWindow<BuildEditor>("打包编辑器");
            window.Show();
        }

        private enum BuildPlatform
        {
            Android = BuildTarget.Android,
            IOS = BuildTarget.iOS,
            Windows = BuildTarget.StandaloneWindows64,
        }

        /// <summary>
        /// 渠道;
        /// </summary>
        private enum BuildChannel
        {
            Oppo = 1,
            Vivo = 2,
        }

        private static BuildPlatform _buildPlatform = BuildPlatform.Windows;
        private static BuildOptions _buildOptions = BuildOptions.AllowDebugging | BuildOptions.Development;
        private static BuildChannel _buildChannel = BuildChannel.Oppo;

        private static string _buildOutputPath = string.Empty;
        private static int _buildVersionCode = 1;
        private static bool _isCompleteBuild = true;
        private static bool _isBuildAssetBundle = true;
        private static bool _isRelease = false;

        private void OnEnable()
        {
            _buildOutputPath = $"{Application.dataPath}/../BuildResult";
            _buildChannel = BuildChannel.Oppo;
            _buildVersionCode = 1;
            _isCompleteBuild = true;
            _isBuildAssetBundle = true;
            _isRelease = false;
        }

        void OnGUI()
        {
            var style = new GUIStyle
            {
                fontSize = 18,
            };

            var color = GUI.backgroundColor;
            var contentColor = GUI.contentColor;

            /*
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
            */

            using (var v = new EditorGUILayout.VerticalScope(GUI.skin.window))
            {
                using (var vv = new EditorGUILayout.VerticalScope(GUI.skin.window))
                {
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("选择渠道:", style);
                        GUILayout.Space(15);
                        _buildChannel = (BuildChannel)EditorGUILayout.EnumPopup(_buildChannel);
                    }

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
                        EditorGUILayout.LabelField("版本号:", style);
                        GUILayout.Space(15);
                        var release = _isRelease ? "1" : "0";
                        var versionCode = $"{_buildVersionCode.ToString()}.{release}";
                        var textColor = style.normal.textColor;
                        style.normal.textColor = Color.red;
                        EditorGUILayout.LabelField(versionCode, style);
                        style.normal.textColor = textColor;
                    }

                    GUILayout.Space(10);
                    using (var vvv = new EditorGUILayout.VerticalScope(GUI.skin.window))
                    {
                        EditorGUILayout.LabelField("打游戏包", style);
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
                            _isCompleteBuild = EditorGUILayout.Toggle(_isCompleteBuild);
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
                        if (GUILayout.Button("开始打游戏包", GUILayout.Height(30)))
                        {
                            if (EditorUtility.DisplayDialog("提示", "确认开始打游戏包?", "确认"))
                            {
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
                    using (var vvv = new EditorGUILayout.VerticalScope(GUI.skin.window))
                    {
                        EditorGUILayout.LabelField("打补丁包", style);
                        GUILayout.Space(20);
                        GUI.backgroundColor = Color.yellow;
                        if (GUILayout.Button("开始打补丁包", GUILayout.Height(30)))
                        {
                            if (EditorUtility.DisplayDialog("提示", "确认开始打补丁包?", "确认"))
                            {
                                EditorApplication.delayCall += () =>
                                {
                                };
                                Close();
                            }
                        }
                        GUI.backgroundColor = color;
                    }
                }
            }
        }

        public static void CommandLineBuild()
        {
            Debug.Log("[PackEditor]start build.");

            var platform = _buildPlatform.ToString();
            var release = _isRelease ? "1" : "0";
            var versionCode = $"{_buildVersionCode.ToString()}.{release}";
            var channel = _buildChannel.ToString();
            _buildOutputPath = $"{_buildOutputPath}/{platform}/{channel}/{versionCode}";

            if (string.IsNullOrEmpty(_buildOutputPath))
            {
                return;
            }
            if (Directory.Exists(_buildOutputPath))
            {
                //Directory.Delete(_buildOutputPath, true);
                LogHelper.PrintError($"打包失败:版本已存在.");
                return;
            }
            Directory.CreateDirectory(_buildOutputPath);

            var extensionName = string.Empty;
            SwitchActiveBuildTarget(() =>
            {
                BuildAssetBundle();
                BuildPlayer();
            });
        }

        private static void GetNewestVersionCode()
        {
            var platform = _buildPlatform.ToString();
            var release = _isRelease ? "1" : "0";
            var channel = _buildChannel.ToString();
            _buildOutputPath = $"{_buildOutputPath}/{platform}/{channel}";

            if (!Directory.Exists(_buildOutputPath))
            {
                _buildVersionCode = 1;
                return;
            }
            var directories = Directory.GetDirectories(_buildOutputPath);

            foreach (var dir in directories)
            {
                //TODO;
            }
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
            if (_isCompleteBuild)
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
                locationPathName = $"{_buildOutputPath}/{PlayerSettings.productName}{extensionName}",
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