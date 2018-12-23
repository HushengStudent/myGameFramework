/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 21:07:40
** desc:  AssetBundle资源同步加载代理;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class SyncAssetProxy : AssetProxy
    {
        protected override void Unload()
        {
            base.Unload();
            if (targetObject != null)
            {
                AssetBundleMgr.Instance.UnloadAsset(assetType, assetName);
            }
            PoolMgr.Instance.ReleaseCsharpObject<SyncAssetProxy>(this);
        }

        protected override void Unload2Pool()
        {
            base.Unload2Pool();

            PoolMgr.Instance.ReleaseCsharpObject<SyncAssetProxy>(this);
        }
    }
}
