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
            ResourceAssetProxy proxy = PoolMgr.Instance.GetCsharpObject<ResourceAssetProxy>();
            proxy.Initialize(path);
            Object asset = Resources.Load(path);
            if (asset == null)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadResourceProxy load asset:{0} failure.", path));
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
            ResourceAssetProxy proxy = PoolMgr.Instance.GetCsharpObject<ResourceAssetProxy>();
            proxy.Initialize(path);
            CoroutineMgr.Instance.RunCoroutine(LoadAsync(path, proxy, progress));
            return proxy;
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="proxy"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private IEnumerator<float> LoadAsync(string path, ResourceAssetProxy proxy, Action<float> progress)
        {
            if (string.IsNullOrEmpty(path))
            {
                yield break;
            }
            ResourceRequest request = Resources.LoadAsync(path);
            while (request.progress < 0.99)
            {
                if (progress != null) progress(request.progress);
                yield return Timing.WaitForOneFrame;
            }
            while (!request.isDone)
            {
                yield return Timing.WaitForOneFrame;
            }
            if (null == request.asset)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAsync load asset:{0} failure.", path));
            }

            //先等一帧;
            yield return Timing.WaitForOneFrame;

            if (proxy != null)
            {
                proxy.OnFinish(request.asset);
            }
            else
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAsync proxy is null:{0}.", path));
            }
        }

        #endregion
    }
}
