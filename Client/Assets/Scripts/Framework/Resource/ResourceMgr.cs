/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:56
** desc:  资源管理;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public partial class ResourceMgr : MonoSingleton<ResourceMgr>
    {
        #region Initialize

        public AssetBundle LuaAssetBundle { get; private set; }

        /// <summary>
        /// 初始化;
        /// </summary>
        protected override void OnInitialize()
        {
            InitLua();
            InitShader();
            CoroutineMgr.Singleton.RunCoroutine(CancleAllProxy());
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            UpdateProxy();
        }

        /// 初始化Shader;
        private void InitShader()
        {
            //Shader初始化;
            var shaderAssetBundle = AssetBundleMgr.singleton.LoadShaderAssetBundle();
            if (shaderAssetBundle != null)
            {
                shaderAssetBundle.LoadAllAssets();
                Shader.WarmupAllShaders();
                LogHelper.Print("[ResourceMgr]Load Shader and WarmupAllShaders Success!");
            }
            else
            {
                LogHelper.PrintError("[ResourceMgr]Load Shader and WarmupAllShaders failure!");
            }
            //AssetBundleMgr.Instance.UnloadMirroring(FilePathHelper.shaderAssetBundleName);
        }

        /// 初始化Lua;
        private void InitLua()
        {
            //Lua初始化;
            LuaAssetBundle = AssetBundleMgr.singleton.LoadLuaAssetBundle();
            if (LuaAssetBundle != null)
            {
                LuaAssetBundle.LoadAllAssets();
                LogHelper.Print("[ResourceMgr]Load Lua Success!");
            }
            else
            {
                LogHelper.PrintError("[ResourceMgr]Load Lua failure!");
            }
            //AssetBundleMgr.Instance.UnloadMirroring(FilePathHelper.luaAssetBundleName);
        }

        #endregion
    }
}

