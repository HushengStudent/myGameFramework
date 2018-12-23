/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/01 23:14:44
** desc:  Unity Object对象池;
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.ObjectPool
{
    internal class UnityObjectPool
    {
        private Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private int _unityObjectPoolMaxCount = 50;
        private Dictionary<int, Stack<Object>> _unityObjectPoolPool = new Dictionary<int, Stack<Object>>();
        private Dictionary<int, int> _unityObjectPoolIndex = new Dictionary<int, int>();

        public int UnityObjectPoolMaxCount
        {
            get { return _unityObjectPoolMaxCount; }
            set { _unityObjectPoolMaxCount = value; }
        }

        public Object GetUnityObject(Object obj)
        {
            if (null == obj)
                return null;
            Object element;
            int instanceId = obj.GetInstanceID();
            Stack<Object> stack;
            if (!_unityObjectPoolPool.TryGetValue(instanceId, out stack))
            {
                stack = new Stack<Object>();
                _unityObjectPoolPool[instanceId] = stack;
            }
            if (stack.Count == 0)
            {
                element = Object.Instantiate(obj);
            }
            else
            {
                element = stack.Pop();
            }
            _unityObjectPoolIndex[element.GetInstanceID()] = instanceId;
            GameObject go = element as GameObject;
            if (go)
            {
                go.transform.SetParent(null);
            }
            return element;
        }

        public void ReleaseUnityObject(Object obj)
        {
            Object element = obj;
            if (null == obj)
                return;
            int instanceId = element.GetInstanceID();
            Stack<Object> stack;
            int parentInstanceId;
            if (_unityObjectPoolPool.ContainsKey(instanceId))
            {
                parentInstanceId = instanceId;
            }
            else
            {
                if (!_unityObjectPoolIndex.TryGetValue(instanceId, out parentInstanceId))
                {
                    /*
                    stack = new Stack<Object>();
                    _unityObjectPoolPool[instanceId] = stack;
                    parentInstanceId = instanceId;
                    */
                    LogHelper.PrintWarning(string.Format("[UnityObjectPool]Release Unity Object:{0} is not create form pool " +
                                        "or it's parents destroyed,but it is trying to release to pool!", element.name));


                    Object.Destroy(element);
                    return;
                }
            }
            if (!_unityObjectPoolPool.TryGetValue(parentInstanceId, out stack))
            {
                stack = new Stack<Object>();
                _unityObjectPoolPool[parentInstanceId] = stack;
            }
            if (stack.Count > UnityObjectPoolMaxCount)
            {
                Object.Destroy(element);
                return;
            }
            stack.Push(element);
            GameObject go = element as GameObject;
            if (go)
            {
                go.transform.SetParent(PoolMgr.Instance.Root.transform);
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.Euler(Vector3.zero);
                go.transform.localScale = Vector3.one;
            }
        }

        public IEnumerator<float> ClearUnityObjectPool()
        {
            Dictionary<int, Stack<Object>> stack = new Dictionary<int, Stack<Object>>(_unityObjectPoolPool);
            _unityObjectPoolIndex = new Dictionary<int, int>();
            _unityObjectPoolPool = new Dictionary<int, Stack<Object>>();
            _stopwatch.Reset();
            _stopwatch.Start();
            foreach (var temp in stack)
            {
                Stack<Object> target = temp.Value;
                if (target == null || target.Count < 1)
                {
                    continue;
                }
                while (target.Count > 0)
                {
                    Object go = target.Pop();
                    if (go == null)
                    {
                        continue;
                    }
                    Object.Destroy(go);
                    if (_stopwatch.Elapsed.Milliseconds >= ResourceMgr.Instance.MAX_LOAD_TIME)
                    {
                        _stopwatch.Stop();
                        yield return Timing.WaitForOneFrame;
                    }
                }
            }
        }
    }
}
