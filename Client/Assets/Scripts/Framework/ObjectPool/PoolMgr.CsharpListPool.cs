/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:49:28
** desc:  List对象池;
*********************************************************************************/

using System;
using System.Collections.Generic;
using Framework.ObjectPool;

namespace Framework
{
    public partial class PoolMgr
    {
        /// <summary>
        /// Csharp List Pool;
        /// </summary>
        private Dictionary<Type, Object> _csharpListPool = new Dictionary<Type, Object>();

        /// <summary>
        /// 获取对象池目标组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">初始化参数</param>
        /// <returns></returns>
        public List<T> GetCsharpList<T>()
        {
            CsharpListPool<T> pool;
            if (_csharpListPool.TryGetValue(typeof(T), out var temp))
            {
                pool = temp as CsharpListPool<T>;
            }
            else
            {
                pool = CreateCsharpListPool<T>();
            }
            return pool.Get();
        }

        /// <summary>
        /// 释放对象池组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void ReleaseCsharpList<T>(List<T> list)
        {
            CsharpListPool<T> pool;
            if (_csharpListPool.TryGetValue(typeof(T), out var temp))
            {
                pool = temp as CsharpListPool<T>;
            }
            else
            {
                pool = CreateCsharpListPool<T>();
            }
            pool.Release(list);
        }

        /// 创建对象池;
        private CsharpListPool<T> CreateCsharpListPool<T>()
        {
            if (_csharpListPool.TryGetValue(typeof(T), out var temp))
            {
                return temp as CsharpListPool<T>;
            }
            else
            {
                var pool = new CsharpListPool<T>();
                _csharpListPool[typeof(T)] = pool;
                return pool;
            }
        }
    }
}
