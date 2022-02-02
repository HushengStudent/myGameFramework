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
#if UNITY_STANDALONE
        public static string PlatformName = "Windows";
#elif UNITY_ANDROID
        public static string PlatformName = "Android";
#elif UNITY_IPHONE
        public static string PlatformName = "IOS";    
#else
        public static string PlatformName = "Default";
#endif

        public static string AssetBundlePath
        {
            get
            {
                return $"{Application.dataPath}/../AssetBundle/{PlatformName}/AssetBundle";
            }
        }

        public static string StreamingAssetsPath
        {
            get
            {
                return $"{Application.streamingAssetsPath}/{PlatformName}";
            }
        }

        public static string PersistentDataPath
        {
            get
            {
                return $"{Application.persistentDataPath}/{PlatformName}";
            }
        }
    }
}
