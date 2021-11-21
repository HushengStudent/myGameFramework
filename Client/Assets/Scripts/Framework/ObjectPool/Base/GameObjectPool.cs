/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/01 23:14:44
** desc:  GameObjectPool对象池;
*********************************************************************************/

using Framework.ResourceManager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Framework.ObjectPoolManager
{
    public class GameObjectPool : IPool
    {
        private int _maxCount = 50;
        private Stack<GameObject> _stack;
        private HashSet<int> _instanceIDHashSet;
        private GameObject _parent;
        private AssetBundleAssetProxy _assetBundleAssetProxy;

        internal string AssetPath { get; private set; }
        internal HashSet<string> TagHashSet { get; private set; }

        internal void Initialize(string assetPath, string tag = null, int gameObjectMaxCount = 50)
        {
            _maxCount = gameObjectMaxCount;
            _stack = new Stack<GameObject>();
            _instanceIDHashSet = new HashSet<int>();
            AssetPath = assetPath;
            TagHashSet = new HashSet<string>();
            _assetBundleAssetProxy = ResourceMgr.singleton.LoadAssetAsync(assetPath);
            if (!string.IsNullOrWhiteSpace(tag) && !TagHashSet.Contains(tag))
            {
                TagHashSet.Add(tag);
            }
        }

        internal void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag) || TagHashSet.Contains(tag))
            {
                return;
            }
            TagHashSet.Add(tag);
        }

        public void SetPoolInitCallback(Action<GameObjectPool> action)
        {
            if (_assetBundleAssetProxy == null)
            {
                LogHelper.PrintError($"[GameObjectPool]SetPoolInitCallback error, pool:{AssetPath} assetProxy is null.");
                return;
            }
            _assetBundleAssetProxy.AddLoadFinishCallBack(() =>
            {
                action?.Invoke(this);
            });
        }

        public GameObject Get()
        {
            if (null == _assetBundleAssetProxy || _assetBundleAssetProxy.IsFinish)
            {
                LogHelper.PrintError($"[GameObjectPool]GetGameObject error, pool:{AssetPath} assetProxy is not finish.");
                return null;
            }
            GameObject go;
            if (_stack.Count == 0)
            {
                go = _assetBundleAssetProxy.GetInstantiateObject<GameObject>();
                var instanceID = go.GetInstanceID();
                _instanceIDHashSet.Add(instanceID);
            }
            else
            {
                go = _stack.Pop();
            }
            if (go)
            {
                go.transform.SetParent(null);
            }
            return go;
        }

        public void Release(GameObject go)
        {
            if (null == go)
            {
                LogHelper.PrintError($"[GameObjectPool]ReleaseGameObject error,gameObject is null, pool:{AssetPath}.");
                return;
            }
            if (_stack.Contains(go))
            {
                LogHelper.PrintError($"[GameObjectPool]ReleaseGameObject error, pool:{AssetPath} is already released to pool.");
                return;
            }
            if (!_instanceIDHashSet.Contains(go.GetInstanceID()))
            {
                LogHelper.PrintError($"[GameObjectPool]ReleaseGameObject error, pool:{AssetPath} is not create from this pool.");
                UnityObject.Destroy(go);
                return;
            }
            if (_stack.Count >= _maxCount)
            {
                UnityObject.Destroy(go);
                return;
            }
            if (_parent == null)
            {
                _parent = new GameObject(AssetPath);
                _parent.transform.SetParent(PoolMgr.singleton.Root.transform);
            }
            _stack.Push(go);
            go.transform.SetParent(_parent.transform);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.Euler(Vector3.zero);
            go.transform.localScale = Vector3.one;
        }

        public void Destroy()
        {
            PoolMgr.singleton.ReleaseGameObjectPool(AssetPath);
        }

        void IPool.OnGet(params object[] args)
        {
        }

        void IPool.OnRelease()
        {
            if (_parent)
            {
                UnityObject.Destroy(_parent);
                _parent = null;
                AssetPath = string.Empty;
            }
            _stack.Clear();
            _instanceIDHashSet.Clear();
            TagHashSet.Clear();
            AssetPath = string.Empty;
            _assetBundleAssetProxy.UnloadProxy();
            _assetBundleAssetProxy = null;
        }
    }
}