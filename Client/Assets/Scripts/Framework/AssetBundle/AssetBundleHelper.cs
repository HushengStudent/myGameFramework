/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2021/12/12 23:05:45
** desc:  #####
*********************************************************************************/

using System.IO;
using UnityEngine;

namespace Framework.AssetBundleModule
{
    public static class AssetBundleHelper
    {
        /// <summary>
        /// 需要打包的资源所在的目录;
        /// </summary>
        public static readonly string ResPath = "Assets/Bundles/";

        public static readonly string ScenePath = "Assets/Bundles/Scene";

        /// <summary>
        /// 需要打包的lua文件;
        /// </summary>
        public static readonly string LuaPath = "Assets/Bundles/Lua";

        public static readonly string LuaAssetBundleName = "Lua";

        /// <summary>
        /// 需要打包的shader文件;
        /// </summary>
        public static readonly string ShaderPath = "Assets/Bundles/Shaders";

        public static readonly string ShaderAssetBundleName = "Shaders";

        /// <summary>
        /// 获取AssetBundle文件的名字;
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetAssetBundleFileName(string path)
        {
            string assetBundleName = null;

            if (string.IsNullOrEmpty(path))
            {
                return assetBundleName;
            }
            //AssetBundle的名字不支持大写;
            assetBundleName = (HashHelper.GetMD5(path) + ".ab").ToLower();
            return assetBundleName;
        }

        /// <summary>
        /// 获取AssetBundle文件加载路径;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetAssetBundlePath(string path)
        {
            var assetBundleName = GetAssetBundleFileName(path);
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return null;
            }
            return GetAssetBundlePathByName(assetBundleName);
        }

        public static string GetMainAssetBundlePath()
        {
            return GetAssetBundlePathByName("AssetBundle");
        }

        public static string GetAssetBundlePathByName(string assetBundleName)
        {
            var patchDirectory = $"{FilePathHelper.PersistentDataPath}/AssetBundle/";
            var patchPath = $"{patchDirectory}/{assetBundleName}";
            if (File.Exists(patchPath))
            {
                return patchPath;
            }
            return $"{FilePathHelper.StreamingAssetsPath}/AssetBundle/{assetBundleName}";
        }
    }
}