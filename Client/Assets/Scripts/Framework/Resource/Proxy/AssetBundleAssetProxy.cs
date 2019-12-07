/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 21:07:40
** desc:  AssetBundle��Դ���ش���;
*********************************************************************************/

namespace Framework
{
    public class AssetBundleAssetProxy : AbsAssetProxy
    {
        private bool storage;

        public new void Initialize(string path, bool isUsePool)
        {
            base.Initialize(path, isUsePool);
            storage = false;
        }

        protected override void Unload()
        {
            /// ���ʹ�ö������,˵����Ҫ����,����AssetBundleMgr�Զ�ж��;
            if (!storage)
            {
                AssetBundleMgr.singleton.UnloadAsset(AssetPath, AssetObject);
            }
            PoolMgr.singleton.ReleaseCsharpObject(this);
        }

        protected override T GetInstantiateObjectEx<T>()
        {
            if (AssetObject != null && CanInstantiate())
            {
                return PoolMgr.singleton.GetUnityObject(AssetObject) as T;
            }
            return null;
        }

        public override void ReleaseInstantiateObject<T>(T t)
        {
            if (t != null)
            {
                if (IsUsePool)
                {
                    PoolMgr.singleton.ReleaseUnityObject(t);
                    storage = true;
                }
                else
                {
                    ResourceMgr.singleton.DestroyInstantiateObject(t);
                }
            }
        }

        protected override T GetUnityAssetEx<T>()
        {
            if (AssetObject != null && !CanInstantiate())
            {
                return AssetObject as T;
            }
            return null;
        }

        protected override void OnReleaseEx()
        {
            base.OnReleaseEx();
            storage = false;
        }
    }
}
