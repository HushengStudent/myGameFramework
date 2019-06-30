/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 23:24:40
** desc:  AssetBundle打包;
*********************************************************************************/

using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class AssetBundleGenerate
    {
        [MenuItem("myGameFramework/AssetBundleTools/Generate AssetBundle", false, 0)]
        public static void GenerateAll()
        {
            if (EditorUtility.DisplayDialog("AssetBundle打包提示", "开始打包AssetBundle？", "打包AssetBundle"))
            {
                AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
                analysiser.AnalysisAllAsset();
                //tips:Unity5.x Scripts not need to build AssetBundle
                //analysiser.BuildAllScripts();
                GenerateAssetBundle(FilePathHelper.AssetBundlePath);
            }
        }

        [MenuItem("myGameFramework/AssetBundleTools/Delete AssetBundle", false, 1)]
        public static void DeleteAll()
        {
            AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
            analysiser.DeleteAllAssetBundle();
            AssetDatabase.Refresh();
        }

        [MenuItem("myGameFramework/AssetBundleTools/Clear AssetName", false, 2)]
        public static void ClearAll()
        {
            AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
            analysiser.ClearAllAssetBundleName();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据AssetBundle Name打包全部AssetBundle;
        /// </summary>
        /// <param name="buildPath">目标路径</param>
        private static void GenerateAssetBundle(string buildPath)
        {
            Stopwatch watch = Stopwatch.StartNew();//开启计时;
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            BuildPipeline.BuildAssetBundles(buildPath, AssetBuildDefine.options, AssetBuildDefine.buildTarget);
            watch.Stop();
            LogHelper.PrintWarning(string.Format("GenerateAllAssetBundle Spend Time:{0}s", watch.Elapsed.TotalSeconds));
            AssetDatabase.Refresh();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
}

