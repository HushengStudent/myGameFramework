/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/24 00:39:54
** desc:  行为树事件;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public delegate void OnBehaviorTreeStartHandler();
    public delegate void OnBehaviorTreeSuccesstHandler();
    public delegate void OnBehaviorTreeFailureHandler();
    public delegate void OnBehaviorTreeResetHandler();

    public delegate void OnBTNodeStartHandler();
    public delegate void OnBTNodeSuccesstHandler();
    public delegate void OnBTNodeFailureHandler();
    public delegate void OnBTNodeResetHandler();
}