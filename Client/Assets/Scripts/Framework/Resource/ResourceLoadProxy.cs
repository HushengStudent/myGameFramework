/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源加载代理;
*********************************************************************************/

using Object = UnityEngine.Object;

namespace Framework
{
    public class ResourceLoadProxy
    {
        public AssetType assetType { get; private set; }
        public string assetName { get; private set; }
        public bool isFinish { get; private set; }
        public Object targetObject { get; private set; }

        /// <summary>
        /// 初始化;
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="assetName"></param>
        public void InitProxy(AssetType assetType, string assetName)
        {
            this.assetType = assetType;
            this.assetName = assetName;
            isFinish = false;
        }

        public void OnFinish(Object target)
        {
            targetObject = target;
            isFinish = true;
        }

        public void CancelProxy()
        {
            if (!UnloadProxy())
            {
                ResourceMgr.Instance.AddProxy(this);
            }
        }

        public bool UnloadProxy()
        {
            if (isFinish)
            {
                ResourceMgr.Instance.UnloadObject(assetType, targetObject);
                assetType = AssetType.Non;
                assetName = string.Empty;
                isFinish = false;
                return true;
            }
            return false;
        }
    }
}
