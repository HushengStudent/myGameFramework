/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/15 18:24:17
** desc:  事件参数接口;
*********************************************************************************/

using Framework.ObjectPoolModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IEventArgs : IPool
    {
        IEventArgs Clone();
    }
}
