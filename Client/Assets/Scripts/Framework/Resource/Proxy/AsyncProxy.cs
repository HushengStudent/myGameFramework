/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/10 01:23:56
** desc:  资源加载代理抽象父类;
*********************************************************************************/

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Framework
{
    public class AsyncProxy : IPool
    {
        public AssetType assetType { get; protected set; }
        public string assetName { get; protected set; }
        //是否加载完成;
        public bool isFinish { get; protected set; }
        //取消了就不用执行异步加载的回调了;
        public bool isCancel { get; protected set; }
        //是否使用对象池;
        public bool isUsePool { get; protected set; }
        //加载完成对象;
        public Object targetObject { get; protected set; }

        //加载完成回调;
        public Action<Object> onLoadFinish = null;

        /// <summary>
        /// 初始化;
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetName"></param>
        /// <param name="isUsePool"></param>
        public void InitProxy(AssetType assetType, string assetName, bool isUsePool = true)
        {
            this.assetType = assetType;
            this.assetName = assetName;
            this.isUsePool = isUsePool;
            isCancel = false;
            isFinish = false;
        }

        public void OnFinish(Object target)
        {
            targetObject = target;
            isFinish = true;
            if (!isCancel && onLoadFinish != null)
            {
                onLoadFinish(target);
            }
        }

        public void CancelProxy()
        {
            isCancel = true;
            if (!UnloadProxy())
            {
                ResourceMgr.Instance.AddProxy(this);
            }
        }

        public bool UnloadProxy()
        {
            if (isFinish)
            {
                if (isUsePool)
                {
                    Unload2Pool();
                }
                else
                {
                    Unload();
                }
                return true;
            }
            return false;
        }

        protected virtual void Unload()
        {
            UnloadProxy(assetType, targetObject);
        }

        protected virtual void Unload2Pool()
        {
            UnloadProxy2Pool(assetType, targetObject);
        }

        public void OnGet(params object[] args) { }

        public void OnRelease()
        {
            assetType = AssetType.Non;
            assetName = string.Empty;
            isCancel = false;
            isFinish = false;
        }

        /// <summary>
        /// 卸载代理;
        /// </summary>
        /// <param name="assetType">资源类型</param>
        /// <param name="asset">资源</param>
        protected void UnloadProxy(AssetType assetType, Object asset)
        {
            if (asset != null)
            {
                ResourceMgr.Instance.UnloadObject(assetType, asset);
            }
        }

        protected void UnloadProxy2Pool(AssetType assetType, Object asset)
        {
            if (asset != null)
            {

            }
        }
    }
}