/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 00:03:35
** desc:  ÓÎÏ·Èë¿Ú
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

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
            LuaMgr.Instance.StartLuaMgr();
            LogUtility.Print("test", LogColor.Non);
            LogUtility.Print("test", LogColor.Red);
            LogUtility.Print("test", LogColor.Green);
            LogUtility.Print("test", LogColor.Yellow);
        }
    }
}
