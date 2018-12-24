/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:56
** desc:  资源管理;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MEC;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class ResourceMgr : MonoSingleton<ResourceMgr>
    {
        #region Initialize

        /// <summary>
        /// 初始化;
        /// </summary>
        public override void Init()
        {
            base.Init();
            InitLua();
            InitShader();
            CoroutineMgr.Instance.RunCoroutine(CancleAllProxy());
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            UpdateProxy();
            UpdateLoadAssetAsync();
        }

        /// <summary>
        /// 初始化Shader;
        /// </summary>
        private void InitShader()
        {
            //Shader初始化;
            AssetBundle shaderAssetBundle = AssetBundleMgr.Instance.LoadShaderAssetBundle();
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
            AssetBundleMgr.Instance.UnloadMirroring(AssetType.Shader, "Shaders");
        }

        /// <summary>
        /// 初始化Lua;
        /// </summary>
        private void InitLua()
        {
            //TODO...
        }

        #endregion
    }
}
