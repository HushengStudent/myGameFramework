/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  Csharp����ع���;
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
        /// ��ȡCsharp�����Ŀ�����;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">��ʼ������</param>
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
        /// �ͷ�Csharp��������;
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

        /// ����Csharp�����;
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
