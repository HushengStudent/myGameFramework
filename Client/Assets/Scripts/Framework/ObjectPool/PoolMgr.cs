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
    public partial class PoolMgr : MonoSingleton<PoolMgr>
    {
        private readonly string _poolRoot = "@ResourcePoolRoot";

        public static Action onPoolInitAction = null;

        public GameObject Root { get; private set; }

        private void Awake()
        {
            Root = GameObject.Find(_poolRoot);
            if (Root == null)
            {
                Root = new GameObject(_poolRoot);
                DontDestroyOnLoad(Root);
            }
        }

        /// <summary>
        /// 初始化;
        /// </summary>
        protected override void OnInitialize()
        {
            CoroutineMgr.Instance.RunCoroutine(ClearPool());
        }

        /// <summary>
        /// 销毁对象池;
        /// </summary>
        public IEnumerator<float> ClearPool()
        {
            _csharpObjectPool.Clear();
            _csharpListPool.Clear();
            IEnumerator<float> _unityObjectPoolItor = _unityObjectPool.ClearUnityObjectPool();
            while (_unityObjectPoolItor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            IEnumerator<float> _unityAssetCachePoolItor = _unityAssetCachePool.ClearUnityAssetCachePool();
            while (_unityAssetCachePoolItor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            if (onPoolInitAction != null)
                onPoolInitAction();
        }
    }
}
