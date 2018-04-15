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

public class Main : MonoBehaviour
{
    void Awake()
    {
        StartGame();
    }

    private void StartGame()
    {
        GameMgr.Instance.InitMgr();
    }
}
