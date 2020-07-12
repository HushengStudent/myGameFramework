/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/28 00:32:46
** desc:  Lua加载工具;
*********************************************************************************/

using LuaInterface;
using UnityEngine;

namespace Framework
{
    public class LuaLoaderHelper : LuaFileUtils
    {
        private AssetBundle _zipFile = null;

        private readonly string[] _heads = new string[] {
            "Assets/Bundles/Lua/",
            "Assets/Bundles/Lua/Network/",
        };

        /// <summary>
        /// 构造函数;
        /// </summary>
        /// <param name="isBundles">是否使用AssetBundle加载</param>
        public LuaLoaderHelper()
        {
            instance = this;
            beZip = GameMgr.AssetBundleModel;
        }


        /// <summary>
        /// 当LuaVM加载Lua文件的时候,这里就会被调用;
        /// 用户可以自定义加载行为,只要返回byte[]即可;
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override byte[] ReadFile(string fileName)
        {
            if (!beZip)
            {
                return base.ReadFile(fileName);
            }
            else
            {
                byte[] buffer = null;

                if (!fileName.EndsWith(".lua"))
                {
                    fileName += ".lua";
                }

#if UNITY_5 || UNITY_2017_1_OR_NEWER
                fileName += ".bytes";
#endif
                if (_zipFile == null)
                {
                    zipMap.TryGetValue(FilePathHelper.luaAssetBundleName, out _zipFile);
                }

                if (_zipFile != null)
                {
                    TextAsset luaCode = null;


                    for (int i = 0; i < _heads.Length; i++)
                    {
                        var head = _heads[i];
                        var tempName = head + fileName;

#if UNITY_5 || UNITY_2017_1_OR_NEWER
                        luaCode = _zipFile.LoadAsset<TextAsset>(tempName);
#else
                        luaCode = _zipFile.Load(fileName, typeof(TextAsset)) as TextAsset;
#endif
                        if (luaCode != null)
                        {
                            buffer = luaCode.bytes;
                            Resources.UnloadAsset(luaCode);
                            break;
                        }
                    }

                }
                return buffer;
            }
        }
    }
}
