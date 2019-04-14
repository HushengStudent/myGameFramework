/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 23:24:40
** desc:  AssetBundle���;
*********************************************************************************/

using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class AssetBundleGenerate
    {
        [MenuItem("myGameFramework/AssetBundleTools/Generate AssetBundle", false, 0)]
        private static void GenerateAll()
        {
            if (EditorUtility.DisplayDialog("AssetBundle�����ʾ", "��ʼ���AssetBundle��", "���AssetBundle"))
            {
                AssetDependenciesAnalysis analysiser = new AssetDependenciesAnalysis();
                analysiser.AnalysisAllAsset();
                //tips:Unity5.x Scripts not need to build AssetBundle
                //analysiser.BuildAllScripts();
                GenerateAssetBundle(FilePathHelper.AssetBundlePath);
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
        /// ����AssetBundle Name���ȫ��AssetBundle;
        /// </summary>
        /// <param name="buildPath">Ŀ��·��</param>
        public static void GenerateAssetBundle(string buildPath)
        {
            Stopwatch watch = Stopwatch.StartNew();//������ʱ;
            BuildPipeline.BuildAssetBundles(buildPath, AssetBuildDefine.options, AssetBuildDefine.buildTarget);
            watch.Stop();
            LogHelper.PrintWarning(string.Format("GenerateAllAssetBundle Spend Time:{0}s", watch.Elapsed.TotalSeconds));
            AssetDatabase.Refresh();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
}

