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
        /// List Pool;
        /// </summary>
        private Dictionary<Type, Object> _listPool = new Dictionary<Type, Object>();

        /// <summary>
        /// 获取对象池目标组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">初始化参数</param>
        /// <returns></returns>
        public List<T> GetList<T>()
        {
            ListPool<T> pool;
            Object temp;
            if (_listPool.TryGetValue(typeof(T), out temp))
            {
                pool = temp as ListPool<T>;
            }
            else
            {
                pool = CreateListPool<T>();
            }
            List<T> t = pool.Get();
            return t;
        }

        /// <summary>
        /// 释放对象池组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void ReleaseList<T>(List<T> list)
        {
            ListPool<T> pool;
            Object temp;
            if (_listPool.TryGetValue(typeof(T), out temp))
            {
                pool = temp as ListPool<T>;
            }
            else
            {
                pool = CreateListPool<T>();
            }
            pool.Release(list);
        }

        /// <summary>
        /// 创建对象池;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ListPool<T> CreateListPool<T>()
        {
            Object temp;
            if (_listPool.TryGetValue(typeof(T), out temp))
            {
                return temp as ListPool<T>;
            }
            else
            {
                ListPool<T> pool = new ListPool<T>();
                _listPool[typeof(T)] = pool;
                return pool;
            }
        }
    }
}
