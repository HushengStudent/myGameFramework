/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源加载代理;
*********************************************************************************/

using Framework.ObjectPool;
using UnityObject = UnityEngine.Object;

namespace Framework
{
    public class ResourceAssetProxy : AbsAssetProxy
    {
        protected override void Unload()
        {
            ResourceMgr.singleton.DestroyUnityAsset(AssetObject);
            PoolMgr.singleton.ReleaseCsharpObject(this);
        }

        protected override T GetInstantiateObjectEx<T>()
        {
            if (AssetObject == null || !CanInstantiate())
            {
                return null;
            }
            return UnityObject.Instantiate(AssetObject) as T;
        }

        public override void ReleaseInstantiateObject<T>(T t)
        {
            if (t != null)
            {
                ResourceMgr.singleton.DestroyInstantiateObject(t);
            }
        }

        protected override T GetUnityAssetEx<T>()
        {
            if (AssetObject == null || CanInstantiate())
            {
                return null;
            }
            return AssetObject as T;
        }
    }
}