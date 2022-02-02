/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/21 02:20:27
** desc:  AssetBundle打包定义;
*********************************************************************************/

using UnityEditor;

namespace Framework.EditorModule.AssetBundle
{
    public static class AssetBuildDefine
    {
        /// <summary>
        /// 打包选项,打包AssetBundle不压缩,使用第三方压缩软件压缩,再解压到沙盒路径,既可以减少包体,加快读取速度,但是占物理磁盘空间;
        /// CompleteAssets默认开启;CollectDependencies默认开启;DeterministicAssetBundle默认开启;ChunkBasedCompression使用LZ4压缩;
        /// </summary>
        public static BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;

        /// <summary>
        /// AssetBundle打包目标平台;
        /// </summary>
        public static BuildTarget buildTarget =

#if UNITY_IOS    //unity5.x UNITY_IPHONE换成UNITY_IOS
	BuildTarget.iOS;
#elif UNITY_ANDROID
    BuildTarget.Android;
#else
    EditorUserBuildSettings.activeBuildTarget;
#endif

    }
}
