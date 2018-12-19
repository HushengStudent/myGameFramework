/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:09
** desc:  AssetBundle管理;
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        /// <summary>
        /// 加载出来的AssetBundle缓存;
        /// </summary>
        private Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 加载出来的AssetBundle引用计数;
        /// </summary>
        private Dictionary<string, int> assetBundleReference = new Dictionary<string, int>();

        /// <summary>
        /// 依赖关系AssetBundle;
        /// </summary>
        private AssetBundle mainAssetBundle;

        /// <summary>
        /// AssetBundleManifest
        /// </summary>
        private AssetBundleManifest manifest;

        /// <summary>
        /// 依赖关系AssetBundle;
        /// </summary>
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
                    LogHelper.PrintError(string.Format("[AssetBundleMgr]Load AssetBundle {0} failure!", FilePathHelper.AssetBundlePath + "/AssetBundle"));
                }
                return mainAssetBundle;
            }
        }

        /// <summary>
        /// AssetBundleManifest
        /// </summary>
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
                    LogHelper.PrintError(string.Format("[AssetBundleMgr]Load AssetBundleManifest {0} failure!", FilePathHelper.AssetBundlePath + "/AssetBundle"));
                }
                return manifest;
            }
        }

        /// <summary>
        /// 正在异步加载中的AssetBundle;
        /// </summary>
        public HashSet<string> assetBundleLoading = new HashSet<string>();

        #endregion

        #region Functions

        /// <summary>
        /// AssetBundle是否正在加载;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsAssetBundleLoading(string path)
        {
            return assetBundleLoading.Contains(path);
        }

        #endregion

        #region AssetBundle Load

        /// <summary>
        /// AssetBundle同步加载LoadFromFile;
        /// </summary>
        /// <param name="path">AssetBundle文件路径</param>
        /// <returns>AssetBundle</returns>
        private AssetBundle LoadSync(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            AssetBundle assetBundle = null;
            if (!assetBundleCache.ContainsKey(path))
            {
                try
                {
                    assetBundle = AssetBundle.LoadFromFile(path);
                    if (null == assetBundle)
                    {
                        LogHelper.PrintError(string.Format("[AssetBundleMgr]Load AssetBundle {0} failure!", path));
                    }
                    else
                    {
                        assetBundleCache[path] = assetBundle;
                        assetBundleReference[path] = 1;
                        LogHelper.Print(string.Format("[AssetBundleMgr]Load AssetBundle {0} Success!", path));
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
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>AssetBundle</returns>
        [Obsolete("Warning,forbid to call!")]
        public AssetBundle LoadAssetBundleSync(AssetType type, string assetName)
        {
            if (type == AssetType.Non || string.IsNullOrEmpty(assetName)) return null;

            string assetBundlePath = FilePathHelper.GetAssetBundlePath(type, assetName);
            if (assetBundlePath == null) return null;
            string assetBundleName = FilePathHelper.GetAssetBundleFileName(type, assetName);

            AssetBundle assetBundle = LoadSync(assetBundlePath);
            if (assetBundle == null) return null;

            //返回AssetBundleName;
            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(AssetType.Shader, "Shaders")) continue;
                string tempPtah = FilePathHelper.AssetBundlePath + tempAssetBundle;
                LoadSync(tempPtah);
            }
            return assetBundle;
        }

        /// <summary>
        /// AssetBundle异步加载LoadFromFileAsync,www异步加载消耗大于LoadFromFileAsync;
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="action">AssetBundle回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        private IEnumerator<float> LoadAsync(string path, Action<AssetBundle> action, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path)) yield break;

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
                        progress(assetBundleReq.progress);
                    yield return Timing.WaitForOneFrame;
                }

                while (!assetBundleReq.isDone)
                {
                    yield return Timing.WaitForOneFrame;
                }
                assetBundle = assetBundleReq.assetBundle;
                if (assetBundle == null)
                {
                    LogHelper.Print(string.Format("[AssetBundleMgr]Load AssetBundle {0} failure!", path));
                }
                else
                {
                    assetBundleCache[path] = assetBundle;
                    assetBundleReference[path] = 1;
                    LogHelper.Print(string.Format("[AssetBundleMgr]Load AssetBundle {0} Success!", path));
                }
                //加载完毕;
                assetBundleLoading.Remove(path);
            }
            else
            {
                assetBundle = assetBundleCache[path];
                assetBundleReference[path]++;
            }
            if (action != null) action(assetBundle);
        }

        /// <summary>
        /// AssetBundle异步加载;
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">AssetBundle回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        public IEnumerator<float> LoadAssetBundleAsync(AssetType type, string assetName, Action<AssetBundle> action, Action<float> progress)
        {
            if (type == AssetType.Non || string.IsNullOrEmpty(assetName)) yield break;
            string assetBundlePath = FilePathHelper.GetAssetBundlePath(type, assetName);
            if (assetBundlePath == null) yield break;
            string assetBundleName = FilePathHelper.GetAssetBundleFileName(type, assetName);
            //先加载依赖的AssetBundle;
            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            float count = DependentAssetBundle.Length;
            float unit = 0.9f / (count + 1);
            int index = 0;
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                float dp = 0f;
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(AssetType.Shader, "Shaders")) continue;
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

        /// <summary>
        /// 加载Shader AssetBundle;
        /// </summary>
        /// <returns>AssetBundle</returns>
        public AssetBundle LoadShaderAssetBundle()
        {
            string path = FilePathHelper.GetAssetBundlePath(AssetType.Shader, "Shaders");
            return LoadSync(path);
        }

        public AssetBundle LoadLuaAssetBundle()
        {
            string path = FilePathHelper.GetAssetBundlePath(AssetType.Lua, "lua");
            return LoadSync(path);
        }

        #endregion

        #region AssetBundle Unload

        /// <summary>
        /// 通用资源AssetBundle卸载方法[Unload(true)];
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        public void UnloadAsset(AssetType type, string assetName)
        {
            if (type == AssetType.Non || type == AssetType.Shader || type == AssetType.Lua || type == AssetType.Scripts || string.IsNullOrEmpty(assetName))
                return;

            string assetBundleName = FilePathHelper.GetAssetBundleFileName(type, assetName);

            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                if (tempAssetBundle == FilePathHelper.GetAssetBundleFileName(AssetType.Shader, "Shaders")) continue;
                string tempPtah = FilePathHelper.AssetBundlePath + tempAssetBundle;
                UnloadAsset(tempPtah, true);
            }
            string assetBundlePath = FilePathHelper.GetAssetBundlePath(type, assetName);
            if (assetBundlePath != null)
            {
                UnloadAsset(assetBundlePath, true);
            }
        }

        /// <summary>
        /// AssetBundle 镜像卸载方法[Unload(false)],使用资源为一般初始化就全局保存不在销毁的资源,如:Shader;
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        public void UnloadMirroring(AssetType type, string assetName)
        {
            if (type == AssetType.Non || type == AssetType.Scripts || string.IsNullOrEmpty(assetName))
                return;
            string assetBundlePath = FilePathHelper.GetAssetBundlePath(type, assetName);
            if (assetBundlePath != null)
            {
                UnloadAsset(assetBundlePath, false);
            }
        }

        /// <summary>
        /// 卸载AssetBundle资源;
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="flag">true or false</param>
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
                    if (bundle != null) bundle.Unload(flag);
                    assetBundleCache.Remove(path);
                    LogHelper.Print(string.Format("[AssetBundleMgr]Unload {0} AssetBundle {1} Success!", flag, path));
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
