/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 00:03:35
** desc:  游戏入口
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Protocol;

namespace Common
{
    public class Main : MonoBehaviour
    {
        void Awake()
        {
            StartGame();
        }

        private void StartGame()
        {
            //==========Singleton==========
            ResourceMgr.Instance.Init();      //资源初始化;
            AssetBundleMgr.Instance.Init();   //...
            BuffMgr.Instance.Init();          //...
            FsmMgr.Instance.Init();           //...
            SkillMgr.Instance.Init();         //...

            //==========MonoSingleton==========
            LuaMgr.Instance.InitMgr();           //Lua初始化;
            CoroutineMgr.Instance.Init();     //...
            TimerMgr.Instance.Init();         //...
            //ECS
            ComponentMgr.Instance.Init();     //...
            EntityMgr.Instance.Init();        //...
            EventMgr.Instance.Init();

        }
    }
}
