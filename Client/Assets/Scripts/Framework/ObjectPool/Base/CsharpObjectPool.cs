/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/23 23:54:49
** desc:  ugui源码;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.ObjectPool
{
    internal class CsharpObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;

        public int CountAll { get; private set; }
        public int CountActive { get { return CountAll - CountInactive; } }
        public int CountInactive { get { return m_Stack.Count; } }

        public CsharpObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                CountAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            {
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            }
            m_ActionOnRelease?.Invoke(element);
            m_Stack.Push(element);
        }
    }
}
