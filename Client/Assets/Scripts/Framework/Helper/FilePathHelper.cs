/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/21 14:09:40
** desc:  文件路径管理工具;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class FilePathHelper
    {
        /// <summary>
        /// AssetBundle打包存储路径;
        /// </summary>
        private static string assetBundlePath = Application.dataPath + "/../AssetBundle";
        public static string AssetBundlePath
        {
            get { return assetBundlePath; }
        }

        /// <summary>
        /// StreamingAssets路径;
        /// </summary>
        private static string streamingAssetsPath = Application.dataPath + "/StreamingAssets";
        public static string StreamingAssetsPath
        {
            get { return streamingAssetsPath; }
        }

        /// <summary>
        /// 需要打包的资源所在的目录;
        /// </summary>
        public static string resPath = "Assets/Bundles/";

        /// <summary>
        /// 需要打包的lua文件;
        /// </summary>
        public static string luaPath = "Assets/Resources/Lua";

        /// <summary>
        /// 二进制文件;
        /// </summary>
        public static string binPath = "Assets/Resources/Bin";

        /// <summary>
        /// 需要打包的Atlas文件;
        /// </summary>
        public static string atlasPath = "Assets/Atlas";

        /// <summary>
        /// 需要打包的Scene文件;
        /// </summary>
        public static string scenePath = "Assets/Scenes/";

        /// <summary>
        /// 该路径下的资源单独打包,主要是为了方便使用资源,如图集,字体,场景大的背景贴图等等;
        /// </summary>
        public static string singleResPath = "Assets/Bundles/Single/";

        /// <summary>
        /// 获取AssetBundle文件的名字;
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>AssetBundle资源名字</returns>
        public static string GetAssetBundleFileName(AssetType type, string assetName)
        {
            string assetBundleName = null;

            if (type == AssetType.Non || string.IsNullOrEmpty(assetName)) return assetBundleName;
            //AssetBundle的名字不支持大写;
            //AssetBundle打包命名方式为[assetType/assetName.assetbundle],每个文件夹下的资源都带有相同的前缀,不同文件夹下,资源前缀不同;
            assetBundleName = (type.ToString() + "/" + assetName + ".assetbundle").ToLower();
            return assetBundleName;
        }

        /// <summary>
        /// 获取AssetBundle文件加载路径;
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>AssetBundle资源路径</returns>
        public static string GetAssetBundlePath(AssetType type, string assetName)
        {
            string assetBundleName = GetAssetBundleFileName(type, assetName);
            if (string.IsNullOrEmpty(assetBundleName)) return null;
            return assetBundlePath + "/" + assetBundleName;
        }

        /// <summary>
        /// 获取Resource文件加载路径;
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>Resource资源路径;</returns>
        public static string GetResourcePath(AssetType type, string assetName)
        {
            if (type == AssetType.Non || type == AssetType.Scripts || string.IsNullOrEmpty(assetName)) return null;
            string assetPath = null;
            switch (type)
            {
                case AssetType.Prefab: assetPath = "Prefab/"; break;
                default:
                    assetPath = type.ToString() + "/";
                    break;
            }
            assetPath = assetPath + assetName;
            return assetPath;
        }
    }
}
