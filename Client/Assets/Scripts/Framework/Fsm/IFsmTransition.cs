/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 20:03:32
** desc:  状态机转换接口;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IFsmTransition
    {
        IFsmState FromState { get; set; }
        IFsmState ToState { get; set; }
        string Name { get; }
        void OnTrans();
        bool CanTrans();
    }
}
