/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/23 00:39:09
** desc:  Asset加载接口;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// 资源加载接口,主要区别是否实例化;
    /// </summary>
    public interface IAssetLoader<T> where T : Object
    {
        T GetAsset(T t, out bool isInstance);
    }

    public class AssetLoader<T> : IAssetLoader<T> where T : Object
    {
        public T GetAsset(T t, out bool isInstance)
        {
            isInstance = false;
            return t;
        }
    }

    public class ResLoader<T> : IAssetLoader<T> where T : Object
    {
        public T GetAsset(T t, out bool isInstance)
        {
            isInstance = true;
            if (t == null) return null;
            return Object.Instantiate(t) as T;
        }
    }
}
