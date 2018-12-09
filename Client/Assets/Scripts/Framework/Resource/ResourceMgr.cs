/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:27:56
** desc:  资源管理;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using MEC;
using System.IO;

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
            //UnloadUnusedAssets();
            //GameGC();
            InitLua();
            InitShader();
            CoroutineMgr.Instance.RunCoroutine(CancleAllProxy());
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
            //AssetBundleMgr.Instance.UnloadMirroring(AssetType.Shader, "Shader");
        }

        /// <summary>
        /// 初始化Lua;
        /// </summary>
        private void InitLua()
        {
            //TODO...
        }

        #endregion

        #region Functions

        /// <summary>
        /// 创建资源加载器;
        /// </summary>
        /// <typeparam name="T">ctrl</typeparam>
        /// <param name="assetType">资源类型</param>
        /// <returns>资源加载器;</returns>
        private IAssetLoader<T> CreateLoader<T>(AssetType assetType) where T : Object
        {
            if (assetType == AssetType.Prefab)
            {
                return new ResLoader<T>();
            }
            return new AssetLoader<T>();
        }

        /// <summary>
        /// Clone GameObject;
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public GameObject Clone(GameObject go)
        {
            GameObject target = GameObject.Instantiate(go);
            return target;
        }

        /// <summary>
        /// Destroy GameObject;
        /// </summary>
        /// <param name="go"></param>
        public void Destroy(GameObject go)
        {
            GameObject.Destroy(go);
        }

        #endregion

        #region Unload Assets

        /// <summary>
        /// 清理;
        /// </summary>
        public void UnloadUnusedAssets(Action<AsyncOperation> action)
        {
            AsyncOperation operation = Resources.UnloadUnusedAssets();
            operation.completed += action;
        }

        /// <summary>
        /// GC;
        /// </summary>
        public void GameGC()
        {
            System.GC.Collect();
        }

        #endregion
    }
}
