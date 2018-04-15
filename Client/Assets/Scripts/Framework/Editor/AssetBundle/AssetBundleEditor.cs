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

namespace Common
{
    public static class AssetBundleEditor
    {
        [MenuItem("MGame/AssetBundleTools/Build AssetBundle", false, 0)]
        private static void BuildAll()
        {
            bool state = false;
            state = EditorUtility.DisplayDialog("提示", "是否确认重新生成依赖关系？\r\n\r\n不能确认请重新生成！", "重新分析", "直接打包");
            if (state)
            {
                //DeleteAll();
                //ClearAll();
                AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
                analysiser.AnalysisAllAsset();
                //tips:Unity5.x Scripts not need to build AssetBundle
                //analysiser.BuildAllScripts();
            }
            BuildAssetBundle(FilePathUtility.AssetBundlePath);
        }

        //[MenuItem("MGame/AssetBundleTools/Clear AssetName", false, 1)]
        private static void ClearAll()
        {
            AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
            analysiser.ClearAllAssetBundleName();
            AssetDatabase.Refresh();
        }

        [MenuItem("MGame/AssetBundleTools/Delete AssetBundle", false, 2)]
        private static void DeleteAll()
        {
            AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
            analysiser.DeleteAllAssetBundle();
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
