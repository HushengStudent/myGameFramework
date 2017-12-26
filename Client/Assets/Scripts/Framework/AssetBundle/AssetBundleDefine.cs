/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 23:42:52
** desc:  AssetBundle打包定义
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public static class AssetBundleDefine
    {
        //AssetBundle打包存储路径;
        private static string assetBundlePath = Application.dataPath + "/StreamingAssets/AssetBundle";
        public static string AssetBundlePath
        {
            get { return assetBundlePath; }
        }
		
	}
}
