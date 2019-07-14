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

        //一秒30帧,一帧最久0.33秒;UWA上有建议加载为0.16s合适;
        public readonly float MAX_LOAD_TIME = 0.16f * 1000;

        /// <summary>
        /// 同步从AssetBundle加载资源;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private AssetBundleAssetProxy LoadAssetSync(string path)
        {
            return LoadAssetSync(path, false);
        }

        /// <summary>
        /// 同步从AssetBundle加载资源;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isUsePool"></param>
        /// <returns></returns>
        private AssetBundleAssetProxy LoadAssetSync(string path, bool isUsePool)
        {
            AssetBundleAssetProxy proxy = PoolMgr.Instance.GetCsharpObject<AssetBundleAssetProxy>();
            proxy.Initialize(path, isUsePool);

            Object asset = null;
            AssetBundle assetBundle = AssetBundleMgr.Instance.LoadFromFile(path);
            if (assetBundle != null)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                asset = assetBundle.LoadAsset(name);
            }
            if (asset == null)
            {
                AssetBundleMgr.Instance.UnloadAsset(path, null);
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadSyncAssetProxy load asset:{0} failure.", path));
            }
            else
            {
                AssetBundleMgr.Instance.AddAssetRef(path, asset);
            }
            proxy.OnFinish(asset);
            return proxy;
        }

        /// <summary>
        /// 异步从AssetBundle加载资源;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AssetBundleAssetProxy LoadAssetAsync(string path)
        {
            return LoadAssetAsync(path, null, false);
        }

        /// <summary>
        /// 异步从AssetBundle加载资源;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isUsePool"></param>
        /// <returns></returns>
        public AssetBundleAssetProxy LoadAssetAsync(string path, bool isUsePool)
        {
            return LoadAssetAsync(path, null, isUsePool);
        }

        /// <summary>
        /// 异步从AssetBundle加载资源;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        /// <param name="isUsePool"></param>
        /// <returns></returns>
        public AssetBundleAssetProxy LoadAssetAsync(string path, Action<float> progress, bool isUsePool)
        {
            AssetBundleAssetProxy proxy = PoolMgr.Instance.GetCsharpObject<AssetBundleAssetProxy>();
            proxy.Initialize(path, isUsePool);
            CoroutineMgr.Instance.RunCoroutine(LoadFromFileAsync(path, proxy, progress));
            return proxy;
        }

        /// <summary>
        /// Asset async load from AssetBundle;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetType"></param>
        /// <param name="path"></param>
        /// <param name="proxy"></param>
        /// <param name="action"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private IEnumerator<float> LoadFromFileAsync(string path, AssetBundleAssetProxy proxy
            , Action<float> progress)
        {
            AssetBundle assetBundle = null;

            //此处加载占90%;
            IEnumerator itor = AssetBundleMgr.Instance.LoadFromFileAsync(path,
                bundle => { assetBundle = bundle; }, progress);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            var name = Path.GetFileNameWithoutExtension(path);
            AssetBundleRequest request = assetBundle.LoadAssetAsync(name);

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
            if (null == request.asset)
            {
                AssetBundleMgr.Instance.UnloadAsset(path, null);
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadFromFileAsync load asset:{0} failure.", path));
            }
            else
            {
                AssetBundleMgr.Instance.AddAssetRef(path, request.asset);
            }

            //先等一帧;
            yield return Timing.WaitForOneFrame;

            if (proxy != null)
            {
                proxy.OnFinish(request.asset);
            }
            else
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadFromFileAsync proxy is null:{0}.", path));
            }
        }

        #endregion
    }
}
