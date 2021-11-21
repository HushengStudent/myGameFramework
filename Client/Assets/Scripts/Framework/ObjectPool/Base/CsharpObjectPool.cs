/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/23 23:54:49
** desc:  ugui源码;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework.ObjectPool
{
    internal class CsharpObjectPool<T> where T : new()
    {
        private readonly int _maxCount = 50;
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;

        public CsharpObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease, int maxCount = 50)
        {
            _maxCount = maxCount;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
            }
            else
            {
                element = m_Stack.Pop();
            }
            m_ActionOnGet?.Invoke(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count >= _maxCount)
            {
                return;
            }
            if (m_Stack.Contains(element))
            {
                LogHelper.PrintError("[CsharpObjectPool]Release error,trying to destroy object that is already released to pool.");
                return;
            }
            m_ActionOnRelease?.Invoke(element);
            m_Stack.Push(element);
        }
    }
}
