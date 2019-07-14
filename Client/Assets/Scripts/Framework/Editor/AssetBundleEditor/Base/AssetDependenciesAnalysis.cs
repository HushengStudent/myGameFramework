/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/21 14:23:58
** desc:  资源依赖关系分析;
*********************************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 依赖关系分析机制;
    /// 获取打包目录下的全部资源,分析全部的依赖关系,其中脚本,Shader和需要单独打包的资源需要特殊处理;
    /// 1.读取全部资源,分析依赖关系,ps:项目AssetBundle打包目录下的资源,不应该引用特殊目录下的资源;
    ///   如APrefab在打包目录,BPrefab在Resource目录下,APrefab依赖于BPrefab,则会导致BPrefab资源冗余;
    /// 2.找出parentDependentAssets==0,parentDependentAssets>1和在Bundles资源目录下的资源;
    ///   这些资源是需要单独打包的,脚本单独打包,在游戏开始时全部加载常驻,Shader资源单独打包,在游戏开始时全部加载常驻;
    /// 3.存储依赖关系;
    /// 4.开始打包;
    /// </summary>
    public class AssetDependenciesAnalysis
    {
        /// <summary>
        /// 打包的全部资源;
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
        /// 全部引用的Lua资源;
        /// </summary>
        public HashSet<string> allLuaAsset = new HashSet<string>();

        /// <summary>
        /// 分析全部资源依赖关系;
        /// </summary>
        /// <returns></returns>
        public List<AssetBundleBuild> AnalysisAllAsset()
        {
            Stopwatch watch = Stopwatch.StartNew();//开启计时;

            string[] allPath = Directory.GetFiles(FilePathHelper.resPath, "*.*", SearchOption.AllDirectories);

            //剔除.meta文件;
            List<string> allAssetPath = new List<string>();
            foreach (string tempPath in allPath)
            {
                string path = tempPath.Replace("\\", "/");
                if (Path.GetExtension(path) == ".meta")
                {
                    continue;
                }
                allAssetPath.Add(path);
            }

            //开始分析资源依赖关系;
            for (int i = 0; i < allAssetPath.Count; i++)
            {
                if (!CheckAssetNode(allAssetPath[i]))
                {
                    continue;
                }

                //还未遍历到该资源;
                if (!allAsset.ContainsKey(allAssetPath[i]))
                {
                    allAsset[allAssetPath[i]] = CreateNewAssetNode(allAssetPath[i]);
                }

                //获取依赖关系;
                string[] allDirectDependencies = AssetDatabase.GetDependencies(allAssetPath[i], false);

                foreach (string tempPath in allDirectDependencies)
                {
                    if (!CheckAssetNode(tempPath))
                    {
                        continue;
                    }

                    //添加依赖的资源信息;
                    allAsset[allAssetPath[i]].sonDependentAssets.Add(tempPath);
                    //添加被依赖的资源信息;
                    if (!allAsset.ContainsKey(tempPath))
                    {
                        allAsset[tempPath] = CreateNewAssetNode(tempPath);
                    }
                    allAsset[tempPath].parentDependentAssets.Add(allAssetPath[i]);
                }
            }

            foreach (var tempAsset in allAsset)
            {
                if (tempAsset.Value.parentDependentAssets.Count == 0 ||//没有被依赖的资源;
                    tempAsset.Value.parentDependentAssets.Count > 1 ||//被超过一个资源依赖的资源;
                    tempAsset.Key.Contains(FilePathHelper.resPath))//Bundles资源目录下的资源,允许加载所以单独打包;
                {
                    independenceAsset[tempAsset.Key] = tempAsset.Value;
                }
            }

            List<AssetBundleBuild> builderList = new List<AssetBundleBuild>();

            foreach (var asset in independenceAsset)
            {
                var node = asset.Value;
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = FilePathHelper.GetAssetBundleFileName(node.assetPath);
                List<string> assetLis = new List<string>();
                assetLis.Add(node.assetPath);

                foreach (var tempAsset in node.sonDependentAssets)
                {
                    if (!independenceAsset.ContainsKey(tempAsset))
                    {
                        assetLis.Add(tempAsset);
                    }
                }
                build.assetNames = assetLis.ToArray();
                builderList.Add(build);
            }

            AssetBundleBuild shaderBuild = new AssetBundleBuild();
            shaderBuild.assetBundleName = FilePathHelper.GetAssetBundleFileName(FilePathHelper.shaderAssetBundleName);
            List<string> shaderList = new List<string>();
            foreach (var shader in allShaderAsset)
            {
                shaderList.Add(shader);
            }
            shaderBuild.assetNames = shaderList.ToArray();
            builderList.Add(shaderBuild);

            AssetBundleBuild luaBuild = new AssetBundleBuild();
            luaBuild.assetBundleName = FilePathHelper.GetAssetBundleFileName(FilePathHelper.luaAssetBundleName);
            List<string> luaList = new List<string>();
            foreach (var lua in allLuaAsset)
            {
                luaList.Add(lua);
            }
            luaBuild.assetNames = luaList.ToArray();
            builderList.Add(luaBuild);

            watch.Stop();

            LogHelper.PrintWarning(string.Format("[AssetDependenciesAnalysis]Asset Dependencies Analysis Spend Time:{0}s", watch.Elapsed.TotalSeconds));

            SaveBuildInfo(builderList);

            return builderList;
        }

        /// <summary>
        /// 根据路径创建新的资源;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssetNode CreateNewAssetNode(string path)
        {
            return new AssetNode()
            {
                assetPath = path,
                parentDependentAssets = new HashSet<string>(),
                sonDependentAssets = new HashSet<string>()
            };
        }

        private bool CheckAssetNode(string path)
        {
            //依赖脚本直接添加到脚本队列;
            if (Path.GetExtension(path) == ".cs")
            {
                return false;
            }
            //依赖Shader直接添加到Shader队列;
            if (Path.GetExtension(path) == ".shader")
            {
                allShaderAsset.Add(path);
                return false;
            }
            if (Path.GetExtension(path) == ".bytes" && path.Contains(FilePathHelper.luaPath))
            {
                allLuaAsset.Add(path);
                return false;
            }
            return true;
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
                if (Path.GetExtension(path) == ".meta" || Path.GetExtension(path) == ".cs")
                {
                    continue;
                }
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
                    AssetDatabase.ImportAsset(str);
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

        private void SaveBuildInfo(List<AssetBundleBuild> info)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < info.Count; i++)
            {
                builder.AppendLine("");
                builder.AppendLine(i.ToString());
                builder.AppendLine(info[i].assetBundleName);
                for (int j = 0; j < info[i].assetNames.Length; j++)
                {
                    builder.AppendLine(info[i].assetNames[j]);
                }
            }
            FileHelper.Write2Txt(Application.dataPath + "/Editor/AssetBundleBuild.txt", builder.ToString());
        }
    }

    /// <summary>
    /// 打包分析资源节点信息;
    /// </summary>
    public class AssetNode
    {
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