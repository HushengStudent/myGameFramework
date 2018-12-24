/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/16 20:34:27
** desc:  Unity Asset缓存池;
*********************************************************************************/

using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Object = UnityEngine.Object;

namespace Framework
{
    internal class UnityAssetCachePool
    {
        public class UnityAssetCacheInfo : IPool
        {
            public AssetType assetType { get; private set; }
            public string assetName { get; private set; }
            public Object target { get; private set; }

            public void Init(AssetType assetType, string assetName, Object target)
            {
                this.assetType = assetType;
                this.assetName = assetName;
                this.target = target;
            }

            public void OnGet(params object[] args)
            {
            }

            public void OnRelease()
            {
                assetType = AssetType.Non;
                assetName = null;
                target = null;
            }
        }

        private Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private int _unityAssetCachePoolMaxCount = 50;
        private Dictionary<string, UnityAssetCacheInfo> _unityAssetCacheDict = new Dictionary<string, UnityAssetCacheInfo>();
        private Dictionary<string, int> _unityAssetRefCountDict = new Dictionary<string, int>();

        public int UnityAssetCachePoolMaxCount
        {
            get { return _unityAssetCachePoolMaxCount; }
            set { _unityAssetCachePoolMaxCount = value; }
        }

        public Object GetUnityAsset(AssetType assetType, string assetName)
        {
            string path = FilePathHelper.GetAssetBundlePath(assetType, assetName);
            UnityAssetCacheInfo info = null;
            if (!string.IsNullOrEmpty(path) && _unityAssetCacheDict.TryGetValue(path, out info))
            {
                int count = 0;
                if (!_unityAssetRefCountDict.TryGetValue(path, out count))
                {
                    count = 0;
                }
                count++;
                _unityAssetRefCountDict[path] = count;
            }
            if (null == info)
            {
                return null;
            }
            else
            {
                return info.target;
            }
        }

        public void ReleaseUnityAsset(AssetType assetType, string assetName, Object obj, bool isUsePool)
        {
            string path = FilePathHelper.GetAssetBundlePath(assetType, assetName);
            if (null == obj)
            {
                LogHelper.PrintError(string.Format("[UnityAssetCachePool]Release unity asset:{0} error" +
                    ",Object is null!", path));
                return;
            }
            if (string.IsNullOrEmpty(path))
            {
                ResourceMgr.Instance.UnloadUnityAsset(assetType, obj);
                return;
            }
            UnityAssetCacheInfo info = null;
            if (_unityAssetCacheDict.TryGetValue(path, out info))
            {
                if (info.target != obj)
                {
                    LogHelper.PrintError(string.Format("[UnityAssetCachePool]Release unity asset:{0} error" +
                        ",the path ref multiple Object!", path));
                    ResourceMgr.Instance.UnloadUnityAsset(assetType, obj);
                    return;
                }
                int count = 0;
                if (!_unityAssetRefCountDict.TryGetValue(path, out count))
                {
                    LogHelper.PrintError(string.Format("[UnityAssetCachePool]Release unity asset:{0} error" +
                        ",ref count not exist!", path));
                    _unityAssetRefCountDict[path] = 0;
                }
                else
                {
                    _unityAssetRefCountDict[path]--;
                    if (!isUsePool)
                    {
                        //引用为0,又不是初次放入,直接回收;
                        if (_unityAssetRefCountDict[path] == 0)
                        {
                            _unityAssetCacheDict.Remove(path);
                            _unityAssetRefCountDict.Remove(path);
                            ResourceMgr.Instance.UnloadUnityAsset(assetType, info.target);
                            PoolMgr.Instance.ReleaseCsharpObject<UnityAssetCacheInfo>(info);
                            AssetBundleMgr.Instance.UnloadAsset(assetType, assetName);
                        }
                    }
                }
            }
            else
            {
                info = PoolMgr.Instance.GetCsharpObject<UnityAssetCacheInfo>();
                info.Init(assetType, assetName, obj);
                _unityAssetCacheDict[path] = info;
                int count = 0;
                if (_unityAssetRefCountDict.TryGetValue(path, out count))
                {
                    LogHelper.PrintError(string.Format("[UnityAssetCachePool]Release unity asset:{0} error" +
                        ",ref count != 0!", path));
                }
                _unityAssetRefCountDict[path] = 0;
            }
        }

        public IEnumerator<float> ClearUnityAssetCachePool()
        {
            Dictionary<string, UnityAssetCacheInfo> tempUnityAssetCacheDict = new Dictionary<string, UnityAssetCacheInfo>(_unityAssetCacheDict);
            _unityAssetCacheDict = new Dictionary<string, UnityAssetCacheInfo>();
            _unityAssetRefCountDict = new Dictionary<string, int>();
            _stopwatch.Reset();
            _stopwatch.Start();
            foreach (var temp in tempUnityAssetCacheDict)
            {
                UnityAssetCacheInfo info = temp.Value;
                if (info == null)
                {
                    continue;
                }
                if (info.target != null)
                {
                    ResourceMgr.Instance.UnloadUnityAsset(info.assetType, info.target);
                }
                PoolMgr.Instance.ReleaseCsharpObject<UnityAssetCacheInfo>(info);
                if (_stopwatch.Elapsed.Milliseconds >= ResourceMgr.Instance.MAX_LOAD_TIME)
                {
                    _stopwatch.Stop();
                    yield return Timing.WaitForOneFrame;
                }
            }
        }
    }
}
