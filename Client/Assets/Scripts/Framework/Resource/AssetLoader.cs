/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/11 23:11:01
** desc:  ×ÊÔ´¼ÓÔØÆ÷;
*********************************************************************************/

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Framework
{
    public class AssetLoader
    {
        public static T GetAsset<T>(AssetType assetType, T t) where T : Object
        {
            if (assetType == AssetType.Prefab)
            {
                if (t == null) return null;
                return Object.Instantiate(t) as T;
            }
            else
            {
                return t;
            }
        }
    }
}
