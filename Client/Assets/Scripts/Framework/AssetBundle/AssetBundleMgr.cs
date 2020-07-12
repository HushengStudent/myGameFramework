/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:09
** desc:  AssetBundle管理;
*********************************************************************************/

using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// 1.AssetBundle加载完成后再加载AssetBundle,或开始异步加载AssetBundle后再同步加载AssetBundle,都会报错;
    /// The AssetBundle xxxx can't be loaded because another AssetBundle with the same files is already loaded.(同一帧先异步加载再同步加载,异步加载成功,但是同步加载报这个错,return null);
    /// 2.开始异步加载AssetBundle后再异步加载AssetBundle,会报错;
    /// Unable to open archive file: xxxx;
    /// </summary>
    public partial class AssetBundleMgr : Singleton<AssetBundleMgr>
    {
        #region Fields

        /// 加载出来的AssetBundle缓存;
        private Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>();

        /// 加载出来的AssetBundle引用计数;
        private Dictionary<string, int> assetBundleReference = new Dictionary<string, int>();

        /// 依赖关系AssetBundle;
        private AssetBundle mainAssetBundle;

        /// AssetBundleManifest
        private AssetBundleManifest manifest;

        /// 依赖关系AssetBundle;
        private AssetBundle MainAssetBundle
        {
            get
            {
                if (null == mainAssetBundle)
                {
                    mainAssetBundle = AssetBundle.LoadFromFile($"{FilePathHelper.AssetBundlePath}/AssetBundle");
                }
                if (mainAssetBundle == null)
                {
                    LogHelper.PrintError($"[AssetBundleMgr]Load assetBundle:{FilePathHelper.AssetBundlePath} failure.");
                }
                return mainAssetBundle;
            }
        }

        /// AssetBundleManifest
        private AssetBundleManifest Manifest
        {
            get
            {
                if (null == manifest && MainAssetBundle != null)
                {
                    manifest = MainAssetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                }
                if (manifest == null)
                {
                    LogHelper.PrintError($"[AssetBundleMgr]Load assetBundleManifest:{FilePathHelper.AssetBundlePath} failure.");
                }
                return manifest;
            }
        }

        /// 正在异步加载中的AssetBundle;
        private HashSet<string> assetBundleLoading = new HashSet<string>();


        /// 资源引用关系;
        private Dictionary<string, List<WeakReference>> _assetBundleRefDict =
            new Dictionary<string, List<WeakReference>>();

        #endregion

        #region Functions

        /// AssetBundle是否正在加载;
        private bool IsAssetBundleLoading(string path)
        {
            return assetBundleLoading.Contains(path);
        }

        /// <summary>
        /// Shaders AssetBundle;
        /// </summary>
        /// <returns>AssetBundle</returns>
        public AssetBundle LoadShaderAssetBundle()
        {
            return LoadFromFile(FilePathHelper.shaderAssetBundleName);
        }

        /// <summary>
        /// Lua AssetBundle;
        /// </summary>
        /// <returns></returns>
        public AssetBundle LoadLuaAssetBundle()
        {
            return LoadFromFile(FilePathHelper.luaAssetBundleName);
        }

        /// <summary>
        /// 添加资源引用;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        public void AddAssetRef(string path, UnityObject asset)
        {
            if (string.IsNullOrEmpty(path) || null == asset)
            {
                return;
            }
            if (!_assetBundleRefDict.TryGetValue(path, out var list))
            {
                list = PoolMgr.singleton.GetCsharpList<WeakReference>();
                _assetBundleRefDict[path] = list;
            }
            var reference = new WeakReference(asset);
            _assetBundleRefDict[path].Add(reference);
        }

        /// <summary>
        /// 自动卸载;
        /// </summary>
        public void AutoUnloadAsset()
        {
            var deprecatedList = PoolMgr.singleton.GetCsharpList<string>();
            foreach (var reference in _assetBundleRefDict)
            {
                var list = reference.Value;
                var deprecated = true;
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].Target != null)
                    {
                        deprecated = false;
                        break;
                    }
                }
                if (deprecated)
                {
                    var path = reference.Key;
                    for (var i = 0; i < list.Count; i++)
                    {
                        UnloadAsset(path, null);
                    }
                    list.Clear();
                    PoolMgr.singleton.ReleaseCsharpList(list);
                    LogHelper.Print($"[AssetBundleMgr]AutoUnloadAsset:{path}");
                    deprecatedList.Add(path);
                }
            }
            for (var i = 0; i < deprecatedList.Count; i++)
            {
                var path = deprecatedList[i];
                _assetBundleRefDict.Remove(path);
            }
            PoolMgr.singleton.ReleaseCsharpList(deprecatedList);
        }

        #endregion

        #region AssetBundle Load

        #region Sync Load

        /// 单个AssetBundle同步加载;
        private AssetBundle LoadSync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            AssetBundle assetBundle = null;
            if (!assetBundleCache.ContainsKey(path))
            {
                try
                {
                    assetBundle = AssetBundle.LoadFromFile(path);
                    if (null == assetBundle)
                    {
                        LogHelper.PrintError($"[AssetBundleMgr]Load assetBundle:{path} failure.");
                    }
                    else
                    {
                        assetBundleCache[path] = assetBundle;
                        assetBundleReference[path] = 1;
                        LogHelper.Print($"[AssetBundleMgr]Load assetBundle:{path} success.");
                    }
                }
                catch (Exception e)
                {
                    LogHelper.PrintError(e.ToString());
                }
            }
            else
            {
                assetBundle = assetBundleCache[path];
                assetBundleReference[path]++;
            }
            return assetBundle;
        }

        /// <summary>
        /// AssetBundle同步加载;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssetBundle LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
            if (assetBundlePath == null)
            {
                return null;
            }
            var assetBundleName = FilePathHelper.GetAssetBundleFileName(path);

            var assetBundle = LoadSync(assetBundlePath);
            if (assetBundle == null)
            {
                return null;
            }
            //返回AssetBundleName;
            var dependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (var tempAssetBundle in dependentAssetBundle)
            {
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.shaderAssetBundleName) ||
                    tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.luaAssetBundleName))
                {
                    continue;
                }
                var tempPtah = FilePathHelper.AssetBundlePath + tempAssetBundle;
                LoadSync(tempPtah);
            }
            return assetBundle;
        }

        #endregion

        #region Async Load

        /// AssetBundle异步加载LoadFromFileAsync,www异步加载消耗大于LoadFromFileAsync;
        private IEnumerator<float> LoadAsync(string path, Action<AssetBundle> action, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                yield break;
            }
            AssetBundle assetBundle = null;
            while (IsAssetBundleLoading(path))
            {
                yield return Timing.WaitForOneFrame;
            }
            if (!assetBundleCache.ContainsKey(path))
            {
                //开始加载;
                assetBundleLoading.Add(path);
                var assetBundleReq = AssetBundle.LoadFromFileAsync(path);
                //加载进度;
                while (assetBundleReq.progress < 0.99)
                {
                    progress?.Invoke(assetBundleReq.progress);
                    yield return Timing.WaitForOneFrame;
                }

                while (!assetBundleReq.isDone)
                {
                    yield return Timing.WaitForOneFrame;
                }
                assetBundle = assetBundleReq.assetBundle;
                if (assetBundle == null)
                {
                    LogHelper.Print($"[AssetBundleMgr]Load assetBundle:{path} failure.");
                }
                else
                {
                    assetBundleCache[path] = assetBundle;
                    assetBundleReference[path] = 1;
                    LogHelper.Print($"[AssetBundleMgr]Load assetBundle:{path} Success.");
                }
                //加载完毕;
                assetBundleLoading.Remove(path);
            }
            else
            {
                assetBundle = assetBundleCache[path];
                assetBundleReference[path]++;
            }
            action?.Invoke(assetBundle);
        }

        /// <summary>
        /// AssetBundle异步加载;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public IEnumerator<float> LoadFromFileAsync(string path, Action<AssetBundle> action, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                yield break;
            }
            var assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
            if (assetBundlePath == null)
            {
                yield break;
            }
            var assetBundleName = FilePathHelper.GetAssetBundleFileName(path);
            //先加载依赖的AssetBundle;
            var dependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            var count = dependentAssetBundle.Length;
            var precent = ResourceMgr.singleton.LOAD_BUNDLE_PRECENT;
            var unit = precent / (count + 1);
            int index = 0;
            foreach (var tempAssetBundle in dependentAssetBundle)
            {
                var dp = 0f;
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.shaderAssetBundleName) ||
                    tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.luaAssetBundleName))
                {
                    continue;
                }
                var tempPtah = $"{FilePathHelper.AssetBundlePath}/{tempAssetBundle}";
                var itor = LoadAsync(tempPtah, null, (value) => { dp = value; });
                while (itor.MoveNext())
                {
                    progress?.Invoke(unit * (index + dp));
                    yield return Timing.WaitForOneFrame;
                }
                index++;
            }
            //加载目标AssetBundle;
            var p = 0f;
            var itorTarget = LoadAsync(assetBundlePath, action, (value) => { p = value; });
            while (itorTarget.MoveNext())
            {
                progress?.Invoke(unit * (count + p));
                yield return Timing.WaitForOneFrame;
            }
        }

        #endregion

        #endregion

        #region AssetBundle Unload

        /// <summary>
        /// 通用资源AssetBundle卸载方法[Unload(true)];
        /// </summary>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        public void UnloadAsset(string path, UnityObject asset)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (asset != null)
            {
                if (_assetBundleRefDict.TryGetValue(path, out var list))
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        if ((UnityObject)(list[i].Target) == asset)
                        {
                            list.RemoveAt(i);
                            //需要UnloadAsset?
                            //Resources.UnloadAsset(asset);
                            break;
                        }
                    }
                }
            }

            var assetBundleName = FilePathHelper.GetAssetBundleFileName(path);

            var dependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (var tempAssetBundle in dependentAssetBundle)
            {
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.shaderAssetBundleName))
                {
                    continue;
                }
                var tempPtah = FilePathHelper.AssetBundlePath + tempAssetBundle;
                UnloadAsset(tempPtah, true);
            }
            var assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
            if (assetBundlePath != null)
            {
                UnloadAsset(assetBundlePath, true);
            }
        }

        /// <summary>
        /// AssetBundle 镜像卸载方法[Unload(false)];
        /// 使用资源为一般初始化就全局保存不在销毁的资源,如:Shader;
        /// </summary>
        /// <param name="path"></param>
        public void UnloadMirroring(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
            if (assetBundlePath != null)
            {
                UnloadAsset(assetBundlePath, false);
            }
        }

        /// 卸载AssetBundle资源;
        private void UnloadAsset(string path, bool flag)
        {
            if (assetBundleReference.TryGetValue(path, out var count))
            {
                count--;
                if (count == 0)
                {
                    assetBundleReference.Remove(path);
                    var bundle = assetBundleCache[path];
                    if (bundle != null)
                    {
                        bundle.Unload(flag);
                    }
                    assetBundleCache.Remove(path);
                    LogHelper.Print($"[AssetBundleMgr]Unload:{flag} assetBundle {path} success.");
                }
                else
                {
                    assetBundleReference[path] = count;
                }
            }
        }

        #endregion
    }
}
