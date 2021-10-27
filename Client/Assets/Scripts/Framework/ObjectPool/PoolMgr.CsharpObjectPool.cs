/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  Csharp对象池管理;
*********************************************************************************/

using Framework.ObjectPool;
using System;
using System.Collections.Generic;

namespace Framework
{
    public partial class PoolMgr
    {
        /// <summary>
        /// Csharp Object Pool;
        /// </summary>
        private Dictionary<Type, object> _csharpObjectPool = new Dictionary<Type, object>();

        /// <summary>
        /// 获取Csharp对象池目标组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">初始化参数</param>
        /// <returns></returns>
        public T GetCsharpObject<T>(params object[] args) where T : new()
        {
            CsharpObjectPool<T> pool;
            if (_csharpObjectPool.TryGetValue(typeof(T), out var temp))
            {
                pool = temp as CsharpObjectPool<T>;
            }
            else
            {
                pool = CreateCsharpPool<T>();
            }
            var t = pool.Get();
            if (t is IPool target)
            {
                target.OnGet(args);
            }
            return t;
        }

        /// <summary>
        /// 释放Csharp对象池组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        public void ReleaseCsharpObject<T>(T type) where T : new()
        {
            if (type is IPool target)
            {
                target.OnRelease();
            }
            CsharpObjectPool<T> pool;
            if (_csharpObjectPool.TryGetValue(typeof(T), out var temp))
            {
                pool = temp as CsharpObjectPool<T>;
            }
            else
            {
                pool = CreateCsharpPool<T>();
            }
            pool.Release(type);
        }

        /// 创建Csharp对象池;
        private CsharpObjectPool<T> CreateCsharpPool<T>() where T : new()
        {
            if (_csharpObjectPool.TryGetValue(typeof(T), out var temp))
            {
                return temp as CsharpObjectPool<T>;
            }
            else
            {
                var pool = new CsharpObjectPool<T>(null, null);
                _csharpObjectPool[typeof(T)] = pool;
                return pool;
            }
        }
    }
}
