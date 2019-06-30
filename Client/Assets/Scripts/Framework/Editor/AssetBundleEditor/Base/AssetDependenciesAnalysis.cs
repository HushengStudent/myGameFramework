/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/21 14:23:58
** desc:  资源依赖关系分析;
*********************************************************************************/

using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using System.Diagnostics;
//using Debug = UnityEngine.Debug;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 依赖关系分析机制;
    /// 获取打包目录下的全部资源,分析全部的依赖关系,其中脚本,Shader和需要单独打包的资源需要特殊处理;
    /// 1.读取全部资源,分析依赖关系,ps:项目AssetBundle打包目录下的资源,不应该引用非打包目录下的资源,如APrefab在打包目录,BPrefab在Resource目录下,
    ///   APrefab依赖于BPrefab,则会导致BPrefab资源冗余;
    /// 2.找出parentDependentAssets==0,parentDependentAssets>1和在需要单独打包的资源目录下的资源,这些资源是需要单独打包的,脚本单独打包,在游戏
    ///   开始时全部加载常驻,Shader资源单独打包,在游戏开始时全部加载常驻;
    /// 3.遍历全部资源,设置资源AssetBundle Name,其中全部Shader设置为同一AssetBundle Name,不需要单独打包的资源AssetBundle Name设置为None;
    /// 4.存储依赖关系;
    /// 5.开始打包;
    /// </summary>
    public class AssetDependenciesAnalysis
    {
        /// <summary>
        /// 打包路径下的全部资源;
        /// </summary>
        public Dictionary<string, AssetNode> allAsset = new Dictionary<string, AssetNode>();

        /// <summary>
        /// 需要单独打包的资源;
        /// </summary>
        public Dictionary<string, AssetNode> independenceAsset = new Dictionary<string, AssetNode>();

        /// <summary>
        /// 全部引用的Shader资源;
        /// </summary>
        public HashSet<string> allShaderAsset = new HashSet<string>();

        /// <summary>
        /// 分析全部资源依赖关系;
        /// </summary>
        public void AnalysisAllAsset()
        {
            Stopwatch watch = Stopwatch.StartNew();//开启计时;

            string[] allPath = Directory.GetFiles(FilePathHelper.resPath, "*.*", SearchOption.AllDirectories);

            //剔除.meta文件;
            List<string> allAssetPath = new List<string>();
            foreach (string tempPath in allPath)
            {
                string path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".meta") continue;
                allAssetPath.Add(path);
            }

            //开始分析资源依赖关系;
            for (int i = 0; i < allAssetPath.Count; i++)
            {
                EditorUtility.DisplayProgressBar("AssetBundle打包提示", "获取需要打包的资源", ((float)i / (float)allAssetPath.Count));

                //还未遍历到该资源;
                if (!allAsset.ContainsKey(allAssetPath[i]))
                {
                    allAsset[allAssetPath[i]] = CreateNewAssetNode(allAssetPath[i]);
                }
                //获取依赖关系;
                string[] allDirectDependencies = AssetDatabase.GetDependencies(allAssetPath[i], false);
                foreach (string tempPath in allDirectDependencies)
                {
                    //依赖脚本直接添加到脚本队列;
                    if (AssetBundleDefine.GetAssetType(tempPath) == AssetType.Scripts)
                    {
                        continue;
                    }
                    //依赖Shader直接添加到Shader队列;
                    if (AssetBundleDefine.GetAssetType(tempPath) == AssetType.Shader)
                    {
                        allShaderAsset.Add(tempPath);
                        continue;
                    }
                    if (tempPath.Contains(FilePathHelper.resPath))
                    {
                        //添加依赖的资源信息;
                        allAsset[allAssetPath[i]].sonDependentAssets.Add(tempPath);
                        //添加被依赖的资源信息;
                        if (!allAsset.ContainsKey(tempPath))
                        {
                            allAsset[tempPath] = CreateNewAssetNode(tempPath);
                        }
                        allAsset[tempPath].parentDependentAssets.Add(allAssetPath[i]);
                    }
                    else
                    {
                        //需要打包AssetBundle的资源目录下的资源,引用非该目录下的资源;
                        LogHelper.Print("[Asset Dependencies Analysis] path:" + allAssetPath[i] + "--->>>reference--->>>: " + tempPath);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            //找出需要打包的资源;
            for (int i = 0; i < allAssetPath.Count; i++)
            {
                EditorUtility.DisplayProgressBar("AssetBundle打包提示", "分析需要打包的资源", ((float)i / (float)allAssetPath.Count));

                //图集特殊处理;
                /*
                if (allAssetPath[i].Contains("Atlas") && Path.GetExtension(allAssetPath[i]) == ".prefab")//ngui
                {
                    independenceAsset[allAssetPath[i]] = allAsset[allAssetPath[i]];
                    continue;
                }
                */
                if (allAssetPath[i].Contains(FilePathHelper.resPath + "Shaders") && Path.GetExtension(allAssetPath[i]) == ".shader")
                {
                    allShaderAsset.Add(allAssetPath[i]);
                    continue;
                }
                if (allAsset[allAssetPath[i]].parentDependentAssets.Count == 0 ||//没有被依赖的资源;
                    allAsset[allAssetPath[i]].parentDependentAssets.Count > 1 ||//被超过一个资源依赖的资源;
                    allAssetPath[i].Contains(FilePathHelper.singleResPath))//指定要求单独打包的资源;
                {
                    independenceAsset[allAssetPath[i]] = allAsset[allAssetPath[i]];
                }
            }
            EditorUtility.ClearProgressBar();

            //设置资源AssetBundle Name;
            for (int i = 0; i < allAssetPath.Count; i++)
            {
                EditorUtility.DisplayProgressBar("AssetBundle打包提示", "设置AssetBundle Name", ((float)i / (float)allAssetPath.Count));

                AssetImporter importer = AssetImporter.GetAtPath(allAssetPath[i]);
                if (importer != null)
                {
                    if (independenceAsset.ContainsKey(allAssetPath[i]))
                    {
                        importer.assetBundleName = FilePathHelper.GetAssetBundleFileName(independenceAsset[allAssetPath[i]].type, independenceAsset[allAssetPath[i]].assetName);
                    }
                    else
                    {
                        importer.assetBundleName = null;
                    }
                    AssetDatabase.ImportAsset(allAssetPath[i]);
                }
            }
            EditorUtility.ClearProgressBar();

            int index = 0;
            //设置Shader AssetBundle Name;
            foreach (string tempPath in allShaderAsset)
            {
                index++;
                EditorUtility.DisplayProgressBar("AssetBundle打包提示", "设置Shader AssetBundle Name", ((float)index / (float)allShaderAsset.Count));
                AssetImporter importer = AssetImporter.GetAtPath(tempPath);
                if (importer != null)
                {
                    importer.assetBundleName = FilePathHelper.GetAssetBundleFileName(AssetType.Shader, "Shaders");
                    AssetDatabase.ImportAsset(tempPath);
                }
            }

            SetLuaAssetName();
            //SetAtlasName();

            EditorUtility.ClearProgressBar();

            watch.Stop();
            LogHelper.PrintWarning(string.Format("[AssetDependenciesAnalysis]Asset Dependencies Analysis Spend Time:{0}s", watch.Elapsed.TotalSeconds));

            AssetDatabase.Refresh();
        }

        private void SetLuaAssetName()
        {
            AssetImporter importer = AssetImporter.GetAtPath(FilePathHelper.luaPath);
            if (importer != null)
            {
                importer.assetBundleName = FilePathHelper.GetAssetBundleFileName(AssetType.Lua, "lua");
                AssetDatabase.ImportAsset(FilePathHelper.luaPath);
            }
        }

        private void SetAtlasName()
        {
            string[] allPath = Directory.GetDirectories(FilePathHelper.atlasPath);
            for (int i = 0; i < allPath.Length; i++)
            {
                AssetImporter importer = AssetImporter.GetAtPath(allPath[i]);
                if (importer != null)
                {
                    importer.assetBundleName = ("atlas/" + Path.GetFileName(allPath[i]) + ".assetbundle").ToLower();
                    AssetDatabase.ImportAsset(FilePathHelper.luaPath);
                }
            }
        }

        /// <summary>
        /// 根据路径创建新的资源;
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns>资源节点</returns>
        public AssetNode CreateNewAssetNode(string path)
        {
            return new AssetNode()
            {
                type = AssetBundleDefine.GetAssetType(path),
                assetName = path.Replace(FilePathHelper.resPath, ""),
                assetPath = path,
                parentDependentAssets = new HashSet<string>(),
                sonDependentAssets = new HashSet<string>()
            };
        }

        /// <summary>
        /// AssetBundles can contain scripts as TextAssets but as such they will not be actual executable code. If you want to include
        /// code in your AssetBundles that can be executed in your application it needs to be pre-compiled into an assembly and loaded
        /// using the Mono Reflection class (Note: Reflection is not available on platforms that use AOT compilation, such as iOS);
        /// </summary>
        public void BuildAllScripts()
        {

        }

        public void ClearAllAssetBundleName()
        {
            string[] allPath = Directory.GetFiles("Assets/", "*.*", SearchOption.AllDirectories);

            //剔除.meta文件;
            List<string> allAssetPath = new List<string>();
            foreach (string tempPath in allPath)
            {
                string path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".meta" || Path.GetExtension(path) == ".cs") continue;
                allAssetPath.Add(path);
            }
            float i = 0;
            foreach (string str in allAssetPath)
            {
                EditorUtility.DisplayProgressBar("AssetBundle打包提示", "删除AssetBundle Name", ((float)i / (float)allAssetPath.Count));
                i++;
                AssetImporter importer = AssetImporter.GetAtPath(str);
                if (importer != null)
                {
                    importer.assetBundleName = null;
                    AssetDatabase.ImportAsset(FilePathHelper.luaPath);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        public void DeleteAllAssetBundle()
        {
            string[] allPath = Directory.GetFiles(FilePathHelper.AssetBundlePath, "*.*", SearchOption.AllDirectories);
            foreach (string str in allPath)
            {
                File.Delete(str);
            }
            Directory.Delete(FilePathHelper.AssetBundlePath, true);
        }
    }

    /// <summary>
    /// 打包分析资源节点信息;
    /// </summary>
    public class AssetNode
    {
        /// <summary>
        /// 资源类型;
        /// </summary>
        public AssetType type;
        /// <summary>
        /// 资源名字;
        /// </summary>
        public string assetName;
        /// <summary>
        /// 资源路径;
        /// </summary>
        public string assetPath;
        /// <summary>
        /// 被依赖的全部资源节点信息;
        /// </summary>
        public HashSet<string> parentDependentAssets;
        /// <summary>
        /// 依赖的全部资源节点信息;
        /// </summary>
        public HashSet<string> sonDependentAssets;
    }
}