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
	public class GameMgr : Singleton<GameMgr> 
	{
		public void InitMgr()
        {
            //==========Singleton==========
            ResourceMgr.Instance.InitMgr();      //资源初始化;
            PoolMgr.Instance.InitMgr();          //对象池初始化;

            //==========MonoSingleton======
            LuaMgr.Instance.InitMgr();           //Lua初始化;
        }
    }
}
