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
using System.Diagnostics;

namespace Framework
{
    public partial class ResourceMgr
    {
        #region AssetBundle Load

        //一秒30帧,一帧最久0.33秒;UWA上有建议加载为0.16s合适;
        public readonly float MAX_LOAD_TIME = 0.16f * 1000;

        //资源加载队列;
        private Queue<AsyncAssetProxy> _asyncProxyQueue = new Queue<AsyncAssetProxy>();
        private Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        //当前加载代理;
        private AsyncAssetProxy _curProxy = null;

        private void UpdateLoadAssetAsync()
        {
            if (_asyncProxyQueue.Count > 0 || null != _curProxy)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                while (true)
                {
                    if (_asyncProxyQueue.Count < 1 && null == _curProxy)
                    {
                        _stopwatch.Stop();
                        break;
                    }
                    if (_asyncProxyQueue.Count > 0)
                    {
                        if (null == _curProxy)
                            _curProxy = _asyncProxyQueue.Dequeue();
                    }
                    if (_curProxy.LoadNode.NodeState == AssetBundleLoadNode.AssetBundleNodeState.Finish)
                    {
                        LoadObjectFromAssetBundleLoadNode(_curProxy);
                        _curProxy = null;
                    }
                    else
                    {
                        _curProxy.LoadNode.Update();//结束了就等下一帧执行回调;
                    }
                    if (_stopwatch.Elapsed.Milliseconds >= MAX_LOAD_TIME)
                    {
                        _stopwatch.Stop();
                        break;
                    }
                }
            }
        }

        private void LoadObjectFromAssetBundleLoadNode(AsyncAssetProxy proxy)
        {
            Object target = null;
            AssetBundle assetBundle = proxy.LoadNode.assetBundle;
            target = PoolMgr.Instance.GetUnityAsset(proxy.assetType, proxy.assetName);
            if (null == target)
            {
                var name = Path.GetFileNameWithoutExtension(proxy.assetName);
                target = assetBundle.LoadAsset(name);
            }
            if (target != null)
            {
                PoolMgr.Instance.ReleaseUnityAsset(proxy.assetType, proxy.assetName, target);
                target = PoolMgr.Instance.GetUnityAsset(proxy.assetType, proxy.assetName);
            }
            proxy.OnFinish(target);
        }

        /// <summary>
        /// 同步从AssetBundle加载资源;
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>同步代理</returns>
        public SyncAssetProxy LoadAssetSync(AssetType assetType, string assetName)
        {
            return LoadAssetSync(assetType, assetName, true);
        }

        /// <summary>
        /// 同步从AssetBundle加载资源;
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="isUsePool">资源是否使用对象池</param>
        /// <returns>同步代理</returns>
        public SyncAssetProxy LoadAssetSync(AssetType assetType, string assetName, bool isUsePool)
        {
            SyncAssetProxy proxy = PoolMgr.Instance.GetCsharpObject<SyncAssetProxy>();
            proxy.InitProxy(assetType, assetName, isUsePool);

            Object ctrl = null;
            AssetBundle assetBundle = AssetBundleMgr.Instance.LoadAssetBundleSync(assetType, assetName);
            if (assetBundle != null)
            {
                var name = Path.GetFileNameWithoutExtension(assetName);
                ctrl = assetBundle.LoadAsset(name);
            }
            if (ctrl == null)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAssetSync Load Asset failure" +
                    ",type:{0},name:{1}!", assetType, assetName));
            }
            proxy.OnFinish(ctrl);
            return proxy;
        }

        /// <summary>
        /// 异步从AssetBundle加载资源;
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <returns>异步代理</returns>
        public AsyncAssetProxy LoadAssetProxy(AssetType assetType, string assetName)
        {
            return LoadAssetProxy(assetType, assetName, null, true);
        }

        /// <summary>
        /// 异步从AssetBundle加载资源;
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="isUsePool">资源是否使用对象池</param>
        /// <returns>异步代理</returns>
        public AsyncAssetProxy LoadAssetProxy(AssetType assetType, string assetName, bool isUsePool)
        {
            return LoadAssetProxy(assetType, assetName, null, isUsePool);
        }

        /// <summary>
        /// 异步从AssetBundle加载资源;
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="progress">加载进度</param>
        /// <param name="isUsePool">资源是否使用对象池</param>
        /// <returns>异步代理</returns>
        public AsyncAssetProxy LoadAssetProxy(AssetType assetType, string assetName, Action<float> progress, bool isUsePool)
        {
            AsyncAssetProxy proxy = PoolMgr.Instance.GetCsharpObject<AsyncAssetProxy>();
            AssetBundleLoadNode loadNode = AssetBundleMgr.Instance.GetAssetBundleLoadNode(assetType, assetName);
            loadNode.AddLoadProgressCallBack(progress);
            proxy.InitProxy(assetType, assetName, loadNode, isUsePool);
            _asyncProxyQueue.Enqueue(proxy);
            return proxy;
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
        private AsyncAssetProxy LoadAssetProxy_discard<T>(AssetType assetType, string assetName
            , Action<T> action, Action<float> progress) where T : Object
        {
            AsyncAssetProxy proxy = PoolMgr.Instance.GetCsharpObject<AsyncAssetProxy>();
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
        private IEnumerator<float> LoadAssetAsync_discard<T>(AssetType assetType, string assetName, AsyncAssetProxy proxy
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
            ctrl = request.asset as T;
            if (null == ctrl)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAssetAsync Load Asset failure," +
                    ",type:{0},name:{1}!", assetType, assetName));
            }
            //--------------------------------------------------------------------------------------
            //先等一帧;
            yield return Timing.WaitForOneFrame;

            if (!proxy.IsCancel && action != null)
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
