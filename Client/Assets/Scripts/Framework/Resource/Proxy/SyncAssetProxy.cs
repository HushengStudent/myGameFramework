/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 21:07:40
** desc:  AssetBundle资源同步加载代理;
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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

        public GameObject LoadGameObject(bool isUsePool = true)
        {
            GameObject go = null;
            if (isUsePool)
            {
                PoolMgr.Instance.GetUnityGameObject(targetObject);
            }
            else
            {
                go = GameObject.Instantiate(targetObject) as GameObject;
            }
            return go;
        }

        public void DestroyGameObject(GameObject go, bool isReturn2Pool = true)
        {
            if (isReturn2Pool)
            {
                PoolMgr.Instance.ReleaseUnityGameObject(targetObject);
            }
            else
            {
                GameObject.Destroy(go);
            }
        }
    }
}
