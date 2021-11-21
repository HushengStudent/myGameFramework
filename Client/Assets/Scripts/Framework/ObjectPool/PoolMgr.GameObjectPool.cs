/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  GameObject对象池管理;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework.ObjectPool
{
    public partial class PoolMgr
    {
        private Dictionary<string, GameObjectPool> _poolDict =
            new Dictionary<string, GameObjectPool>();

        private Dictionary<string, HashSet<GameObjectPool>> _tagDict =
            new Dictionary<string, HashSet<GameObjectPool>>();

        /// <summary>
        /// 获取GameObjectPool;
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public GameObjectPool GetGameObjectPool(string assetPath, string tag = null)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                LogHelper.PrintError($"[PoolMgr]GameObjectPool error,assetPath is null.");
                return null;
            }
            if (!_poolDict.TryGetValue(assetPath, out var targetPool))
            {
                targetPool = GetCsharpObject<GameObjectPool>();
                targetPool.Initialize(assetPath, tag);
                _poolDict[assetPath] = targetPool;
            }
            if (!string.IsNullOrWhiteSpace(tag))
            {
                if (!_tagDict.TryGetValue(tag, out var poolList))
                {
                    poolList = new HashSet<GameObjectPool>();
                    _tagDict[tag] = poolList;
                }
                if (!poolList.Contains(targetPool))
                {
                    poolList.Add(targetPool);
                }
            }
            return targetPool;
        }

        /// <summary>
        /// 释放GameObjectPool;
        /// </summary>
        /// <param name="targetPool"></param>
        public void ReleaseGameObjectPool(GameObjectPool targetPool)
        {
            if (null == targetPool)
            {
                return;
            }
            ReleaseGameObjectPool(targetPool.AssetPath);
        }

        /// <summary>
        /// 释放GameObjectPool;
        /// </summary>
        /// <param name="assetPath"></param>
        public void ReleaseGameObjectPool(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return;
            }
            if (!_poolDict.TryGetValue(assetPath, out var targetPool))
            {
                return;
            }
            _poolDict.Remove(assetPath);
            foreach (var tag in targetPool.TagHashSet)
            {
                if (!_tagDict.TryGetValue(tag, out var poolList))
                {
                    continue;
                }
                if (poolList.Contains(targetPool))
                {
                    poolList.Remove(targetPool);
                }
            }
            ReleaseCsharpObject(targetPool);
        }

        public void ReleaseAllGameObjectPool()
        {
            foreach (var temp in _poolDict)
            {
                var targetPool = temp.Value;
                ReleaseCsharpObject(targetPool);
            }
            _poolDict.Clear();
            _tagDict.Clear();
        }
    }
}
