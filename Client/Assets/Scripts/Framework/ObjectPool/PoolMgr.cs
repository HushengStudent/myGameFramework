/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 18:14:29
** desc:  对象池管理
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PoolMgr : Singleton<PoolMgr>
    {
        public T Get<T>(T type,params Object[] args) where T : IPool, new()
        {
            T t = new T();
            t.Init(args);
            return t;
        }

        public void Release<T>(T type) where T : IPool, new()
        {
            type.Release();
            CommonRelease<T>(type);
        }

        private void CommonRelease<T>(T type) where T : IPool, new()
        {
            //TODO:position...
        }
    }
}
