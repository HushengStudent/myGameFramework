/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  GameObject对象池管理;
*********************************************************************************/

using Framework.ObjectPool;
using UnityObject = UnityEngine.Object;

namespace Framework
{
    public partial class PoolMgr
    {
        /// <summary>
        /// Unity Object Pool;
        /// </summary>
        private UnityObjectPool _unityObjectPool = new UnityObjectPool();

        /// <summary>
        /// 获取Unity GameObject;
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public UnityObject GetUnityObject(UnityObject asset)
        {
            if (null == asset)
            {
                return null;
            }
            return _unityObjectPool.GetUnityObject(asset);
        }

        /// <summary>
        /// 贮存Unity GameObject;
        /// </summary>
        /// <param name="asset"></param>
        public void ReleaseUnityObject(UnityObject asset)
        {
            if (null == asset)
            {
                return;
            }
            _unityObjectPool.ReleaseUnityObject(asset);
        }
    }
}
