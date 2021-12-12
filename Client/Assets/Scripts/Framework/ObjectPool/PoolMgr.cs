/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 18:14:29
** desc:  对象池管理;
*********************************************************************************/

using Framework.EventModule;
using Framework.ResourceModule;
using System.Collections;
using UnityEngine;
using EventType = Framework.EventModule.EventType;

namespace Framework.ObjectPoolModule
{
    public partial class PoolMgr : MonoSingleton<PoolMgr>
    {
        private readonly string _poolRoot = "@AssetPoolRoot";

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
            CoroutineMgr.singleton.RunCoroutine(ClearPool());
        }

        /// <summary>
        /// 销毁对象池;
        /// </summary>
        public IEnumerator ClearPool()
        {
            yield return CoroutineMgr.WaitForEndOfFrame;
            _csharpObjectPool.Clear();
            _csharpListPool.Clear();
            ReleaseAllGameObjectPool();
            ResourceMgr.singleton.UnloadUnusedAssets(() =>
            {
                EventMgr.singleton.FireGlobalEvent(EventType.POOL_MGR_INIT, null);
            });
        }

        protected override void OnDestroyEx()
        {
            base.OnDestroyEx();
            DestroyImmediate(Root);
        }
    }
}