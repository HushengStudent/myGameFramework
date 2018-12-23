/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/09 20:08:35
** desc:  Resource资源异步加载代理;
*********************************************************************************/

using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class AsyncResourceProxy : AssetProxy
    {
        protected override void Unload()
        {
            PoolMgr.Instance.ReleaseCsharpObject<AsyncResourceProxy>(this);
        }

        protected override void Unload2Pool()
        {
            PoolMgr.Instance.ReleaseCsharpObject<AsyncResourceProxy>(this);
        }
    }
}
