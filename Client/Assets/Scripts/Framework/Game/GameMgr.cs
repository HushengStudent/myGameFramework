/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:31:02
** desc:  游戏管理;
*********************************************************************************/

using UnityEngine;

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
        private int _gameFrame = 60;
        private float _syncInterval = 0.2f;
        private MobileLevel _mobileLevelValue = MobileLevel.High;

        public bool CheckUpdateState = false;

        public int GameFrame { get { return _gameFrame; } set { _gameFrame = value; } }
        public float SyncInterval { get { return _syncInterval; } set { _syncInterval = value; } }
        public MobileLevel MobileLevelValue { get { return _mobileLevelValue; } set { _mobileLevelValue = value; } }

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

            ResourceMgr.onResourceInitAction = () =>
            {
                ResourceMgr.onResourceInitAction = null;
                InitPoolMgr();
            };
            ResourceMgr.Instance.Launch();      //资源初始化;
        }

        private void InitPoolMgr()
        {
            PoolMgr.onPoolInitAction = () =>
            {
                PoolMgr.onPoolInitAction = null;
                InitApp();
            };
            PoolMgr.Instance.Launch();          //对象池初始化;
        }

        private void InitApp()
        {
            SetGameConfig();
#if UNITY_EDITOR
            //DebugMgr.Instance.Init();         //Debug工具初始化;
#endif
            EventMgr.Instance.Launch();         //事件系统初始化;
            UIEventMgr<int>.Init();             //UI事件系统初始化;
            SdkMgr.Instance.Launch();           //平台初始化;
            TimerMgr.Instance.Launch();         //定时器初始化;
            ComponentMgr.Instance.Launch();     //组件初始化;
            EntityMgr.Instance.Launch();        //实体初始化;
            UIMgr.Instance.Launch();            //UI初始化;
            SceneMgr.Instance.Launch();         //场景初始化;
            LuaMgr.Instance.Launch();           //lua初始化;
            MemoryMgr.Instance.Launch();
            NetMgr.Instance.Launch();           //网络初始化;


            //EnterGame();

        }

        public void CheckUpdate()
        {
            UpdateMgr.Instance.Launch();
        }

        public void EnterGame()
        {
            ResourceMgr.Instance.LoadSceneAsync("Assets/Bundles/Scene/Level01.unity");
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
