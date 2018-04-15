/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/18 00:30:07
** desc:  游戏配置
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public static class GameConfig
	{
        public static int GameFrame = 60;
        public static float SyncInterval = 0.2f;
        public static MobileLevel MobileLevelValue = MobileLevel.High;

        public static string applicationDataPath =Application.dataPath.ToLower();
    }
}
