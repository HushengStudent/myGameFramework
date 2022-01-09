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
        public static string StreamingAssetsPath
        {
            get
            {
#if UNITY_EDITOR
                return $"{Application.dataPath}/StreamingAssets";
#else
                return Application.streamingAssetsPath;
#endif
            }
        }
    }
}
