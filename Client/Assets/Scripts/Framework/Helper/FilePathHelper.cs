/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/21 14:09:40
** desc:  文件路径管理工具;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public static class FilePathHelper
    {
        public static string AssetBundlePath { get; } = $"{Application.dataPath}/../AssetBundle";
        public static string StreamingAssetsPath { get; } = $"{Application.dataPath}/StreamingAssets";

        /// <summary>
        /// 需要打包的资源所在的目录;
        /// </summary>
        public static readonly string resPath = "Assets/Bundles/";

        public static readonly string scenePath = "Assets/Bundles/Scene";

        /// <summary>
        /// 需要打包的lua文件;
        /// </summary>
        public static readonly string luaPath = "Assets/Bundles/Lua";

        public static readonly string luaAssetBundleName = "Lua";

        /// <summary>
        /// 需要打包的shader文件;
        /// </summary>
        public static readonly string shaderPath = "Assets/Bundles/Shaders";

        public static readonly string shaderAssetBundleName = "Shaders";

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
            string assetBundleName = GetAssetBundleFileName(path);
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return null;
            }
            return $"{AssetBundlePath}/{assetBundleName}";
        }

        /// <summary>
        /// 获取Resource文件加载路径;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetResourcePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            string assetPath = path;
            return assetPath;
        }
    }
}
