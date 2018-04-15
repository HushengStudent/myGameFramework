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
        private Dictionary<AssetType, Dictionary<string, Stack<GameObject>>> _pool =
            new Dictionary<AssetType, Dictionary<string, Stack<GameObject>>>();

        public IEnumerator<float> Get(AssetType type, string assetName, Action<GameObject> onLoadFinish)
        {
            GameObject element;
            Dictionary<string, Stack<GameObject>> m_Dict;
            Stack<GameObject> m_Stack;
            if (!_pool.TryGetValue(type, out m_Dict))
            {
                m_Dict = new Dictionary<string, Stack<GameObject>>();
                _pool[type] = m_Dict;
            }
            if (!m_Dict.TryGetValue(assetName, out m_Stack))
            {
                m_Stack = new Stack<GameObject>();
                m_Dict[assetName] = m_Stack;
            }
            if (m_Stack.Count == 0)
            {
                element = ResourceMgr.Instance.LoadResSync<GameObject>(type, assetName);
                if (element)
                {
                    onLoadFinish(element);
                    yield break;
                }
                IEnumerator<float> itor = ResourceMgr.Instance.LoadGameObjectFromAssetBundleAsync(type, assetName, onLoadFinish, null);
                while (itor.MoveNext())
                {
                    yield return Timing.WaitForOneFrame;
                }
            }
            else
            {
                element = m_Stack.Pop();
                if (onLoadFinish != null)
                {
                    onLoadFinish(element);
                }
            }
        }

        public void Release(AssetType type, string assetName, GameObject element)
        {
            Dictionary<string, Stack<GameObject>> m_Dict;
            Stack<GameObject> m_Stack;
            if (!_pool.TryGetValue(type, out m_Dict))
            {
                m_Dict = new Dictionary<string, Stack<GameObject>>();
                _pool[type] = m_Dict;
            }
            if (!m_Dict.TryGetValue(assetName, out m_Stack))
            {
                m_Stack = new Stack<GameObject>();
                m_Dict[assetName] = m_Stack;
            }
            element.transform.SetParent(null);
            element.transform.position = Vector3.zero;
            element.transform.rotation = Quaternion.Euler(Vector3.zero);
            element.transform.localScale = Vector3.one;
            m_Stack.Push(element);
        }

        public IEnumerator<float> ClearPool(Action onFinish)
        {
            Dictionary<AssetType, Dictionary<string, Stack<GameObject>>> tempDict
                = new Dictionary<AssetType, Dictionary<string, Stack<GameObject>>>(_pool);
            _pool = new Dictionary<AssetType, Dictionary<string, Stack<GameObject>>>();

            foreach (var temp in tempDict)
            {
                if (temp.Value == null)
                {
                    continue;
                }
                foreach (var value in temp.Value)
                {
                    Stack<GameObject> stack = value.Value;
                    if (stack == null)
                    {
                        continue;
                    }
                    while (stack.Count > 0)
                    {
                        GameObject go = stack.Pop();
                        GameObject.DestroyImmediate(go);
                        yield return Timing.WaitForOneFrame;
                    }
                }
            }
            Resources.UnloadUnusedAssets();
            if (onFinish != null)
            {
                onFinish();
            }
        }
    }
}
