/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/20 00:13:50
** desc:  对象池组件接口
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IPool
    {
        protected virtual void Init(params Object[] args);
        protected virtual void Release();
    }
}
