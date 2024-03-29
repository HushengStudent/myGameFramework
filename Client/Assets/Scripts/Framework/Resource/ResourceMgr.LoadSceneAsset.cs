/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/7/14 17:52:09
** desc:  场景加载;
*********************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using Framework.AssetBundleModule;
using System.Collections;

namespace Framework.ResourceModule
{
    public partial class ResourceMgr
    {
        private readonly Dictionary<string, AssetBundle> _sceneAssetBundleDict =
            new Dictionary<string, AssetBundle>();

        private ISceneLoader _sceneLoader;
        private ISceneLoader SceneLoader
        {
            get
            {
                if (_sceneLoader == null)
                {
                    if (GameMgr.AssetBundleModel)
                    {
                        _sceneLoader = new AssetBundleSceneLoader();
                    }
                    else
                    {
                        _sceneLoader = new EditorSceneLoader();
                    }
                }
                return _sceneLoader;
            }
        }

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
            CoroutineMgr.singleton.RunCoroutine(LoadScene(path, onSceneLoaded, progress));
        }

        /// 加载场景;
        private IEnumerator LoadScene(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded
        , Action<float> progress)
        {
            path = $"Assets/Bundles/{path}";
            var itor = SceneLoader.LoadSceneAsync(path, onSceneLoaded, progress);
            while (itor.MoveNext())
            {
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
            LogHelper.Print($"[ResourceMgr]LoadSceneAsync success:{path}.");
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
            CoroutineMgr.singleton.RunCoroutine(UnloadScene(path, onSceneLoaded, progress));
        }

        /// 卸载场景;
        private IEnumerator UnloadScene(string path, Action<UnityEngine.SceneManagement.Scene> onSceneUnloaded
        , Action<float> progress)
        {
            var name = Path.GetFileNameWithoutExtension(path);

            SceneLoader.UnloadSceneAsync(path, onSceneUnloaded, progress);
            var op = SceneManager.UnloadSceneAsync(name);
            while (op.progress < 0.99f)
            {
                progress?.Invoke(op.progress);
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
            while (!op.isDone)
            {
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
            LogHelper.Print($"[ResourceMgr]UnloadSceneAsync success:{path}.");
        }

        #endregion

        #region Loader

        private interface ISceneLoader
        {
            IEnumerator LoadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded
        , Action<float> progress);

            void UnloadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneUnloaded
        , Action<float> progress);

        }

        internal class AssetBundleSceneLoader : ISceneLoader
        {
            public IEnumerator LoadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded, Action<float> progress)
            {
                if (singleton._sceneAssetBundleDict.TryGetValue(path, out var assetBundle))
                {
                    if (assetBundle != null)
                    {
                        LogHelper.Print($"[ResourceMgr]LoadSceneFromAssetBundleAsync repeat:{path}.");
                        yield break;
                    }
                }

                //此处加载占90%;
                var itor = AssetBundleMgr.singleton.LoadFromFileAsync(path, bundle => { assetBundle = bundle; }, progress);
                while (itor.MoveNext())
                {
                    yield return CoroutineMgr.WaitForEndOfFrame;
                }

                singleton._sceneAssetBundleDict[path] = assetBundle;

                var name = Path.GetFileNameWithoutExtension(path);

                //先等一帧;
                yield return CoroutineMgr.WaitForEndOfFrame;

                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    onSceneLoaded?.Invoke(scene);
                };
                var op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

                //此处加载占10%;
                while (op.progress < 0.99f)
                {
                    progress?.Invoke(0.9f + 0.1f * op.progress);
                    yield return CoroutineMgr.WaitForEndOfFrame;
                }
                while (!op.isDone)
                {
                    yield return CoroutineMgr.WaitForEndOfFrame;
                }
            }

            public void UnloadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneUnloaded, Action<float> progress)
            {
                SceneManager.sceneUnloaded += (scene) =>
                {
                    if (singleton._sceneAssetBundleDict.TryGetValue(path, out var assetBundle))
                    {
                        AssetBundleMgr.singleton.UnloadAsset(path, null);
                    }
                    onSceneUnloaded?.Invoke(scene);
                };
            }
        }

        internal class EditorSceneLoader : ISceneLoader
        {
            public IEnumerator LoadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneLoaded, Action<float> progress)
            {
                //先等一帧;
                yield return CoroutineMgr.WaitForEndOfFrame;

                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    onSceneLoaded?.Invoke(scene);
                };

#if UNITY_EDITOR

                //var op = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(path);
                var op = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters()
                {
                    loadSceneMode = LoadSceneMode.Single,
                    localPhysicsMode = LocalPhysicsMode.Physics3D
                });
                while (op.progress < 0.99f)
                {
                    progress?.Invoke(op.progress);
                    yield return CoroutineMgr.WaitForEndOfFrame;
                }
                while (!op.isDone)
                {
                    yield return CoroutineMgr.WaitForEndOfFrame;
                }
#endif
            }

            public void UnloadSceneAsync(string path, Action<UnityEngine.SceneManagement.Scene> onSceneUnloaded, Action<float> progress)
            {
                SceneManager.sceneUnloaded += (scene) =>
                {
                    onSceneUnloaded?.Invoke(scene);
                };
            }
        }

        #endregion
    }
}