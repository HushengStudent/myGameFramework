/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 23:24:40
** desc:  AssetBundle打包;
*********************************************************************************/

using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
//using Debug = UnityEngine.Debug;

namespace Framework
{
    public static class AssetBundleEditor
    {
        [MenuItem("myGameFramework/AssetBundleTools/Build AssetBundle", false, 0)]
        private static void BuildAll()
        {
            if (EditorUtility.DisplayDialog("AssetBundle打包提示", "开始打包AssetBundle？", "打包AssetBundle"))
            {
                AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
                analysiser.AnalysisAllAsset();
                //tips:Unity5.x Scripts not need to build AssetBundle
                //analysiser.BuildAllScripts();
                BuildAssetBundle(FilePathHelper.AssetBundlePath);
            }
        }

        [MenuItem("myGameFramework/AssetBundleTools/Delete AssetBundle", false, 1)]
        private static void DeleteAll()
        {
            AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
            analysiser.DeleteAllAssetBundle();
            AssetDatabase.Refresh();
        }

        [MenuItem("myGameFramework/AssetBundleTools/Clear AssetName", false, 2)]
        private static void ClearAll()
        {
            AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
            analysiser.ClearAllAssetBundleName();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据AssetBundle Name打包全部AssetBundle;
        /// </summary>
        /// <param name="buildPath">目标路径</param>
        public static void BuildAssetBundle(string buildPath)
        {
            Stopwatch watch = Stopwatch.StartNew();//开启计时;
            BuildPipeline.BuildAssetBundles(buildPath, AssetBuildDefine.options, AssetBuildDefine.buildTarget);
            watch.Stop();
            LogUtil.LogUtility.PrintWarning(string.Format("[BuildBat]BuildAllAssetBundle Spend Time:{0}s", watch.Elapsed.TotalSeconds));
            AssetDatabase.Refresh();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
}
