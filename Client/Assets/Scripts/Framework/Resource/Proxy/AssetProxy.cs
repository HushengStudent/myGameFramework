/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/10 01:23:56
** desc:  资源加载抽象父类;
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public abstract class AssetProxy : IPool
    {
        //加载完成回调;
        private Action _onLoadFinish = null;

        public AssetType assetType { get; protected set; }
        public string assetName { get; protected set; }
        //是否加载完成;
        public bool IsFinish { get; protected set; }
        //取消了就不用执行异步加载的回调了;
        public bool IsCancel { get; protected set; }
        //是否使用对象池;
        public bool IsUsePool { get; protected set; }
        //加载完成对象;
        protected Object TargetObject { get; set; }

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
            this.IsUsePool = isUsePool;
            IsCancel = false;
            IsFinish = false;
        }

        /// <summary>
        /// 添加加载完成回调;
        /// </summary>
        /// <param name="action"></param>
        public void AddLoadFinishCallBack(Action action)
        {
            if (action != null)
            {
                if (IsFinish)
                {
                    action();
                    return;
                }
                _onLoadFinish += action;
            }
        }

        /// <summary>
        /// 设置完成;
        /// </summary>
        /// <param name="target"></param>
        public void OnFinish(Object target)
        {
            TargetObject = target;
            IsFinish = true;
            if (!IsCancel && _onLoadFinish != null)
            {
                _onLoadFinish();
                _onLoadFinish = null;
                OnFinishEx();
            }
        }

        protected virtual void OnFinishEx() { }

        /// <summary>
        /// 卸载代理;
        /// </summary>
        /// <returns></returns>
        public bool UnloadProxy()
        {
            if (IsFinish)
            {
                Unload();
                return true;
            }
            else
            {
                if (!IsCancel)
                {
                    CancelProxy();
                }
            }
            return false;
        }

        /// <summary>
        /// 取消代理,会自动卸载;
        /// </summary>
        protected void CancelProxy()
        {
            IsCancel = true;
            if (!UnloadProxy())
            {
                ResourceMgr.Instance.AddProxy(this);
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
            IsCancel = false;
            IsFinish = false;
            TargetObject = null;
            OnReleaseEx();
        }

        protected virtual void OnReleaseEx() { }

        protected abstract void Unload();

        public abstract T LoadUnityObject<T>() where T : Object;
        public abstract void DestroyUnityObject<T>(T t) where T : Object;

        public abstract T LoadUnitySharedAsset<T>() where T : Object;
        public abstract void DestroyUnitySharedAsset<T>(T t) where T : Object;

        protected bool IsInstantiate()
        {
            if (null != TargetObject)
            {
                //GameObject允许克隆;
                if (TargetObject as GameObject)
                {
                    return true;
                }
                //材质允许克隆;
                if (TargetObject as Material)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
