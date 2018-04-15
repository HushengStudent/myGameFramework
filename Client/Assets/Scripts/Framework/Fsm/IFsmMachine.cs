/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 20:02:33
** desc:  状态机接口;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IFsmMachine
    {
        IFsmState CurrentState { get; }
        IFsmState DefaultState { get; set; }
        bool AddState(IFsmState state);
        bool RemoveState(IFsmState state);
        IFsmState GetState(IFsmState state);
        bool AddTransition(IFsmTransition trans);
        bool RemoveTransition(IFsmTransition trans);
        IFsmTransition GetTransition(IFsmTransition trans);
    }
}
