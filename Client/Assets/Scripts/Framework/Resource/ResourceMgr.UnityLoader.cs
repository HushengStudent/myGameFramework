/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 17:34:50
** desc:  资源加载器,不参与资源管理;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MEC;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class ResourceMgr
    {
        #region Unload Assets

        public void UnloadUnityAssetMemory(AssetType assetType, Object asset)
        {
            if (asset != null)
            {
                if (assetType == AssetType.Prefab)
                {
                    return;
                }
                if (assetType == AssetType.AnimeClip || assetType == AssetType.AnimeCtrl
                        || assetType == AssetType.Audio || assetType == AssetType.Texture
                        || assetType == AssetType.Material)
                {
                    /// 卸载不需实例化的资源:纹理,animator,clip,material;
                    /// 卸载非GameObject类型的资源,会将内存中已加载资源及其克隆体卸载:前提是已经没有任何引用持有该资源,可以置null再卸载;
                    /// Unload Assets may only be used on individual assets and can not be used on GameObject's/Components or AssetBundles;
                    Resources.UnloadAsset(asset);
                    return;
                }
                LogHelper.PrintError(string.Format("[ResourceMgr]UnloadUnityObject error,AssetType:{0},Object:{1}"
                    , assetType.ToString(), asset.name));
            }
        }

        public void UnloadUnitySceneMemory(AssetType assetType, Object asset)
        {
            if (asset != null)
            {
                if (assetType == AssetType.Prefab)
                {
                    //资源是GameObject;
                    GameObject go = asset as GameObject;
                    if (go)
                    {
                        Destroy(go);
                        return;
                    }
                    //泛型加载,资源是Prefab上的MonoBehaviour脚本;
                    MonoBehaviour monoBehaviour = (MonoBehaviour)asset;
                    if (monoBehaviour != null)
                    {
                        Destroy(monoBehaviour.gameObject);
                        return;
                    }
                }
                if (assetType == AssetType.AnimeClip || assetType == AssetType.AnimeCtrl
                    || assetType == AssetType.Audio || assetType == AssetType.Texture
                    || assetType == AssetType.Material)
                {
                    Destroy(asset);
                    return;
                }
                LogHelper.PrintError(string.Format("[ResourceMgr]UnloadUnityObject error,AssetType:{0},Object:{1}"
                    , assetType.ToString(), asset.name));
            }
        }

        /// <summary>
        /// 手动GC;
        /// </summary>
        public void GameGC()
        {
            System.GC.Collect();
        }

        /// <summary>
        /// 清理;
        /// </summary>
        public void UnloadUnusedAssets(Action<AsyncOperation> action)
        {
            AsyncOperation operation = Resources.UnloadUnusedAssets();
            operation.completed += action;
        }

        #endregion
    }
}