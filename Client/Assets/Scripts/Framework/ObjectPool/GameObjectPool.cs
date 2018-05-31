/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/01 23:14:44
** desc:  GameObject对象池;
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class GameObjectPool
    {
        /// <summary>
        /// 单帧卸载数量;
        /// </summary>
        private int _preFrameClearCount = 10;

        private Dictionary<int, Stack<GameObject>> _gameObjectPool = new Dictionary<int, Stack<GameObject>>();

        private Dictionary<int, int> _gameObjectIndex = new Dictionary<int, int>();

        public int PreFrameClearCount { get { return _preFrameClearCount; } set { _preFrameClearCount = value; } }

        public GameObject Clone(GameObject go)
        {
            GameObject element;
            int instanceId = go.GetInstanceID();
            Stack<GameObject> stack;
            if (!_gameObjectPool.TryGetValue(instanceId, out stack))
            {
                stack = new Stack<GameObject>();
                _gameObjectPool[instanceId] = stack;
            }
            if (stack.Count == 0)
            {
                element = GameObject.Instantiate(go);//clone出来的子物体instanceId == parent instanceId;
            }
            else
            {
                element = stack.Pop();
            }
            _gameObjectIndex[element.GetInstanceID()] = instanceId;
            element.transform.SetParent(null);
            return element;
        }

        public void Release(GameObject element)
        {
            int instanceId = element.GetInstanceID();
            Stack<GameObject> stack;
            int parentInstanceId;
            if (_gameObjectPool.ContainsKey(instanceId))
            {
                parentInstanceId = instanceId;
            }
            else
            {
                if (!_gameObjectIndex.TryGetValue(instanceId, out parentInstanceId))
                {
                    stack = new Stack<GameObject>();
                    _gameObjectPool[instanceId] = stack;
                    parentInstanceId = instanceId;
                    LogUtil.LogUtility.PrintWarning(string.Format("[GameObjectPool]the game object:{0} is not create form pool " +
                        "or it's parents destroyed,but it is trying to release to pool!",element.name));
                }
            }
            if (!_gameObjectPool.TryGetValue(parentInstanceId, out stack))
            {
                stack = new Stack<GameObject>();
                _gameObjectPool[parentInstanceId] = stack;
            }
            element.transform.SetParent(PoolMgr.Instance.Root.transform);
            element.transform.position = Vector3.zero;
            element.transform.rotation = Quaternion.Euler(Vector3.zero);
            element.transform.localScale = Vector3.one;
            stack.Push(element);
        }

        public IEnumerator<float> ClearGameObjectPool()
        {
            Dictionary<int, Stack<GameObject>> stack = new Dictionary<int, Stack<GameObject>>(_gameObjectPool);
            _gameObjectIndex = new Dictionary<int, int>();
            _gameObjectPool = new Dictionary<int, Stack<GameObject>>();
            int index = 0;
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
                    GameObject.DestroyImmediate(go);
                    index++;
                    if (index > PreFrameClearCount)
                    {
                        yield return Timing.WaitForOneFrame;
                        index = 0;
                    }
                }
            }
        }
    }
}
