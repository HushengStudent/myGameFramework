/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:15:17
** desc:  状态机管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Unity状态机实现更简单;
    /// </summary>
    public class FsmMgr : Singleton<FsmMgr>
    {
        private bool[,] _fsmTransition = new bool[,] {
            {false, false},
            {false, false}
        };

        public bool[,] FsmTransition { get { return _fsmTransition; } }
    }
}
