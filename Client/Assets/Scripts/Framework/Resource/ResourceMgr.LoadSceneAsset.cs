/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/7/14 17:52:09
** desc:  场景加载;
*********************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using MEC;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

namespace Framework
{
    public partial class ResourceMgr
    {
        private Dictionary<string, AssetBundle> _sceneAssetBundleDict =
            new Dictionary<string, AssetBundle>();

        #region Scene Load

        /// <summary>
        /// 加载场景;
        /// </summary>
        /// <param name="path"></param>
        public void LoadSceneAsync(string path)
        {
            LoadSceneAsync(path, null, null);
        }

        /// <summary>
        /// 加载场景;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        public void LoadSceneAsync(string path, Action<float> progress)
        {
            LoadSceneAsync(path, null, progress);
        }

        /// <summary>
        /// 加载场景;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onSceneLoaded"></param>
        /// <param name="progress"></param>
        public void LoadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded
        , Action<float> progress)
        {
            CoroutineMgr.Instance.RunCoroutine(LoadSceneFromAssetBundleAsync(path, onSceneLoaded, progress));
        }

        /// 加载场景;
        private IEnumerator<float> LoadSceneFromAssetBundleAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded
        , Action<float> progress)
        {
            AssetBundle assetBundle = null;

            if (_sceneAssetBundleDict.TryGetValue(path, out assetBundle))
            {
                if (assetBundle != null)
                {
                    LogHelper.Print($"[ResourceMgr]LoadSceneFromAssetBundleAsync repeat:{path}.");
                    yield break;
                }
            }

            //此处加载占90%;
            IEnumerator itor = AssetBundleMgr.Instance.LoadFromFileAsync(path,
                bundle => { assetBundle = bundle; }, progress);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }

            _sceneAssetBundleDict[path] = assetBundle;

            var name = Path.GetFileNameWithoutExtension(path);

            //先等一帧;
            yield return Timing.WaitForOneFrame;

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                onSceneLoaded?.Invoke(scene);
            };
            AsyncOperation operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            //此处加载占10%;
            while (operation.progress < 0.99f)
            {
                progress?.Invoke(0.9f + 0.1f * operation.progress);
                yield return Timing.WaitForOneFrame;
            }
            while (!operation.isDone)
            {
                yield return Timing.WaitForOneFrame;
            }

            LogHelper.Print($"[ResourceMgr]LoadSceneFromAssetBundleAsync success:{path}.");
        }

        #endregion

        #region Scene Unload

        /// <summary>
        /// 卸载场景;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onSceneLoaded"></param>
        public void UnloadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded)
        {
            UnloadSceneAsync(path, onSceneLoaded, null);
        }

        /// <summary>
        /// 卸载场景;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onSceneLoaded"></param>
        /// <param name="progress"></param>
        public void UnloadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded
        , Action<float> progress)
        {
            CoroutineMgr.Instance.RunCoroutine(UnloadSceneFromAssetBundleAsync(path, onSceneLoaded, progress));
        }

        /// 卸载场景;
        private IEnumerator<float> UnloadSceneFromAssetBundleAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneUnloaded
        , Action<float> progress)
        {
            var name = Path.GetFileNameWithoutExtension(path);

            SceneManager.sceneUnloaded += (scene) =>
            {
                AssetBundle assetBundle;
                if (_sceneAssetBundleDict.TryGetValue(path, out assetBundle))
                {
                    AssetBundleMgr.Instance.UnloadAsset(path, null);
                }
                onSceneUnloaded?.Invoke(scene);
            };
            AsyncOperation operation = SceneManager.UnloadSceneAsync(name);

            while (operation.progress < 0.99f)
            {
                progress?.Invoke(operation.progress);
                yield return Timing.WaitForOneFrame;
            }
            while (!operation.isDone)
            {
                yield return Timing.WaitForOneFrame;
            }

            LogHelper.Print($"[ResourceMgr]UnloadSceneFromAssetBundleAsync success:{path}.");
        }

        #endregion
    }
}