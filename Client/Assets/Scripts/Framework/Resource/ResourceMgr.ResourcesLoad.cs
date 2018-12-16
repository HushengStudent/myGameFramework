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
                    return AssetLoader.GetAsset(assetType, ctrl);
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
            ResourceAsyncProxy proxy = PoolMgr.Instance.Get<ResourceAsyncProxy>();
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

            //--------------------------------------------------------------------------------------
            //正在加载的数量超出最大值,等下一帧吧;
            if (AsyncMgr.CurCount() > AsyncMgr.ASYNC_LOAD_MAX_VALUE)
                yield return Timing.WaitForOneFrame;

            var loadID = AsyncMgr.LoadID;
            AsyncMgr.Add(loadID);
            //--------------------------------------------------------------------------------------

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
                ctrl = AssetLoader.GetAsset(assetType, request.asset as T);
            }
            if (null == ctrl)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadResourceAsync Load Asset {0} failure!",
                    assetName + "." + assetType.ToString()));
            }
            //--------------------------------------------------------------------------------------
            //先等一帧;
            yield return Timing.WaitForOneFrame;
            var finishTime = AsyncMgr.GetCurTime();
            var timeOver = false;
            var isloading = AsyncMgr.IsContains(loadID);
            while (isloading && !timeOver && AsyncMgr.CurLoadID != loadID)
            {
                timeOver = AsyncMgr.IsTimeOverflows(finishTime);
                if (timeOver)
                {
                    LogHelper.PrintWarning(string.Format("[ResourceMgr]LoadResourceAsync excute callback over time, type:{0},name{1}."
                        , assetType, assetName));
                    break;
                }
                yield return Timing.WaitForOneFrame;
            }
            //--------------------------------------------------------------------------------------
            if (!proxy.isCancel && action != null)
            {
                action(ctrl);
            }
            if (proxy != null)
            {
                proxy.OnFinish(ctrl);
            }
            //--------------------------------------------------------------------------------------
            if (!isloading)
            {
                yield break;
            }
            if (timeOver && AsyncMgr.CurLoadID != loadID)
            {
                AsyncMgr.Remove(loadID);

                if (AsyncMgr.CurLoadTimeOverflows())
                {
                    AsyncMgr.CurLoadID = 0;
                }
            }
            else
            {
                AsyncMgr.CurLoadID = 0;
            }
            //--------------------------------------------------------------------------------------
        }

        #endregion
    }
}
