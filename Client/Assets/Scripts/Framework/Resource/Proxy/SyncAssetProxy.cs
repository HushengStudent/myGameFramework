/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 21:07:40
** desc:  AssetBundle资源同步加载代理;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class SyncAssetProxy : AssetProxy
    {
        protected override void Unload()
        {
            PoolMgr.Instance.ReleaseUnityAsset(assetType, assetName, TargetObject, IsUsePool);
            PoolMgr.Instance.ReleaseCsharpObject(this);
        }

        public override T LoadUnityObject<T>()
        {
            if (TargetObject != null && IsInstantiate())
            {
                return PoolMgr.Instance.GetUnityObject(TargetObject) as T;
            }
            return null;
        }

        public override void DestroyUnityObject<T>(T t)
        {
            if (t != null)
            {
                if (IsUsePool)
                {
                    PoolMgr.Instance.ReleaseUnityObject(t);
                }
                else
                {
                    ResourceMgr.Instance.UnloadUnitySceneMemory(assetType, t);
                }
            }
        }

        public override T LoadUnitySharedAsset<T>()
        {
            if (TargetObject != null && !IsInstantiate())
            {
                return PoolMgr.Instance.GetUnityAsset(assetType, assetName) as T;
            }
            return null;
        }

        public override void DestroyUnitySharedAsset<T>(T t)
        {
            if (t != null)
            {
                PoolMgr.Instance.ReleaseUnityAsset(assetType, assetName, t, IsUsePool);
            }
        }
    }
}
