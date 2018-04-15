/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:09
** desc:  AssetBundle管理;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// AssetBundle在异步加载完成时再同步加载会报错;
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
                    mainAssetBundle = AssetBundle.LoadFromFile(FilePathUtility.AssetBundlePath + "AssetBundle");
                }
                if (mainAssetBundle == null)
                {
                    LogUtil.LogUtility.PrintError(string.Format("[AssetBundleMgr]Load AssetBundle {0} failure!", FilePathUtility.AssetBundlePath + "AssetBundle"));
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
                    LogUtil.LogUtility.PrintError(string.Format("[AssetBundleMgr]Load AssetBundleManifest {0} failure!", FilePathUtility.AssetBundlePath + "AssetBundle"));
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
        private AssetBundle LoadSingleSync(string path)
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
                        LogUtil.LogUtility.PrintError(string.Format("[AssetBundleMgr]Load AssetBundle {0} failure!", path));
                    }
                    else
                    {
                        assetBundleCache[path] = assetBundle;
                        assetBundleReference[path] = 1;
                        LogUtil.LogUtility.Print(string.Format("[AssetBundleMgr]Load AssetBundle {0} Success!", path));
                    }
                }
                catch (Exception e)
                {
                    LogUtil.LogUtility.PrintError(e.ToString());
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
        /// AssetBundle异步加载LoadFromFileAsync,www异步加载消耗大于LoadFromFileAsync;
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="action">AssetBundle回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        private IEnumerator LoadSingleAsync(string path, Action<AssetBundle> action, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path)) yield break;

            AssetBundle assetBundle = null;
            while (IsAssetBundleLoading(path))
            {
                yield return null;
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
                    yield return null;
                }

                while (!assetBundleReq.isDone)
                {
                    yield return null;
                }
                assetBundle = assetBundleReq.assetBundle;
                if (assetBundle == null)
                {
                    LogUtil.LogUtility.Print(string.Format("[AssetBundleMgr]Load AssetBundle {0} failure!", path));
                }
                else
                {
                    assetBundleCache[path] = assetBundle;
                    assetBundleReference[path] = 1;
                    LogUtil.LogUtility.Print(string.Format("[AssetBundleMgr]Load AssetBundle {0} Success!", path));
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
        /// AssetBundle同步加载;
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>AssetBundle</returns>
        public AssetBundle LoadAssetBundleSync(AssetType type, string assetName)
        {
            if (type == AssetType.Non || string.IsNullOrEmpty(assetName)) return null;

            string assetBundlePath = FilePathUtility.GetAssetBundlePath(type, assetName);
            if (assetBundlePath == null) return null;
            string assetBundleName = FilePathUtility.GetAssetBundleFileName(type, assetName);

            AssetBundle assetBundle = LoadSingleSync(assetBundlePath);
            if (assetBundle == null) return null;

            //返回AssetBundleName;
            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                if (tempAssetBundle == FilePathUtility.GetAssetBundleFileName(AssetType.Shader, "Shader")) continue;
                string tempPtah = FilePathUtility.AssetBundlePath + tempAssetBundle;
                LoadSingleSync(tempPtah);
            }
            return assetBundle;
        }

        /// <summary>
        /// AssetBundle异步加载;
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">AssetBundle回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        public IEnumerator LoadAssetBundleAsync(AssetType type, string assetName, Action<AssetBundle> action, Action<float> progress)
        {
            if (type == AssetType.Non || string.IsNullOrEmpty(assetName)) yield break;
            string assetBundlePath = FilePathUtility.GetAssetBundlePath(type, assetName);
            if (assetBundlePath == null) yield break;
            string assetBundleName = FilePathUtility.GetAssetBundleFileName(type, assetName);
            //先加载依赖的AssetBundle;
            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                if (tempAssetBundle == FilePathUtility.GetAssetBundleFileName(AssetType.Shader, "Shader")) continue;
                string tempPtah = FilePathUtility.AssetBundlePath + tempAssetBundle;
                IEnumerator itor = LoadSingleAsync(tempPtah, null, null);
                while (itor.MoveNext())
                {
                    yield return null;
                }
            }
            //加载目标AssetBundle;
            IEnumerator itorTarget = LoadSingleAsync(assetBundlePath, action, progress);
            while (itorTarget.MoveNext())
            {
                yield return null;
            }
        }

        /// <summary>
        /// 加载Shader AssetBundle;
        /// </summary>
        /// <returns>AssetBundle</returns>
        public AssetBundle LoadShaderAssetBundle()
        {
            string path = FilePathUtility.GetAssetBundlePath(AssetType.Shader, "Shader");
            return LoadSingleSync(path);
        }

        public AssetBundle LoadLuaAssetBundle()
        {
            string path = FilePathUtility.GetAssetBundlePath(AssetType.Lua, "lua");
            return LoadSingleSync(path);
        }

        #endregion

        #region AssetBundle Unload

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
                    LogUtil.LogUtility.Print(string.Format("[AssetBundleMgr]Unload {0} AssetBundle {0} Success!", flag, path));
                }
                else
                {
                    assetBundleReference[path] = Count;
                }
            }
        }

        /// <summary>
        /// 通用资源AssetBundle卸载方法[Unload(true)];
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        public void UnloadAsset(AssetType type, string assetName)
        {
            if (type == AssetType.Non || type == AssetType.Shader || type == AssetType.Lua || type == AssetType.Scripts || string.IsNullOrEmpty(assetName))
                return;

            string assetBundleName = FilePathUtility.GetAssetBundleFileName(type, assetName);

            string[] DependentAssetBundle = Manifest.GetAllDependencies(assetBundleName);
            foreach (string tempAssetBundle in DependentAssetBundle)
            {
                if (tempAssetBundle == FilePathUtility.GetAssetBundleFileName(AssetType.Shader, "Shader")) continue;
                string tempPtah = FilePathUtility.AssetBundlePath + tempAssetBundle;
                UnloadAsset(tempPtah, true);
            }
            string assetBundlePath = FilePathUtility.GetAssetBundlePath(type, assetName);
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
            string assetBundlePath = FilePathUtility.GetAssetBundlePath(type, assetName);
            if (assetBundlePath != null)
            {
                UnloadAsset(assetBundlePath, false);
            }
        }

        #endregion

    }
}
