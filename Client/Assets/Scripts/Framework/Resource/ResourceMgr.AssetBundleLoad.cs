/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 16:12:18
** desc:  AssetBundle资源加载;
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
            if (assetBundle != null)
            {
                var name = Path.GetFileNameWithoutExtension(assetName);
                T tempObject = assetBundle.LoadAsset<T>(name);
                ctrl = loader.GetAsset(tempObject);
            }
            if (ctrl == null)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAssetSync Load Asset {0} failure!", assetName));
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
        /// <returns></returns>
        public IEnumerator<float> LoadAssetAsync<T>(AssetType type, string assetName, Action<T> action)
            where T : Object
        {
            IEnumerator itor = LoadAssetAsync<T>(type, assetName, action, null);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
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
            var name = Path.GetFileNameWithoutExtension(assetName);
            AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(name);
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
            ctrl = loader.GetAsset(request.asset as T);
            if (ctrl == null)
            {
                LogHelper.PrintError(string.Format("[ResourceMgr]LoadAssetAsync Load Asset {0} failure!", assetName));
            }
            else
            {
                if (action != null)
                    action(ctrl);
            }
        }

        #endregion
    }
}
