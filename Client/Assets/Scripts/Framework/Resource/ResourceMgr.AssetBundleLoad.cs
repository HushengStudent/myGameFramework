/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 16:12:18
** desc:  AssetBundle资源加载;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MEC;
using Object = UnityEngine.Object;
using System.IO;

namespace Framework
{
    public partial class ResourceMgr
    {
        #region AssetBundle Load

        private void UpdateLoadAssetAsync()
        {

        }

        /// <summary>
        /// Asset sync load from AssetBundle;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>ctrl</returns>
        public T LoadAssetSync<T>(AssetType assetType, string assetName) where T : Object
        {
            T ctrl = null;
            AssetBundle assetBundle = AssetBundleMgr.Instance.LoadAssetBundleSync(assetType, assetName);
            if (assetBundle != null)
            {
                var name = Path.GetFileNameWithoutExtension(assetName);
                T tempObject = assetBundle.LoadAsset<T>(name);
                ctrl = AssetLoader.GetAsset(assetType, tempObject);
            }
            if (ctrl == null)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAssetSync Load Asset failure" +
                    ",type:{0},name:{1}!", assetType, assetName));
            }
            return ctrl;
        }

        /// <summary>
        /// Asset异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>代理</returns>
        public AssetAsyncProxy LoadAssetProxy<T>(AssetType assetType, string assetName) where T : Object
        {
            return LoadAssetProxy<T>(assetType, assetName, null, null);

        }

        /// <summary>
        /// Asset异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <returns>代理</returns>
        public AssetAsyncProxy LoadAssetProxy<T>(AssetType assetType, string assetName
            , Action<T> action) where T : Object
        {
            return LoadAssetProxy<T>(assetType, assetName, action, null);
        }

        /// <summary>
        /// Asset异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns>代理</returns>
        public AssetAsyncProxy LoadAssetProxy<T>(AssetType assetType, string assetName
            , Action<T> action, Action<float> progress) where T : Object
        {
            AssetAsyncProxy proxy = PoolMgr.Instance.GetObject<AssetAsyncProxy>();
            proxy.InitProxy(assetType, assetName);
            CoroutineMgr.Instance.RunCoroutine(LoadAssetAsync<T>(assetType, assetName, proxy, action, progress));
            return proxy;
        }

        /// <summary>
        /// Asset async load from AssetBundle;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="proxy">代理</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        private IEnumerator<float> LoadAssetAsync<T>(AssetType assetType, string assetName, AssetAsyncProxy proxy
            , Action<T> action, Action<float> progress)
            where T : Object
        {
            T ctrl = null;
            AssetBundle assetBundle = null;

            IEnumerator itor = AssetBundleMgr.Instance.LoadAssetBundleAsync(assetType, assetName,
                ab => { assetBundle = ab; }, progress);//此处加载占90%;
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            var name = Path.GetFileNameWithoutExtension(assetName);
            AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(name);
            //此处加载占10%;
            while (request.progress < 0.99)
            {
                if (progress != null)
                {
                    progress(0.9f + 0.1f * request.progress);
                }
                yield return Timing.WaitForOneFrame;
            }
            while (!request.isDone)
            {
                yield return Timing.WaitForOneFrame;
            }
            ctrl = AssetLoader.GetAsset(assetType, request.asset as T);
            if (null == ctrl)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAssetAsync Load Asset failure," +
                    ",type:{0},name:{1}!", assetType, assetName));
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







        //=======================================================Discarded=======================================================

        /// <summary>
        /// Asset异步加载;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns>代理</returns>
        [Obsolete("warning,this method is discarded!")]
        private AssetAsyncProxy LoadAssetProxy_discard<T>(AssetType assetType, string assetName
            , Action<T> action, Action<float> progress) where T : Object
        {
            AssetAsyncProxy proxy = PoolMgr.Instance.GetObject<AssetAsyncProxy>();
            proxy.InitProxy(assetType, assetName);
            CoroutineMgr.Instance.RunCoroutine(LoadAssetAsync_discard<T>(assetType, assetName, proxy, action, progress));
            return proxy;
        }

        /// <summary>
        /// Asset async load from AssetBundle;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="proxy">代理</param>
        /// <param name="action">资源回调</param>
        /// <param name="progress">progress回调</param>
        /// <returns></returns>
        private IEnumerator<float> LoadAssetAsync_discard<T>(AssetType assetType, string assetName, AssetAsyncProxy proxy
            , Action<T> action, Action<float> progress)
            where T : Object
        {
            T ctrl = null;
            AssetBundle assetBundle = null;

            IEnumerator itor = AssetBundleMgr.Instance.LoadAssetBundleAsync(assetType, assetName,
                ab => { assetBundle = ab; }, progress);//此处加载占90%;
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            var name = Path.GetFileNameWithoutExtension(assetName);
            AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(name);
            //此处加载占10%;
            while (request.progress < 0.99)
            {
                if (progress != null)
                {
                    progress(0.9f + 0.1f * request.progress);
                }
                yield return Timing.WaitForOneFrame;
            }
            while (!request.isDone)
            {
                yield return Timing.WaitForOneFrame;
            }
            ctrl = AssetLoader.GetAsset(assetType, request.asset as T);
            if (null == ctrl)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAssetAsync Load Asset failure," +
                    ",type:{0},name:{1}!", assetType, assetName));
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
