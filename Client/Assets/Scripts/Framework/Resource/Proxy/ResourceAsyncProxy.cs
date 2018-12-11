/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源加载代理;
*********************************************************************************/

using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class ResourceAsyncProxy : AsyncProxy
    {
        protected override void Unload()
        {
            PoolMgr.Instance.Release<ResourceAsyncProxy>(this);
        }

        protected override void Unload2Pool()
        {
            PoolMgr.Instance.Release<ResourceAsyncProxy>(this);
        }
    }
}
