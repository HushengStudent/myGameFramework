/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源加载代理;
*********************************************************************************/

using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class ResourceLoadProxy : AbsLoadProxy
    {
        protected override void Unload()
        {
            if (targetObject != null)
            {
                ResourceMgr.Instance.UnloadObject(assetType, targetObject);
            }
            PoolMgr.Instance.Release<ResourceLoadProxy>(this);
        }
    }
}
