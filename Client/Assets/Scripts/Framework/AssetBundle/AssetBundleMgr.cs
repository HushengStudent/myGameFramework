/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:09
** desc:  AssetBundle管理;
*********************************************************************************/

using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
                    mainAssetBundle = AssetBundle.LoadFromFile(FilePathHelper.AssetBundlePath + "/AssetBundle");
                }
                if (mainAssetBundle == null)
                {
                    LogHelper.PrintError(string.Format("[AssetBundleMgr]Load assetBundle:{0} failure.", FilePathHelper.AssetBundlePath));
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
                    LogHelper.PrintError(string.Format("[AssetBundleMgr]Load assetBundleManifest:{0} failure.", FilePathHelper.AssetBundlePath));
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
        public void AddAssetRef(string path, Object asset)
        {
            if (string.IsNullOrEmpty(path) || null == asset)
            {
                return;
            }
            List<WeakReference> list;
            if (!_assetBundleRefDict.TryGetValue(path, out list))
            {
                list = PoolMgr.Instance.GetCsharpList<WeakReference>();
                _assetBundleRefDict[path] = list;
            }
            WeakReference reference = new WeakReference(asset);
            _assetBundleRefDict[path].Add(reference);
        }

        /// <summary>
        /// 自动卸载;
        /// </summary>
        public void AutoUnloadAsset()
        {
            List<string> deprecatedList = PoolMgr.Instance.GetCsharpList<string>();
            foreach (var reference in _assetBundleRefDict)
            {
                List<WeakReference> list = reference.Value;
                bool deprecated = true;
                for (int i = 0; i < list.Count; i++)
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
                    for (int i = 0; i < list.Count; i++)
                    {
                        UnloadAsset(path, null);
                    }
                    list.Clear();
                    PoolMgr.Instance.ReleaseCsharpList(list);
                    LogHelper.Print("[AssetBundleMgr]AutoUnloadAsset:" + path);
                    deprecatedList.Add(path);
                }
            }
            for (int i = 0; i < deprecatedList.Count; i++)
            {
                var path = deprecatedList[i];
                _assetBundleRefDict.Remove(path);
            }
            PoolMgr.Instance.ReleaseCsharpList(deprecatedList);
        }

        #endregion

        #region AssetBundle Load

        #region Sync Load

        /// <summary>
        /// 单个AssetBundle同步加载;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
                        LogHelper.PrintError(string.Format("[AssetBundleMgr]Load assetBundle:{0} failure.", path));
                    }
                    else
                    {
                        assetBundleCache[path] = assetBundle;
                        assetBundleReference[path] = 1;
                        LogHelper.Print(string.Format("[AssetBundleMgr]Load assetBundle:{0} Success.", path));
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
            string assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
            if (assetBundlePath == null)
            {
                return null;
            }
            string assetBundleName = FilePathHelper.GetAssetBundleFileName(path);

            AssetBundle assetBundle = LoadSync(assetBundlePath);
            if (assetBundle == null)
            {
                return null;
            }
            //返回AssetBundleName;
            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.shaderAssetBundleName) ||
                    tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.luaAssetBundleName))
                {
                    continue;
                }
                string tempPtah = FilePathHelper.AssetBundlePath + tempAssetBundle;
                LoadSync(tempPtah);
            }
            return assetBundle;
        }

        #endregion

        #region Async Load

        /// <summary>
        /// AssetBundle异步加载LoadFromFileAsync,www异步加载消耗大于LoadFromFileAsync;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
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
                AssetBundleCreateRequest assetBundleReq = AssetBundle.LoadFromFileAsync(path);
                //加载进度;
                while (assetBundleReq.progress < 0.99)
                {
                    if (null != progress)
                    {
                        progress(assetBundleReq.progress);
                    }
                    yield return Timing.WaitForOneFrame;
                }

                while (!assetBundleReq.isDone)
                {
                    yield return Timing.WaitForOneFrame;
                }
                assetBundle = assetBundleReq.assetBundle;
                if (assetBundle == null)
                {
                    LogHelper.Print(string.Format("[AssetBundleMgr]Load assetBundle:{0} failure.", path));
                }
                else
                {
                    assetBundleCache[path] = assetBundle;
                    assetBundleReference[path] = 1;
                    LogHelper.Print(string.Format("[AssetBundleMgr]Load assetBundle:{0} Success.", path));
                }
                //加载完毕;
                assetBundleLoading.Remove(path);
            }
            else
            {
                assetBundle = assetBundleCache[path];
                assetBundleReference[path]++;
            }
            if (action != null)
            {
                action(assetBundle);
            }
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
            string assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
            if (assetBundlePath == null)
            {
                yield break;
            }
            string assetBundleName = FilePathHelper.GetAssetBundleFileName(path);
            //先加载依赖的AssetBundle;
            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            float count = DependentAssetBundle.Length;
            float unit = 0.9f / (count + 1);
            int index = 0;
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                float dp = 0f;
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.shaderAssetBundleName) ||
                    tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.luaAssetBundleName))
                {
                    continue;
                }
                string tempPtah = FilePathHelper.AssetBundlePath + tempAssetBundle;
                IEnumerator<float> itor = LoadAsync(tempPtah, null, (value) => { dp = value; });
                while (itor.MoveNext())
                {
                    if (progress != null)
                    {
                        progress(unit * (index + dp));
                    }
                    yield return Timing.WaitForOneFrame;
                }
                index++;
            }
            //加载目标AssetBundle;
            float p = 0f;
            IEnumerator<float> itorTarget = LoadAsync(assetBundlePath, action, (value) => { p = value; });
            while (itorTarget.MoveNext())
            {
                if (progress != null)
                {
                    progress(unit * (count + p));
                }
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
        public void UnloadAsset(string path, Object asset)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (asset != null)
            {
                List<WeakReference> list;
                if (_assetBundleRefDict.TryGetValue(path, out list))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if ((Object)(list[i].Target) == asset)
                        {
                            list.RemoveAt(i);
                            //需要UnloadAsset?
                            //Resources.UnloadAsset(asset);
                            break;
                        }
                    }
                }
            }

            string assetBundleName = FilePathHelper.GetAssetBundleFileName(path);

            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(FilePathHelper.shaderAssetBundleName))
                {
                    continue;
                }
                string tempPtah = FilePathHelper.AssetBundlePath + tempAssetBundle;
                UnloadAsset(tempPtah, true);
            }
            string assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
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
            string assetBundlePath = FilePathHelper.GetAssetBundlePath(path);
            if (assetBundlePath != null)
            {
                UnloadAsset(assetBundlePath, false);
            }
        }

        /// <summary>
        /// 卸载AssetBundle资源;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flag"></param>
        private void UnloadAsset(string path, bool flag)
        {
            int Count = 0;
            if (assetBundleReference.TryGetValue(path, out Count))
            {
                Count--;
                if (Count == 0)
                {
                    assetBundleReference.Remove(path);
                    AssetBundle bundle = assetBundleCache[path];
                    if (bundle != null)
                    {
                        bundle.Unload(flag);
                    }
                    assetBundleCache.Remove(path);
                    LogHelper.Print(string.Format("[AssetBundleMgr]Unload:{0} assetBundle {1} success.", flag, path));
                }
                else
                {
                    assetBundleReference[path] = Count;
                }
            }
        }

        #endregion
    }
}
