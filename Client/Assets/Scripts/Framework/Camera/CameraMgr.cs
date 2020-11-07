/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/24 01:45:07
** desc:  相机管理;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class CameraMgr : MonoSingleton<CameraMgr>
    {
        private readonly string _uiRoot = "Prefab/UI/Common/UIRoot.prefab";
        private readonly string _mainCamera = "Prefab/Common/MainCamera.prefab";
        private AssetBundleAssetProxy uiRootProxy;
        private AssetBundleAssetProxy mainCameraProxy;

        public Camera MainCamera { get; private set; }
        public Camera MainUICamera { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            uiRootProxy = ResourceMgr.singleton.LoadAssetAsync(_uiRoot);
            uiRootProxy.AddLoadFinishCallBack(() =>
            {
                var go = uiRootProxy.GetInstantiateObject<GameObject>();
                go.transform.localPosition = Vector3.zero;
                MainUICamera = go.transform.Find("MainUICamera").GetComponent<Camera>();
                DontDestroyOnLoad(go);
                OnInitFinish();
            });
            mainCameraProxy = ResourceMgr.singleton.LoadAssetAsync(_mainCamera);
            mainCameraProxy.AddLoadFinishCallBack(() =>
            {
                var go = mainCameraProxy.GetInstantiateObject<GameObject>();
                go.transform.localPosition = Vector3.zero;
                MainCamera = go.transform.GetComponent<Camera>();
                DontDestroyOnLoad(go);
                OnInitFinish();
            });
        }

        private void OnInitFinish()
        {
            if (uiRootProxy.IsFinish && mainCameraProxy.IsFinish)
            {
                EventMgr.singleton.FireGlobalEvent(EventType.CAMERA_MGR_INIT, null);
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            if (MainCamera)
            {

            }
        }

        protected override void OnDestroyEx()
        {
            base.OnDestroyEx();
            if (uiRootProxy != null)
            {
                uiRootProxy.UnloadProxy();
            }
            if (mainCameraProxy != null)
            {
                mainCameraProxy.UnloadProxy();
            }
        }
    }
}