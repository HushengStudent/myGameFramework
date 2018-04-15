/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:31:02
** desc:  游戏管理
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

        public void InitMgr()
        {
            SetGameConfig();

            //==========Singleton==========
            ResourceMgr.Instance.InitMgr();      //资源初始化;
            PoolMgr.Instance.InitMgr();          //对象池初始化;

            //==========MonoSingleton======
            LuaMgr.Instance.InitMgr();           //Lua初始化;
        }

        /// <summary>
        /// 设置游戏配置;
        /// </summary>
        private void SetGameConfig()
        {
            SetGameFrame(GameConfig.GameFrame);
            SetMobileLevel(GameConfig.MobileLevelValue);
            SetSyncInterval(GameConfig.SyncInterval);
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
    }
}
