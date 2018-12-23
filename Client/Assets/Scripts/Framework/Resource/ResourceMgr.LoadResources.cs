/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 16:12:18
** desc:  Resource资源加载;
*********************************************************************************/

using UnityEngine;
using System.Collections;
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
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>ctrl</returns>
        public T LoadResourceSync<T>(AssetType assetType, string assetName) where T : Object
        {
            string path = FilePathHelper.GetResourcePath(assetType, assetName);
            if (path != null)
            {
                //Resources.Load加载同一资源,只会有一份Asset,需要实例化的资源可以Instantiate多个对象;
                T ctrl = Resources.Load<T>(path);
                if (ctrl != null)
                {
                    return ctrl;
                }
            }
            LogHelper.PrintError(string.Format("[ResourceMgr]LoadResourceSync Load Asset {0} failure!",
                assetName + "." + assetType.ToString()));
            return null;
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>代理</returns>
        public ResourceAsyncProxy LoadResourceProxy<T>(AssetType assetType, string assetName) where T : Object
        {
            return LoadResourceProxy<T>(assetType, assetName, null, null);
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <returns>代理</returns>
        public ResourceAsyncProxy LoadResourceProxy<T>(AssetType assetType, string assetName
            , Action<T> action) where T : Object
        {
            return LoadResourceProxy<T>(assetType, assetName, action, null);
        }

        /// <summary>
        /// Resource异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns>代理</returns>
        public ResourceAsyncProxy LoadResourceProxy<T>(AssetType assetType, string assetName
            , Action<T> action, Action<float> progress) where T : Object
        {
            ResourceAsyncProxy proxy = PoolMgr.Instance.GetObject<ResourceAsyncProxy>();
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
        /// <param name="proxy">代理</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        private IEnumerator<float> LoadResourceAsync<T>(AssetType assetType, string assetName, ResourceAsyncProxy proxy
            , Action<T> action, Action<float> progress) where T : Object
        {
            string path = FilePathHelper.GetResourcePath(assetType, assetName);
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
                ctrl = request.asset as T;
            }
            if (null == ctrl)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadResourceAsync Load Asset {0} failure!",
                    assetName + "." + assetType.ToString()));
            }
            //--------------------------------------------------------------------------------------
            //先等一帧;
            yield return Timing.WaitForOneFrame;

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
    }
}
