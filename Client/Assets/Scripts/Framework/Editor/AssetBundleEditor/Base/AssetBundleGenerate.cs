/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 23:24:40
** desc:  AssetBundle打包;
*********************************************************************************/

using Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace FrameworkEditor
{
    public static class AssetBundleGenerate
    {
        [MenuItem("myGameFramework/AssetBundle/Build AssetBundle", false, 0)]
        public static void BuildAll()
        {
            if (EditorUtility.DisplayDialog("AssetBundle Build", "开始打包AssetBundle？", "Build AssetBundle"))
            {
                BuildAllCommand();
            }
        }

        public static void BuildAllCommand()
        {
            //ToLuaMenu.BuildLuaToBundles();

            ToLuaMenu.CopyLuaFilesToBundles();

            var analysiser = new AssetDependenciesAnalysis();
            var list = analysiser.AnalysisAllAsset();

            //tips:Unity5.x Scripts not need to build AssetBundle
            //analysiser.BuildAllScripts();
            BuildAssetBundle(FilePathHelper.AssetBundlePath, list);

            ExportABPackage.CopyAssetBundle(FilePathHelper.StreamingAssetsPath);
        }

        [MenuItem("myGameFramework/AssetBundle/Delete AssetBundle", false, 1)]
        public static void DeleteAll()
        {
            var analysiser = new AssetDependenciesAnalysis();
            analysiser.DeleteAllAssetBundle();
            AssetDatabase.Refresh();
        }

        [MenuItem("myGameFramework/AssetBundle/Clear AssetName", false, 2)]
        public static void ClearAll()
        {
            var analysiser = new AssetDependenciesAnalysis();
            analysiser.ClearAllAssetBundleName();
            AssetDatabase.Refresh();
        }

        private static void BuildAssetBundle(string buildPath, List<AssetBundleBuild> list)
        {
            var watch = Stopwatch.StartNew();//开启计时;
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            BuildPipeline.BuildAssetBundles(buildPath, list.ToArray(), AssetBuildDefine.options, AssetBuildDefine.buildTarget);
            watch.Stop();
            LogHelper.PrintWarning($"GenerateAllAssetBundle Spend Time:{watch.Elapsed.TotalSeconds}s.");
            AssetDatabase.Refresh();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
}

