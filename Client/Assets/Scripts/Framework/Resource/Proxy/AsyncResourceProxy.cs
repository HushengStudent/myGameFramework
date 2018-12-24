/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源异步加载代理;
*********************************************************************************/

using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class AsyncResourceProxy : AssetProxy
    {
        public void InitProxy(AssetType assetType, string assetName, string assetBundlePath)
        {
            base.InitProxy(assetType, assetName, false);
        }

        protected override void Unload()
        {
            ResourceMgr.Instance.UnloadUnityAssetMemory(assetType, TargetObject);
            PoolMgr.Instance.ReleaseCsharpObject(this);
        }

        public override T LoadUnityObject<T>()
        {
            T t = null;
            if (TargetObject != null && IsInstantiate())
            {
                t = Object.Instantiate(TargetObject) as T;
            }
            return t;
        }

        public override void DestroyUnityObject<T>(T t)
        {
            if (t != null)
            {
                ResourceMgr.Instance.UnloadUnitySceneMemory(assetType, t);
            }
        }

        public override T LoadUnitySharedAsset<T>()
        {
            T t = null;
            if (TargetObject != null && !IsInstantiate())
            {
                t = TargetObject as T;
            }
            return t;
        }

        public override void DestroyUnitySharedAsset<T>(T t)
        {
            if (t != null)
            {
                ResourceMgr.Instance.UnloadUnityAssetMemory(assetType, t);
            }
        }
    }
}
