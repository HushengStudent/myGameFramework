/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 16:12:18
** desc:  Resource资源加载;
*********************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using MEC;
using Object = UnityEngine.Object;

namespace Framework
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
        public ResourceAssetProxy LoadResourceProxy(string path)
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
        public ResourceAssetProxy LoadResourceAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return LoadResourceAsync(path, null);
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public ResourceAssetProxy LoadResourceAsync(string path, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var proxy = PoolMgr.singleton.GetCsharpObject<ResourceAssetProxy>();
            proxy.Initialize(path);
            CoroutineMgr.singleton.RunCoroutine(LoadAsync(path, proxy, progress));
            return proxy;
        }

        /// Resource异步加载;
        private IEnumerator<float> LoadAsync(string path, ResourceAssetProxy proxy, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                yield break;
            }
            var request = Resources.LoadAsync(path);
            while (request.progress < 0.99f)
            {
                progress?.Invoke(request.progress);
                yield return Timing.WaitForOneFrame;
            }
            while (!request.isDone)
            {
                yield return Timing.WaitForOneFrame;
            }
            if (null == request.asset)
            {
                LogHelper.PrintError($"[ResourceMgr]LoadAsync load asset:{path} failure.");
            }

            //先等一帧;
            yield return Timing.WaitForOneFrame;

            if (proxy != null)
            {
                proxy.OnFinish(request.asset);
            }
            else
            {
                LogHelper.PrintError($"[ResourceMgr]LoadAsync proxy is null:{path}.");
            }
        }

        #endregion
    }
}
