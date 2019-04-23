/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/26 00:03:35
** desc:  游戏入口;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Protocol;

/// <summary>
/// 配置环境变量;
/// myGameFramework:(工程路径)如C:/Users/husheng/Desktop/MyProject/4GameFramework/myGameFramework/
/// myGameFramework_protoc:(protoc.exe路径)如c:/protobuf-3.0.0/src/
/// </summary>
public class Main : MonoBehaviour
{
    void Awake()
    {
        StartGame();
    }

    /// <summary>
    /// 初始化游戏;
    /// </summary>
    private void StartGame()
    {
        GameMgr.Instance.Init();

    }
}
