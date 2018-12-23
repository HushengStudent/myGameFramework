/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/01 23:14:44
** desc:  Unity GameObject对象池;
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
    internal class UnityGameObjectPool
    {
        private Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private int _unityGameObjectPoolMaxCount = 50;
        private Dictionary<int, Stack<GameObject>> _unityGameObjectPoolPool = new Dictionary<int, Stack<GameObject>>();
        private Dictionary<int, int> _unityGameObjectPoolIndex = new Dictionary<int, int>();

        public int UnityGameObjectPoolMaxCount
        {
            get { return _unityGameObjectPoolMaxCount; }
            set { _unityGameObjectPoolMaxCount = value; }
        }

        public GameObject GetUnityGameObject(Object obj)
        {
            GameObject go = obj as GameObject;
            if (null == go)
                return null;
            GameObject element;
            int instanceId = go.GetInstanceID();
            Stack<GameObject> stack;
            if (!_unityGameObjectPoolPool.TryGetValue(instanceId, out stack))
            {
                stack = new Stack<GameObject>();
                _unityGameObjectPoolPool[instanceId] = stack;
            }
            if (stack.Count == 0)
            {
                element = GameObject.Instantiate(go);
            }
            else
            {
                element = stack.Pop();
            }
            _unityGameObjectPoolIndex[element.GetInstanceID()] = instanceId;
            element.transform.SetParent(null);
            return element;
        }

        public void ReleaseUnityGameObject(Object obj)
        {
            GameObject element = obj as GameObject;
            if (null == element)
                return;
            int instanceId = element.GetInstanceID();
            Stack<GameObject> stack;
            int parentInstanceId;
            if (_unityGameObjectPoolPool.ContainsKey(instanceId))
            {
                parentInstanceId = instanceId;
            }
            else
            {
                if (!_unityGameObjectPoolIndex.TryGetValue(instanceId, out parentInstanceId))
                {
                    stack = new Stack<GameObject>();
                    _unityGameObjectPoolPool[instanceId] = stack;
                    parentInstanceId = instanceId;
                    LogHelper.PrintWarning(string.Format("[UnityGameObjectPool]the game object:{0} is not create form pool " +
                        "or it's parents destroyed,but it is trying to release to pool!", element.name));
                }
            }
            if (!_unityGameObjectPoolPool.TryGetValue(parentInstanceId, out stack))
            {
                stack = new Stack<GameObject>();
                _unityGameObjectPoolPool[parentInstanceId] = stack;
            }
            if (stack.Count > UnityGameObjectPoolMaxCount)
            {
                GameObject.Destroy(element);
                return;
            }
            element.transform.SetParent(PoolMgr.Instance.Root.transform);
            element.transform.position = Vector3.zero;
            element.transform.rotation = Quaternion.Euler(Vector3.zero);
            element.transform.localScale = Vector3.one;
            stack.Push(element);
        }

        public IEnumerator<float> ClearUnityGameObjectPool()
        {
            Dictionary<int, Stack<GameObject>> stack = new Dictionary<int, Stack<GameObject>>(_unityGameObjectPoolPool);
            _unityGameObjectPoolIndex = new Dictionary<int, int>();
            _unityGameObjectPoolPool = new Dictionary<int, Stack<GameObject>>();
            _stopwatch.Reset();
            _stopwatch.Start();
            foreach (var temp in stack)
            {
                Stack<GameObject> target = temp.Value;
                if (target == null || target.Count < 1)
                {
                    continue;
                }
                while (target.Count > 0)
                {
                    GameObject go = target.Pop();
                    if (go == null)
                    {
                        continue;
                    }
                    GameObject.Destroy(go);
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
