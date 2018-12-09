/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 16:12:18
** desc:  Resource资源加载;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using MEC;
using System.IO;

namespace Framework
{
    public partial class ResourceMgr
    {
        #region Resources Load

        /// <summary>
        /// Resource同步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>ctrl</returns>
        public T LoadResourceSync<T>(AssetType assetType, string assetName) where T : Object
        {
            string path = FilePathHelper.GetResourcePath(assetType, assetName);
            IAssetLoader<T> loader = CreateLoader<T>(assetType);
            if (path != null)
            {
                //Resources.Load加载同一资源,只会有一份Asset,需要实例化的资源可以Instantiate多个对象;
                T ctrl = Resources.Load<T>(path);
                if (ctrl != null)
                {
                    return loader.GetAsset(ctrl);
                }
            }
            LogHelper.PrintError(string.Format("[ResourceMgr]LoadResourceSync Load Asset {0} failure!",
                assetName + "." + assetType.ToString()));
            return null;
        }

        /// <summary>
        /// Resource同步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <returns></returns>
        public ResourceLoadProxy LoadResourceProxyAsync<T>(AssetType assetType, string assetName, Action<T> action) where T : Object
        {
            return LoadResourceProxyAsync<T>(assetType, assetName, action, null);
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">进度回调</param>
        /// <returns></returns>
        public ResourceLoadProxy LoadResourceProxyAsync<T>(AssetType assetType, string assetName
            , Action<T> action, Action<float> progress) where T : Object
        {
            ResourceLoadProxy proxy = PoolMgr.Instance.Get<ResourceLoadProxy>();
            proxy.InitProxy(assetType, assetName);
            CoroutineMgr.Instance.RunCoroutine(LoadResourceAsync<T>(assetType, assetName, proxy, action, progress));
            return proxy;
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">进度回调</param>
        /// <returns></returns>
        private IEnumerator<float> LoadResourceAsync<T>(AssetType assetType, string assetName, ResourceLoadProxy proxy
            , Action<T> action, Action<float> progress) where T : Object
        {
            string path = FilePathHelper.GetResourcePath(assetType, assetName);
            IAssetLoader<T> loader = CreateLoader<T>(assetType);

            T ctrl = null;
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
                ctrl = loader.GetAsset(request.asset as T);
            }
            if (null == ctrl)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadResourceAsync Load Asset {0} failure!",
                    assetName + "." + assetType.ToString()));
            }
            if (!proxy.isCancel && action != null)
            {
                action(ctrl);
            }
            if (proxy != null)
            {
                proxy.OnFinish(ctrl);
            }
        }

        #endregion

        #region Unload Assets

        public void UnloadObject(AssetType assetType, Object asset)
        {
            if (asset != null)
            {
                if (assetType == AssetType.Prefab)
                {
                    GameObject go = asset as GameObject;
                    if (go)
                    {
                        Destroy(go);
                        return;
                    }
                    MonoBehaviour monoBehaviour = (MonoBehaviour)asset;
                    if (monoBehaviour != null)
                    {
                        Destroy(monoBehaviour.gameObject);
                        return;
                    }
                }
                if (assetType == AssetType.AnimeClip || assetType == AssetType.AnimeCtrl
                    || assetType == AssetType.Audio || assetType == AssetType.Material
                    || assetType == AssetType.Texture)
                {
                    UnloadObject(asset);
                    return;
                }
                if (assetType == AssetType.Scripts)
                {
                    Destroy(asset);
                }
            }
        }

        /// <summary>
        /// 卸载不需实例化的资源(纹理,Animator);
        /// 卸载非GameObject类型的资源,会将内存中已加载资源及其克隆体卸载;
        /// </summary>
        /// <param name="asset"></param>
        public void UnloadObject(Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        #endregion
    }
}
