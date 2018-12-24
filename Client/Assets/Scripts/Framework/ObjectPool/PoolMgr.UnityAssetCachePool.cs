/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  Unity Assetª∫¥Ê≥ÿ;
*********************************************************************************/

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class PoolMgr
    {
        /// <summary>
        /// Unity Asset Pool;
        /// </summary>
        private UnityAssetCachePool _unityAssetCachePool = new UnityAssetCachePool();

        /// <summary>
        /// ªÒ»°Unity Asset;
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Object GetUnityAsset(AssetType assetType, string assetName)
        {
            return _unityAssetCachePool.GetUnityAsset(assetType, assetName);
        }

        /// <summary>
        /// ÷¸¥ÊUnity Asset;
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetName"></param>
        /// <param name="obj"></param>
        public void ReleaseUnityAsset(AssetType assetType, string assetName, Object obj, bool isUsePool = true)
        {
            _unityAssetCachePool.ReleaseUnityAsset(assetType, assetName, obj, isUsePool);
        }
    }
}
