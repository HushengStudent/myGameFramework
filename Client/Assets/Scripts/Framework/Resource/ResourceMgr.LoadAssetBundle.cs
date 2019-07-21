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

        /// 一秒30帧,一帧最久0.33秒;UWA上有建议加载为0.16s合适;
        public readonly float MAX_LOAD_TIME = 0.16f * 1000;

        public readonly float LOAD_BUNDLE_PRECENT = 0.8f;

        public readonly float LOAD_ASSET_PRECENT = 0.2f;

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

        /// Asset async load from AssetBundle;
        private IEnumerator<float> LoadFromFileAsync(string path, AssetBundleAssetProxy proxy
            , Action<float> progress)
        {
            AssetBundle assetBundle = null;

            //此处加载占0.8;
            IEnumerator itor = AssetBundleMgr.Instance.LoadFromFileAsync(path,
                bundle => { assetBundle = bundle; }, progress);
            while (itor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            var name = Path.GetFileNameWithoutExtension(path);
            AssetBundleRequest request = assetBundle.LoadAssetAsync(name);

            //此处加载占0.2;
            while (request.progress < 0.99f)
            {
                if (progress != null)
                {
                    progress(LOAD_BUNDLE_PRECENT + LOAD_ASSET_PRECENT * request.progress);
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

/*

结论:1、多次从AssetBundle加载同一个资源,返回的资源是同一个资源实例;

2、Resources.UnloadUnusedAssets();当资源被MonoBehavior脚本非局部变量,静态变量等持有的时候,该接口不能销毁该资源;
An asset is deemed to be unused if it isn't reached after walking the whole game object hierarchy,
including script components. Static variables are also examined;
如果它没有达到遍历整个游戏对象层级之后,一个资源被视为卸载,包含脚本组件,静态变量也被检查;

遍历对象InstaceID到指针的全局表,收集仍未销毁的Object对象到资源回收表中;
在资源回收表中查找所有仍挂接在场景中的根节点对象,并递归遍历其下引用的所有Object对象,将其标记为被引用对象;
遍历资源回收表,卸载表中所有不存在任何引用的对象;

3、Resources.UnloadAsset();The referenced asset(assetToUnload) will be unloaded from memory.The object will become invalid and can't be loaded back from disk.
Any subsequently loaded scenes or assets that reference the asset on disk will cause a new instance of the object to be loaded from disk.
This new instance will not be connected to the previously unloaded object;

4、Resources.UnloadUnusedAssets同样可以卸载由AssetBundle.Load加载的资源,
只是前提是其对应的AssetBundle已经调用Unload(false),且并没有被引用;切场景时,就算没有Unload(false),也会被卸载;

1）静态变量引用的资源:Monobehavior中变量/属性引用的资源视为used;
2）被C#层强引用的资源,视为used;
3) 弱引用被Unity视为unused,是会被清掉的;
但是(先GC.Collect，再UnloadUnusedAssets则暂时清不掉,下次GC.Collect才能,或者交换顺序就能立刻清掉);

每一个load出来的asset难道不是同一个C#对象？为啥WeakReference的target变成null了???

 */
