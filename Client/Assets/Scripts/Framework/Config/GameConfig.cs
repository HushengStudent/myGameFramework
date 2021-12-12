/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/18 00:30:07
** desc:  游戏配置;
*********************************************************************************/

using System.Net;
using UnityEngine;

namespace Framework
{
    public static class GameConfig
    {
        public static string LoaclAppVersionFilePath =
            $"{Application.persistentDataPath.ToLower()}/version/version.xml";

        public static string NetAppVersionFilePath =
            $"{Application.persistentDataPath.ToLower()}/version/temp/version.xml";

        public static string NetAppVersionUrl =
            $"http://127.0.0.1/version.xml";

        public static int GameFrame = 60;
        public static float SyncInterval = 0.2f;
        public static MobileLevel MobileLevelValue = MobileLevel.High;

        public static IPAddress IPAddress = IPAddress.Parse("127.0.0.1");
        public static int Port = 10000;
    }
}