/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源加载代理;
*********************************************************************************/

using Object = UnityEngine.Object;

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
            ResourceMgr.Instance.DestroyUnityAsset(AssetObject);
            PoolMgr.Instance.ReleaseCsharpObject(this);
        }

        public override T GetInstantiateObject<T>()
        {
            T t = null;
            if (AssetObject != null && CanInstantiate())
            {
                t = Object.Instantiate(AssetObject) as T;
            }
            return t;
        }

        public override void ReleaseInstantiateObject<T>(T t)
        {
            if (t != null)
            {
                ResourceMgr.Instance.DestroyInstantiateObject(t);
            }
        }

        public override T GetUnityAsset<T>()
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
