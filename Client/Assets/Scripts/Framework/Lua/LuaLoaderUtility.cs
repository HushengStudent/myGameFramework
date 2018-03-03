/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/28 00:32:46
** desc:  Lua加载工具
*********************************************************************************/

using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class LuaLoaderUtility : LuaFileUtils
    {
        /// <summary>
        /// 构造函数;
        /// </summary>
        /// <param name="isBundles">是否使用AssetBundle加载</param>
        public LuaLoaderUtility(bool isBundles = false)
        {
            beZip = isBundles;
        }

        public void AddBundle(string bundleName)
        {

        }
    }
}
