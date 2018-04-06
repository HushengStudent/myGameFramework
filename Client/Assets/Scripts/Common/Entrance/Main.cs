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
            ResourceMgr.Instance.InitMgr();      //资源初始化;
            
            //==========MonoSingleton==========
            LuaMgr.Instance.InitMgr();           //Lua初始化;

        }
    }
}
