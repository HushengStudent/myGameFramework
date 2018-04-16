/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 18:14:29
** desc:  对象池管理;
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Framework
{
    public class PoolMgr : Singleton<PoolMgr>, IMgr
    {
        /// <summary>
        /// C# Object Pool;
        /// </summary>
        private Dictionary<Type, Object> _pool = new Dictionary<Type, Object>();

        /// <summary>
        /// GameObject Pool;
        /// </summary>
        private GameObjectPool _gameObjectPool = new GameObjectPool();

        /// <summary>
        /// 初始化;
        /// </summary>
        public void InitMgr()
        {
            ClearPool(null);
        }

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
            t.OnGet(args);
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

        /// <summary>
        /// 获取GameObject,可能为null;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public IEnumerator<float> Get(AssetType type, string assetName, Action<GameObject> onLoadFinish)
        {
            IEnumerator<float> itor = _gameObjectPool.Get(type, assetName, onLoadFinish);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        /// <summary>
        /// 贮存GameObject;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName"></param>
        /// <param name="element"></param>
        public void Release(AssetType type, string assetName, GameObject element)
        {
            _gameObjectPool.Release(type, assetName, element);
        }

        /// <summary>
        /// 销毁对象池;
        /// </summary>
        public void ClearPool(Action onFinish)
        {
            _pool.Clear();
            CoroutineMgr.Instance.RunCoroutine(_gameObjectPool.ClearPool(
                () =>
                {
                    System.GC.Collect();
                    if (onFinish != null)
                    {
                        onFinish();
                    }
                }
                ));
        }
    }
}
