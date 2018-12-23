/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  GameObject对象池管理;
*********************************************************************************/

using Framework.ObjectPool;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class PoolMgr
    {
        /// <summary>
        /// Unity Object Pool;
        /// </summary>
        private UnityGameObjectPool _unityGameObjectPool = new UnityGameObjectPool();

        /// <summary>
        /// 获取Unity GameObject;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public GameObject GetUnityGameObject(Object obj)
        {
            if (null == obj)
                return null;
            return _unityGameObjectPool.GetUnityGameObject(obj);
        }

        /// <summary>
        /// 贮存Unity GameObject;
        /// </summary>
        /// <param name="obj"></param>
        public void ReleaseUnityGameObject(Object obj)
        {
            if (null == obj)
                return;
            _unityGameObjectPool.ReleaseUnityGameObject(obj);
        }
    }
}
