/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/22 23:53:32
** desc:  事件类型;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum EventType : int
    {
        INIT_GAME_EVENT      = 1,   //初始化游戏;

        ON_SCENE_UNLOAD      = 2,   //卸载场景;
        ON_SCENE_LOADED      = 3,   //加载场景;
    }
}
