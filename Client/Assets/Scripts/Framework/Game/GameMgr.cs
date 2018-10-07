/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:31:02
** desc:  游戏管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
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

        public int GameFrame { get { return _gameFrame; } set { _gameFrame = value; } }
        public float SyncInterval { get { return _syncInterval; } set { _syncInterval = value; } }
        public MobileLevel MobileLevelValue { get { return _mobileLevelValue; } set { _mobileLevelValue = value; } }

        public override void Init()
        {
            base.Init();
            PoolMgr.Instance.ClearFinishHandler = () => InitApp();
            PoolMgr.Instance.Init();          //对象池初始化;
        }

        private void InitApp()
        {
            PoolMgr.Instance.ClearFinishHandler = null;
            SetGameConfig();
#if UNITY_EDITOR
            DebugMgr.Instance.Init();         //Debug工具初始化;
#endif
            EventMgr.Instance.Init();         //事件系统初始化;
            UIEventMgr<int>.Init();           //UI事件系统初始化;
            SdkMgr.Instance.Init();           //平台初始化;
            TimerMgr.Instance.Init();         //定时器初始化;
            ComponentMgr.Instance.Init();     //组件初始化;
            EntityMgr.Instance.Init();        //实体初始化;
            ResourceMgr.Instance.Init();      //资源初始化;
            UIMgr.Instance.Init();            //UI初始化;
            SceneMgr.Instance.Init();         //场景初始化;
            LuaMgr.Instance.Init();           //lua初始化;
            MemoryMgr.Instance.Init();
            NetMgr.Instance.Init();           //网络初始化;
        }

        public void CheckUpdate()
        {
            UpdateMgr.Instance.Init();
        }

        public void EnterGame()
        {

        }

        /// <summary>
        /// 设置游戏配置;
        /// </summary>
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
            LogUtil.LogUtility.LogEnable = false;
#if UNITY_EDITOR
            LogUtil.LogUtility.LogEnable = true;
#endif
            LogUtil.LogUtility.WarningEnable = true;
            LogUtil.LogUtility.ErrorEnable = true;
        }
    }
}
