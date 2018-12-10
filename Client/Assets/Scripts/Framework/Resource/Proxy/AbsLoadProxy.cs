/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/10 01:23:56
** desc:  资源加载代理抽象父类;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;

namespace Framework
{
    public abstract class AbsLoadProxy : IPool
    {
        public AssetType assetType { get; protected set; }
        public string assetName { get; protected set; }
        public bool isFinish { get; protected set; }//是否加载完成;
        public bool isCancel { get; protected set; }//取消了就不用执行异步加载的回调了;
        public Object targetObject { get; protected set; }
        public Action onLoadFinish;//加载完成回调;

        /// 初始化;
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetName"></param>
        public void InitProxy(AssetType assetType, string assetName)
        {
            this.assetType = assetType;
            this.assetName = assetName;
            isCancel = false;
            isFinish = false;
        }

        public void OnFinish(Object target)
        {
            targetObject = target;
            isFinish = true;
            if(!isCancel && onLoadFinish != null)
            {
                onLoadFinish();
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
                Unload();
                return true;
            }
            return false;
        }

        protected abstract void Unload();

        public void OnGet(params object[] args)
        {

        }

        public void OnRelease()
        {
            assetType = AssetType.Non;
            assetName = string.Empty;
            isCancel = false;
            isFinish = false;
        }
    }
}
