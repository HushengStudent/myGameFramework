/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 18:14:29
** desc:  对象池管理
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class PoolMgr : Singleton<PoolMgr>
    {
        private Dictionary<Type, Object> _pool = new Dictionary<Type, Object>();

        private Dictionary<int, UnityEngine.GameObject> _objectPool = new Dictionary<int, UnityEngine.GameObject>();

        /// <summary>
        /// 获取对象池目标组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">初始化参数</param>
        /// <returns></returns>
        public T Get<T>(params Object[] args) where T : IPool, new()
        {
            ObjectPool<T> pool;
            Object temp;
            if (_pool.TryGetValue(typeof(T), out temp))
            {
                pool = temp as ObjectPool<T>;
            }
            else
            {
                pool = CreatePool<T>();
            }
            T t = pool.Get();
            t.OnInit(args);
            return t;
        }

        /// <summary>
        /// 释放对象池组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        public void Release<T>(T type) where T : IPool, new()
        {
            type.OnRelease();
            //UnityEngine.GameObject go = type.OnAddGameObject();
            //if (go)
            //{
            //    _objectPool.Add(_objectPool.Count, go);
            //}
            ObjectPool<T> pool;
            Object temp;
            if (_pool.TryGetValue(typeof(T), out temp))
            {
                pool = temp as ObjectPool<T>;
            }
            else
            {
                pool = CreatePool<T>();
            }
            pool.Release(type);
        }

        /// <summary>
        /// 销毁对象池;
        /// </summary>
        public void DestroyPool()
        {
            //for (int i = 0; i < _objectPool.Count; i++)
            //{
            //    if (_objectPool[i])
            //    {
            //        UnityEngine.GameObject.Destroy(_objectPool[i]);//销毁对象附着GameObject,对象GC回收;
            //    }
            //}
            _objectPool.Clear();
            _pool.Clear();
        }

        /// <summary>
        /// 创建对象池;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ObjectPool<T> CreatePool<T>() where T : new()
        {
            Object temp;
            if (_pool.TryGetValue(typeof(T), out temp))
            {
                return temp as ObjectPool<T>;
            }
            else
            {
                ObjectPool<T> pool = new ObjectPool<T>(null, null);
                _pool[typeof(T)] = pool;
                return pool;
            }
        }
    }
}
