/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/10 01:23:56
** desc:  资源加载代理父类;
*********************************************************************************/

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Framework
{
    public class AsyncProxy : IPool
    {
        //加载完成回调;
        private Action<Object> _onLoadFinish = null;

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

        /// <summary>
        /// 添加加载完成回调;
        /// </summary>
        /// <param name="action"></param>
        public void AddLoadFinishCallBack(Action<Object> action)
        {
            _onLoadFinish += action;
        }

        /// <summary>
        /// 设置完成;
        /// </summary>
        /// <param name="target"></param>
        public void OnFinish(Object target)
        {
            targetObject = target;
            isFinish = true;
            if (!isCancel && _onLoadFinish != null)
            {
                _onLoadFinish(target);
                _onLoadFinish = null;
                OnFinishEx();
            }
        }

        protected virtual void OnFinishEx() { }

        /// <summary>
        /// 取消代理,会自动卸载;
        /// </summary>
        public void CancelProxy()
        {
            isCancel = true;
            if (!UnloadProxy())
            {
                ResourceMgr.Instance.AddProxy(this);
            }
        }

        /// <summary>
        /// 卸载代理;
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 卸载;
        /// </summary>
        protected virtual void Unload()
        {
            if (targetObject != null)
            {
                ResourceMgr.Instance.UnloadObject(assetType, targetObject);
            }
        }

        /// <summary>
        /// 卸载到对象池;
        /// </summary>
        protected virtual void Unload2Pool()
        {
            if (targetObject != null)
            {

            }
        }

        public void OnGet(params object[] args)
        {
            OnGetEx(args);
        }

        protected virtual void OnGetEx(params object[] args) { }

        public void OnRelease()
        {
            _onLoadFinish = null;
            assetType = AssetType.Non;
            assetName = string.Empty;
            isCancel = false;
            isFinish = false;
            targetObject = null;
            OnReleaseEx();
        }

        protected virtual void OnReleaseEx() { }
    }
}
