/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 16:12:18
** desc:  Resource资源加载;
*********************************************************************************/

using UnityEngine;
using System;
using Framework.ObjectPoolModule;
using System.Collections;

namespace Framework.ResourceModule
{
    /// <summary>
    /// 调Resources.LoadAsync加载的同一帧,调Resources.Load加载同一资源会报错;
    /// </summary>
    public partial class ResourceMgr
    {
        #region Resources Load

        /// <summary>
        /// Resource同步加载;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ResourceAssetProxy LoadResourceAssetSync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var proxy = PoolMgr.singleton.GetCsharpObject<ResourceAssetProxy>();
            proxy.Initialize(path);
            var asset = Resources.Load(path);
            if (asset == null)
            {
                LogHelper.PrintError($"[ResourceMgr]LoadResourceProxy load asset:{path} failure.");
            }
            proxy.OnFinish(asset);
            return proxy;
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private ResourceAssetProxy LoadResourceAssetAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return LoadResourceAssetAsync(path, null);
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private ResourceAssetProxy LoadResourceAssetAsync(string path, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var proxy = PoolMgr.singleton.GetCsharpObject<ResourceAssetProxy>();
            proxy.Initialize(path);
            CoroutineMgr.singleton.RunCoroutine(LoadResourceAssetAsync(path, proxy, progress));
            return proxy;
        }

        /// Resource异步加载;
        private IEnumerator LoadResourceAssetAsync(string path, ResourceAssetProxy proxy, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                yield break;
            }
            var exception = PoolMgr.singleton.GetCsharpObject<GameException>();
            var request = Resources.LoadAsync(path);
            while (request.progress < 0.99f)
            {
                progress?.Invoke(request.progress);
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
            while (!request.isDone)
            {
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
            if (null == request.asset)
            {
                exception.AppendMsg($"[ResourceMgr]LoadAsync load asset:{path} failure.");
            }

            //先等一帧;
            yield return CoroutineMgr.WaitForEndOfFrame;

            if (proxy != null)
            {
                proxy.OnFinish(request.asset);
            }
            else
            {
                exception.AppendMsg($"[ResourceMgr]LoadAsync proxy is null:{path}.");
            }

            if (exception.IsActive)
            {
                throw exception;
            }
            else
            {
                PoolMgr.singleton.ReleaseCsharpObject(exception);
            }
        }

        #endregion
    }
}
