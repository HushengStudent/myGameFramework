/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:15:17
** desc:  状态机管理
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FsmMgr : Singleton<BuffMgr>
    {
        public override void Init()
        {
            base.Init();
            LogUtil.LogUtility.Print("FsmMgr Init!", LogUtil.LogColor.Green);
        }
    }
}
