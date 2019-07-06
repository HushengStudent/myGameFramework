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
        public static string VersionFilePath = Application.dataPath.ToLower() + "/../Config/Version.xml";
        public static string NetVersionFilePath = Application.dataPath.ToLower() + "/../Config/temp/Version.xml";

        public static int GameFrame = 60;
        public static float SyncInterval = 0.2f;
        public static MobileLevel MobileLevelValue = MobileLevel.High;

        public static string applicationDataPath = Application.dataPath.ToLower();
    
        public static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        public static int port = 10000;
    }
}
