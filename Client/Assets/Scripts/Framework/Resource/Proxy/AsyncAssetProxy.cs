/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/10 01:03:13
** desc:  AssetBundle资源异步加载代理;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;

namespace Framework
{
    public class AsyncAssetProxy : SyncAssetProxy
    {
        public AssetBundleLoadNode LoadNode { get; private set; }

        public void InitProxy(AssetType assetType, string assetName, AssetBundleLoadNode LoadNode
            , bool isUsePool = true)
        {
            base.InitProxy(assetType, assetName, isUsePool);
            this.LoadNode = LoadNode;
        }

        protected override void Unload()
        {
            PoolMgr.Instance.ReleaseUnityAsset(assetType, assetName, TargetObject, IsUsePool);
            PoolMgr.Instance.ReleaseCsharpObject(this);
        }

        protected override void OnReleaseEx()
        {
            base.OnReleaseEx();
            PoolMgr.Instance.ReleaseCsharpObject<AssetBundleLoadNode>(LoadNode);
            LoadNode = null;
        }
    }
}
