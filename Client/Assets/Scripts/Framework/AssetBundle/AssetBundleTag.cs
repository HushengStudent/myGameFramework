/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/20 00:11:40
** desc:  AssetBundle±ê¼Ç
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AssetBundleTag : MonoBehaviour
    {
        private string assetBundleName = string.Empty;

        public string AssetBundleName
        {
            set { assetBundleName = value; }
        }

        void OnDestroy()
        {

        }
    }
}
