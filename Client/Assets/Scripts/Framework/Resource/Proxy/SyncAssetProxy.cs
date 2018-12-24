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
            T t = null;
            if (TargetObject != null)
            {
                if (IsUsePool)
                {
                    t = PoolMgr.Instance.GetUnityObject(TargetObject) as T;
                }
                else
                {
                    t = Object.Instantiate(TargetObject) as T;
                }
            }
            return t;
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
                    ResourceMgr.Instance.UnloadUnityAsset(assetType, t);
                }
            }
        }

        public override T LoadUnitySharedAsset<T>()
        {
            T t = null;
            if (TargetObject != null)
            {
                if (IsUsePool)
                {
                    t = PoolMgr.Instance.GetUnityAsset(assetType, assetName) as T;
                }
                else
                {
                    t = TargetObject as T;
                }
            }
            return t;
        }

        public override void DestroyUnitySharedAsset<T>(T t)
        {
            if (t != null)
            {
                if (IsUsePool)
                {
                    PoolMgr.Instance.ReleaseUnityAsset(assetType, assetName, t, IsUsePool);
                }
                else
                {
                    ResourceMgr.Instance.UnloadUnityAsset(assetType, t);
                }
            }
        }
    }
}
