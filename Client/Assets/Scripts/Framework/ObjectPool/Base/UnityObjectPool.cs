/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/01 23:14:44
** desc:  Unity Object对象池;
*********************************************************************************/

using MEC;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Framework.ObjectPool
{
    internal class UnityObjectPool
    {
        private Stopwatch _stopwatch;
        private Dictionary<int, Stack<UnityObject>> _unityObjectDict;
        private Dictionary<int, int> _unityObjectRefDict;

        public int UnityObjectPoolMaxCount { get; set; }

        public UnityObjectPool()
        {
            UnityObjectPoolMaxCount = 50;
            _stopwatch = new Stopwatch();
            _unityObjectDict = new Dictionary<int, Stack<UnityObject>>();
            _unityObjectRefDict = new Dictionary<int, int>();
        }

        public UnityObject GetUnityObject(UnityObject asset)
        {
            if (null == asset)
            {
                return null;
            }
            var instanceID = asset.GetInstanceID();
            if (!_unityObjectDict.TryGetValue(instanceID, out var stack))
            {
                stack = new Stack<UnityObject>();
                _unityObjectDict[instanceID] = stack;
            }
            UnityObject element;
            if (stack.Count == 0)
            {
                element = UnityObject.Instantiate(asset);
            }
            else
            {
                element = stack.Pop();
            }
            _unityObjectRefDict[element.GetInstanceID()] = instanceID;
            var go = element as GameObject;
            if (go)
            {
                go.transform.SetParent(null);
            }
            return element;
        }

        public void ReleaseUnityObject(UnityObject asset)
        {
            var element = asset;
            if (null == element)
            {
                return;
            }
            var instanceID = element.GetInstanceID();
            int parentInstanceID;
            if (_unityObjectDict.ContainsKey(instanceID))
            {
                parentInstanceID = instanceID;
            }
            else
            {
                if (!_unityObjectRefDict.TryGetValue(instanceID, out parentInstanceID))
                {
                    /*
                    stack = new Stack<Object>();
                    _unityObjectPoolPool[instanceId] = stack;
                    parentInstanceId = instanceId;
                    */

                    /// not create form pool or parents destroyed;
                    LogHelper.PrintWarning($"[UnityObjectPool]Release unity object error:{element.name}.");

                    UnityObject.Destroy(element);

                    return;
                }
            }
            if (!_unityObjectDict.TryGetValue(parentInstanceID, out var stack))
            {
                stack = new Stack<UnityObject>();
                _unityObjectDict[parentInstanceID] = stack;
            }
            if (stack.Count > UnityObjectPoolMaxCount)
            {
                UnityObject.Destroy(element);
                return;
            }
            stack.Push(element);
            var go = element as GameObject;
            if (go)
            {
                go.transform.SetParent(PoolMgr.singleton.Root.transform);
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.Euler(Vector3.zero);
                go.transform.localScale = Vector3.one;
            }
        }

        public IEnumerator<float> ClearUnityObjectPool()
        {
            var objectPool = new Dictionary<int, Stack<UnityObject>>(_unityObjectDict);
            _unityObjectRefDict = new Dictionary<int, int>();
            _unityObjectDict = new Dictionary<int, Stack<UnityObject>>();
            _stopwatch.Reset();
            _stopwatch.Start();
            foreach (var temp in objectPool)
            {
                var target = temp.Value;
                if (target == null || target.Count < 1)
                {
                    continue;
                }
                while (target.Count > 0)
                {
                    var go = target.Pop();
                    if (go == null)
                    {
                        continue;
                    }
                    UnityObject.Destroy(go);
                    if (_stopwatch.Elapsed.Milliseconds >= ResourceMgr.singleton.MAX_LOAD_TIME)
                    {
                        _stopwatch.Stop();
                        yield return Timing.WaitForOneFrame;
                    }
                }
            }
        }
    }
}
