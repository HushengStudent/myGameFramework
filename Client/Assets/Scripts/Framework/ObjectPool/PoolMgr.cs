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
        public ManagerInitEventHandler PoolMgrInitHandler = null;

        public GameObject Root { get; private set; }

        private void Awake()
        {
            Root = GameObject.Find("_resPoolRoot");
            if (Root == null)
            {
                Root = new GameObject("_resPoolRoot");
                DontDestroyOnLoad(Root);
            }
        }

        /// <summary>
        /// 初始化;
        /// </summary>
        public override void Init()
        {
            base.Init();
            CoroutineMgr.Instance.RunCoroutine(ClearPool());
        }

        /// <summary>
        /// 销毁对象池;
        /// </summary>
        public IEnumerator<float> ClearPool()
        {
            _csharpObjectPool.Clear();
            IEnumerator<float> _goPoolItor = _unityGameObjectPool.ClearUnityGameObjectPool();
            while (_goPoolItor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            if (PoolMgrInitHandler != null)
                PoolMgrInitHandler();
        }
    }
}
