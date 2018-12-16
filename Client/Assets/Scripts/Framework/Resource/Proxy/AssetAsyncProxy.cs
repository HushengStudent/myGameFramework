/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/10 01:03:13
** desc:  AssetBundle资源加载代理;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class AssetAsyncProxy : AsyncProxy
    {
        protected override void Unload()
        {
            base.Unload();
            if (targetObject != null)
            {
                AssetBundleMgr.Instance.UnloadAsset(assetType, assetName);
            }
            PoolMgr.Instance.ReleaseObject<AssetAsyncProxy>(this);
        }

        protected override void Unload2Pool()
        {
            base.Unload2Pool();

            PoolMgr.Instance.ReleaseObject<AssetAsyncProxy>(this);
        }
    }
}
