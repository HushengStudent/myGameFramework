/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:56
** desc:  资源管理;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MEC;

namespace Framework
{
    public class ResourceMgr : Singleton<ResourceMgr>, IManagr
    {
        #region Functions

        /// <summary>
        /// 初始化;
        /// </summary>
        public void InitEx()
        {
            //UnloadUnusedAssets();
            //GameGC();
            InitLua();
            InitShader();
            LogUtil.LogUtility.Print("[ResourceMgr]ResourceMgr init!");
        }

        /// <summary>
        /// 初始化Shader;
        /// </summary>
        private void InitShader()
        {
            //Shader初始化;
            AssetBundle shaderAssetBundle = AssetBundleMgr.Instance.LoadShaderAssetBundle();
            if (shaderAssetBundle != null)
            {
                shaderAssetBundle.LoadAllAssets();
                Shader.WarmupAllShaders();
                LogUtil.LogUtility.Print("[ResourceMgr]Load Shader and WarmupAllShaders Success!");
            }
            else
            {
                LogUtil.LogUtility.PrintError("[ResourceMgr]Load Shader and WarmupAllShaders failure!");
            }
            //AssetBundleMgr.Instance.UnloadMirroring(AssetType.Shader, "Shader");
        }

        /// <summary>
        /// 初始化Lua;
        /// </summary>
        private void InitLua()
        {
            //TODO...
        }

        /// <summary>
        /// 清理;
        /// </summary>
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// GC;
        /// </summary>
        public void GameGC()
        {
            System.GC.Collect();
        }

        /// <summary>
        /// 卸载不需实例化的资源(纹理,Animator);
        /// </summary>
        /// <param name="asset"></param>
        public void UnloadObject(Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        /// <summary>
        /// Clone GameObject;
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private GameObject CloneGameObject(GameObject go)
        {
            GameObject target = GameObject.Instantiate(go);
            AssetBundleTag tag = target.GetComponent<AssetBundleTag>();
            if (tag)
                tag.IsClone = true;
            return target;
        }

        /// <summary>
        /// 创建资源加载器;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <returns>资源加载器;</returns>
        private IAssetLoader<T> CreateLoader<T>(AssetType assetType) where T : Object
        {
            if (assetType == AssetType.Prefab || assetType == AssetType.Model) return new ResLoader<T>();
            return new AssetLoader<T>();
        }

        #endregion

        #region Resources Load

        /// <summary>
        /// Resource同步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>ctrl</returns>
        public T LoadResSync<T>(AssetType type, string assetName) where T : Object
        {
            string path = FilePathUtility.GetResourcePath(type, assetName);
            IAssetLoader<T> loader = CreateLoader<T>(type);
            bool isInstance = false;
            if (path != null)
            {
                T ctrl = Resources.Load<T>(path);
                if (ctrl != null)
                {
                    return loader.GetAsset(ctrl, out isInstance);
                }
            }
            LogUtil.LogUtility.PrintError(string.Format("[ResourceMgr]LoadResSync Load Asset {0} failure!", assetName + "." + type.ToString()));
            return null;
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns></returns>
        public IEnumerator<float> LoadResAsync<T>(AssetType type, string assetName) where T : Object
        {
            IEnumerator itor = LoadResAsync<T>(type, assetName, null, null);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <returns></returns>
        public IEnumerator<float> LoadResAsync<T>(AssetType type, string assetName, Action<T> action) where T : Object
        {
            IEnumerator itor = LoadResAsync<T>(type, assetName, action, null);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">进度回调</param>
        /// <returns></returns>
        public IEnumerator<float> LoadResAsync<T>(AssetType type, string assetName, Action<T> action, Action<float> progress) where T : Object
        {
            string path = FilePathUtility.GetResourcePath(type, assetName);
            IAssetLoader<T> loader = CreateLoader<T>(type);

            T ctrl = null;
            bool isInstance = false;
            if (path != null)
            {
                ResourceRequest request = Resources.LoadAsync<T>(path);
                while (request.progress < 0.99)
                {
                    if (progress != null) progress(request.progress);
                    yield return Timing.WaitForOneFrame;
                }
                while (!request.isDone)
                {
                    yield return Timing.WaitForOneFrame;
                }
                ctrl = loader.GetAsset(request.asset as T, out isInstance);
            }
            if (action != null)
            {
                action(ctrl);
            }
            else
            {
                LogUtil.LogUtility.PrintError(string.Format("[ResourceMgr]LoadResAsync Load Asset {0} failure!", assetName + "." + type.ToString()));
            }
        }

        #endregion 

        #region AssetBundle Load

        /// <summary>
        /// Asset sync load from AssetBundle;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>ctrl</returns>
        public T LoadAssetSync<T>(AssetType type, string assetName) where T : Object
        {
            T ctrl = null;
            IAssetLoader<T> loader = CreateLoader<T>(type);
            AssetBundle assetBundle = AssetBundleMgr.Instance.LoadAssetBundleSync(type, assetName);
            bool isInstance = false;
            if (assetBundle != null)
            {
                T tempObject = assetBundle.LoadAsset<T>(assetName);
                ctrl = loader.GetAsset(tempObject, out isInstance);
            }
            if (ctrl == null)
            {
                LogUtil.LogUtility.PrintError(string.Format("[ResourceMgr]LoadAssetFromAssetBundleSync Load Asset {0} failure!", assetName + "." + type.ToString()));
            }
            else
            {
                if (isInstance)
                {
                    var go = ctrl as GameObject;
                    if (go)
                    {
                        UnityUtility.AddOrGetComponent<AssetBundleTag>(go, (tag) =>//自动卸载;
                        {
                            tag.AssetBundleName = assetName;
                            tag.Type = type;
                            tag.IsClone = false;
                        });
                    }
                }
            }
            return ctrl;
        }

        /// <summary>
        /// Asset async load from AssetBundle;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="type">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        public IEnumerator<float> LoadAssetAsync<T>(AssetType type, string assetName, Action<T> action, Action<float> progress)
            where T : Object
        {
            T ctrl = null;
            AssetBundle assetBundle = null;
            IAssetLoader<T> loader = CreateLoader<T>(type);

            IEnumerator itor = AssetBundleMgr.Instance.LoadAssetBundleAsync(type, assetName,
                ab =>
                {
                    assetBundle = ab;
                },
                null);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }

            AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(assetName);
            while (request.progress < 0.99)
            {
                if (progress != null)
                    progress(request.progress);
                yield return Timing.WaitForOneFrame;
            }
            while (!request.isDone)
            {
                yield return Timing.WaitForOneFrame;
            }
            bool isInstance = false;
            ctrl = loader.GetAsset(request.asset as T, out isInstance);
            if (ctrl == null)
            {
                LogUtil.LogUtility.PrintError(string.Format("[ResourceMgr]LoadAssetFromAssetBundleSync Load Asset {0} failure!", assetName + "." + type.ToString()));
            }
            else
            {
                if (isInstance)
                {
                    var go = ctrl as GameObject;
                    if (go)
                    {
                        UnityUtility.AddOrGetComponent<AssetBundleTag>(go, (tag) =>//自动卸载;
                        {
                            tag.AssetBundleName = assetName;
                            tag.Type = type;
                            tag.IsClone = false;
                        });
                    }
                }
                if (action != null)
                    action(ctrl);
            }
        }

        #endregion
    }
}
