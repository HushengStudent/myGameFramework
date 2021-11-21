/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  GameObject对象池管理;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework.ObjectPoolModule
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
            targetPool.AddTag(tag);
            if (!string.IsNullOrWhiteSpace(tag))
            {
                if (!_tagDict.TryGetValue(tag, out var poolHashSet))
                {
                    poolHashSet = new HashSet<GameObjectPool>();
                    _tagDict[tag] = poolHashSet;
                }
                if (!poolHashSet.Contains(targetPool))
                {
                    poolHashSet.Add(targetPool);
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
                if (!_tagDict.TryGetValue(tag, out var poolHashSet))
                {
                    continue;
                }
                if (poolHashSet.Contains(targetPool))
                {
                    poolHashSet.Remove(targetPool);
                }
            }
            ReleaseCsharpObject(targetPool);

            var list = GetCsharpList<string>();
            foreach (var temp in _tagDict)
            {
                if (temp.Value.Count < 1)
                {
                    list.Add(temp.Key);
                }
            }
            foreach (var name in list)
            {
                _tagDict.Remove(name);
            }
            ReleaseCsharpList(list);
        }

        /// <summary>
        /// 释放GameObjectPool;
        /// </summary>
        /// <param name="tag"></param>
        public void ReleaseGameObjectPoolByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return;
            }
            if (!_tagDict.TryGetValue(tag, out var poolHashSet))
            {
                return;
            }
            foreach (var targetPool in poolHashSet)
            {
                _poolDict.Remove(targetPool.AssetPath);
                ReleaseCsharpObject(targetPool);
            }
            _tagDict.Remove(tag);
        }

        /// <summary>
        /// 释放全部GameObjectPool;
        /// </summary>
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