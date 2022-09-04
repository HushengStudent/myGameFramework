/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  Csharp对象池管理;
*********************************************************************************/

using System;
using System.Collections.Generic;

namespace Framework.ObjectPoolModule
{
    public partial class PoolMgr
    {
        /// <summary>
        /// Csharp Object Pool;
        /// </summary>
        private readonly Dictionary<Type, object> _csharpObjectPool = new Dictionary<Type, object>();

        /// <summary>
        /// 获取Csharp对象池目标组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
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
            var value = pool.Get();
            if (value is IPool target)
            {
                target.OnGet(args);
            }
            return value;
        }

        /// <summary>
        /// 释放Csharp对象池组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void ReleaseCsharpObject<T>(T value) where T : new()
        {
            if (value is IPool target)
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
            pool.Release(value);
        }

        /// <summary>
        /// 创建Csharp对象池;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
