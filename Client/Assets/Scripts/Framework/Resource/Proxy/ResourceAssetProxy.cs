/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源加载代理;
*********************************************************************************/

using UnityObject = UnityEngine.Object;

namespace Framework
{
    public class ResourceAssetProxy : AbsAssetProxy
    {
        public void Initialize(string path)
        {
            Initialize(path, false);
        }

        protected override void Unload()
        {
            ResourceMgr.Singleton.DestroyUnityAsset(AssetObject);
            PoolMgr.Singleton.ReleaseCsharpObject(this);
        }

        protected override T GetInstantiateObjectEx<T>()
        {
            T t = null;
            if (AssetObject != null && CanInstantiate())
            {
                t = UnityObject.Instantiate(AssetObject) as T;
            }
            return t;
        }

        public override void ReleaseInstantiateObject<T>(T t)
        {
            if (t != null)
            {
                ResourceMgr.Singleton.DestroyInstantiateObject(t);
            }
        }

        protected override T GetUnityAssetEx<T>()
        {
            T t = null;
            if (AssetObject != null && !CanInstantiate())
            {
                t = AssetObject as T;
            }
            return t;
        }
    }
}
