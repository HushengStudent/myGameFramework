/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/02/05 16:50:41
** desc:  单个模型组件;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class CommonModelComponent : ModelComponent
    {
        private readonly string _skeletonPath = "Prefab/Models/Common/Skeleton.prefab";
        private GameObject _skeleton;
        private AssetBundleAssetProxy _skeletonProxy;

        protected override void InitializeEx()
        {
            base.InitializeEx();
            _skeletonProxy = ResourceMgr.singleton.LoadAssetAsync(_skeletonPath);
            _skeletonProxy.AddLoadFinishCallBack(() =>
            {
                _skeleton = _skeletonProxy.GetInstantiateObject<GameObject>();
                OnLoadFinish();
            });
        }

        protected override void OnLoadFinishEx()
        {
            base.OnLoadFinishEx();
            GameObject = _skeleton;
        }

        protected override void UnInitializeEx()
        {
            base.UnInitializeEx();
            if (_skeleton)
            {
                _skeletonProxy.ReleaseInstantiateObject(_skeleton);
            }
            _skeleton = null;
            _skeletonProxy.UnloadProxy();
        }
    }
}