/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/23 17:34:50
** desc:  资源加载器,不参与资源管理;
*********************************************************************************/

using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class ResourceMgr
    {
        #region Unload Assets

        public void DestroyUnityAsset(Object asset)
        {
            if (asset != null)
            {
                if(asset is GameObject)
                {
                    return;
                }

                /// 卸载不需实例化的资源:纹理,animator,clip,material;
                /// 卸载非GameObject类型的资源,会将内存中已加载资源及其克隆体卸载:前提是已经没有任何引用持有该资源,可以置null再卸载;
                /// Unload Assets may only be used on individual assets and can not be used on GameObject's/Components or AssetBundles;
                Resources.UnloadAsset(asset);
            }
        }

        public void DestroyInstantiateObject(Object asset)
        {
            if (asset != null)
            {
                Destroy(asset);
                return;
            }
        }

        public void GameGC()
        {
            GC.Collect();
        }

        public void UnloadUnusedAssets(Action onFinish)
        {
            UnloadUnusedAssets((option) =>
            {
                AssetBundleMgr.Instance.AutoUnloadAsset();
                if (onFinish != null)
                {
                    onFinish();
                }
            });
        }

        private void UnloadUnusedAssets(Action<AsyncOperation> action)
        {
            AsyncOperation operation = Resources.UnloadUnusedAssets();
            if (operation.isDone)
            {
                if (action != null)
                {
                    action(operation);
                }
            }
            else
            {
                operation.completed += action;
            }
        }

        #endregion
    }
}