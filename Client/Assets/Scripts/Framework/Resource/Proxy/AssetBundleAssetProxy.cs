/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 21:07:40
** desc:  AssetBundle资源加载代理;
*********************************************************************************/

using Framework.AssetBundleModule;
using Framework.ObjectPoolModule;
using UnityObject = UnityEngine.Object;

namespace Framework.ResourceModule
{
    public class AssetBundleAssetProxy : AbsAssetProxy
    {
        protected override void Unload()
        {
            AssetBundleMgr.singleton.UnloadAsset(AssetPath, AssetObject);
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