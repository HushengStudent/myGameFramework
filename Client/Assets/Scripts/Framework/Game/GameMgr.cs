/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:31:02
** desc:  游戏管理;
*********************************************************************************/

using Framework.CameraModule;
using Framework.ECSModule;
using Framework.EventModule;
using Framework.ObjectPoolModule;
using Framework.ResourceModule;
using Framework.SceneModule;
using Framework.UIModule;
using UnityEngine;
using EventType = Framework.EventModule.EventType;

namespace Framework
{
    public enum MobileLevel : int
    {
        Low = 1,
        Middle,
        High
    }

    public class GameMgr : Singleton<GameMgr>
    {
        public bool CheckUpdateState = false;

        public int GameFrame { get; set; } = 60;
        public float SyncInterval { get; set; } = 0.2f;
        public MobileLevel MobileLevelValue { get; set; } = MobileLevel.High;

        public bool IsEditor
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static bool _assetBundleModel;
        public static bool AssetBundleModel
        {
            get
            {
#if !UNITY_EDITOR
                return true;
#else
                return _assetBundleModel;
#endif
            }
            set
            {
#if UNITY_EDITOR
                _assetBundleModel = value;
#endif
            }
        }

        protected override void OnInitialize()
        {
            CheckUpdateState = false;

            EventMgr.singleton.Launch();         //事件系统初始化;
            EventMgr.singleton.AddGlobalEvent(EventType.RESOURCE_MGR_INIT, OnResourceInit);
            EventMgr.singleton.AddGlobalEvent(EventType.POOL_MGR_INIT, OnPoolInit);
            EventMgr.singleton.AddGlobalEvent(EventType.CAMERA_MGR_INIT, OnCameraInit);

            ResourceMgr.singleton.Launch();      //资源初始化;
        }

        private void OnResourceInit(IEventArgs args)
        {
            EventMgr.singleton.RemoveGlobalEvent(EventType.RESOURCE_MGR_INIT);
            PoolMgr.singleton.Launch();          //对象池初始化;
        }

        private void OnPoolInit(IEventArgs args)
        {
            EventMgr.singleton.RemoveGlobalEvent(EventType.POOL_MGR_INIT);
            CameraMgr.singleton.Launch();
        }

        private void OnCameraInit(IEventArgs args)
        {
            EventMgr.singleton.RemoveGlobalEvent(EventType.CAMERA_MGR_INIT);
            InitApp();
        }

        private void InitApp()
        {
            SetGameConfig();
#if UNITY_EDITOR
            //DebugMgr.Instance.Init();         //Debug工具初始化;
#endif
            //UIEventMgr<int>.Init();             //UI事件系统初始化;
            SdkMgr.singleton.Launch();           //平台初始化;
            TimerMgr.singleton.Launch();         //定时器初始化;
            ComponentMgr.singleton.Launch();     //组件初始化;
            EntityMgr.singleton.Launch();        //实体初始化;
            UIMgr.singleton.Launch();            //UI初始化;
            SceneMgr.singleton.Launch();         //场景初始化;
            LuaMgr.singleton.Launch();           //lua初始化;
            MemoryMgr.singleton.Launch();
            //NetMgr.Instance.Launch();           //网络初始化;

            EnterGame();

        }

        public void CheckUpdate()
        {
            UpdateMgr.singleton.Launch();
        }

        public void EnterGame()
        {
            ResourceMgr.singleton.LoadSceneAsync("Scene/Level01.unity");
            EntityMgr.singleton.CreateEntity<PlayerEntity>(1, 1, "_entity_test");
        }

        /// 设置游戏配置;
        private void SetGameConfig()
        {
            SetGameFrame(GameConfig.GameFrame);
            SetMobileLevel(GameConfig.MobileLevelValue);
            SetSyncInterval(GameConfig.SyncInterval);
            SetLogLevel();
        }

        /// <summary>
        /// 设置帧率;
        /// </summary>
        /// <param name="frame"></param>
        public void SetGameFrame(int frame)
        {
            Application.targetFrameRate = frame;
        }

        /// <summary>
        /// 设置同步间隔;
        /// </summary>
        /// <param name="interval"></param>
        public void SetSyncInterval(float interval)
        {
            SyncInterval = interval;
        }

        /// <summary>
        /// 设置手机级别;
        /// </summary>
        /// <param name="level"></param>
        public void SetMobileLevel(MobileLevel level)
        {
            MobileLevelValue = level;
        }

        public void SetLogLevel()
        {
            LogHelper.LogEnable = false;
#if UNITY_EDITOR
            LogHelper.LogEnable = true;
#endif
            LogHelper.WarningEnable = true;
            LogHelper.ErrorEnable = true;
        }
    }
}
