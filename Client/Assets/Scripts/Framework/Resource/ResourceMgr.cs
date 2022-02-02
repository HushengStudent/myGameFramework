/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:56
** desc:  资源管理;
*********************************************************************************/

using Framework.AssetBundleModule;
using UnityEngine;

namespace Framework.ResourceModule
{
    public partial class ResourceMgr : MonoSingleton<ResourceMgr>
    {
        #region Initialize

        public AssetBundle LuaAssetBundle { get; private set; }

        private Object[] _allLua;
        private Object[] _allShader;

        /// <summary>
        /// 初始化;
        /// </summary>
        protected override void OnInitialize()
        {
            var assetBundleModel = GameMgr.AssetBundleModel;
            if (assetBundleModel)
            {
                InitLua();
                InitShader();
            }
            CoroutineMgr.singleton.RunCoroutine(CancleAllProxy());
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
                _allShader = shaderAssetBundle.LoadAllAssets();
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
                _allLua = LuaAssetBundle.LoadAllAssets();
                LogHelper.Print("[ResourceMgr]Load Lua Success!");
            }
            else
            {
                LogHelper.PrintError("[ResourceMgr]Load Lua failure!");
            }
            //AssetBundleMgr.Instance.UnloadMirroring(FilePathHelper.luaAssetBundleName);
        }

        protected override void OnDestroyEx()
        {
            base.OnDestroyEx();
            _allLua = null;
            _allShader = null;
        }

        public Shader FindShader(string name)
        {
            if (_allShader != null && _allShader.Length > 0)
            {
                foreach (var target in _allShader)
                {
                    var shader = target as Shader;
                    if (shader && shader.name == name)
                    {
                        return shader;
                    }
                }
            }
            return Shader.Find(name);
        }

        #endregion

        #region Load

        public AbsAssetProxy LoadAssetSync(string path)
        {
            if (!path.StartsWith("Assets/Bundles/"))
            {
                return LoadResourceAssetSync(path);
            }
            else
            {
                return LoadAssetBundleAssetSync(path);
            }
        }

        public AbsAssetProxy LoadAssetAsync(string path)
        {
            if (!path.StartsWith("Assets/Bundles/"))
            {
                return LoadResourceAssetAsync(path);
            }
            else
            {
                return LoadAssetBundleAssetAsync(path);
            }
        }

        #endregion
    }
}

