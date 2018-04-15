/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/20 00:11:40
** desc:  AssetBundle标记;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AssetBundleTag : MonoBehaviour
    {
        private string assetName = string.Empty;

        private AssetType type = AssetType.Non;

        public string AssetBundleName
        {
            get { return assetName; }
            set { assetName = value; }
        }

        public AssetType Type
        {
            get { return type; }
            set { type = value; }
        }

        void OnDestroy()
        {
            AssetBundleMgr.Instance.UnloadAsset(Type, AssetBundleName);
        }
    }
}
