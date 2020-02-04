/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/29 18:41:26
** desc:  CommandBuffer Renderer;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public class CBRenderer : IPool
    {
        private List<Renderer> _rendererList;
        private CommandBuffer CommandBuffer { get; set; }

        public bool Deprecated { get; set; }
        public Camera Camera { get; private set; }
        public RenderTexture RenderTexture { get; private set; }

        public void Initialize(GameObject go, Camera camera, Material mat = null)
        {
            if (!go || !camera)
            {
                Deprecated = true;
                return;
            }
            Camera = camera;
            _rendererList = PoolMgr.singleton.GetCsharpList<Renderer>();
            _rendererList.Add(go.GetComponent<Renderer>());
            _rendererList.AddRange(go.GetComponentsInChildren<Renderer>(true));
            if (_rendererList.Count < 1)
            {
                PoolMgr.singleton.ReleaseCsharpList(_rendererList);
                return;
            }
            RenderTexture = RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
            CommandBuffer = new CommandBuffer();
            CommandBuffer.Clear();
            CommandBuffer.SetRenderTarget(RenderTexture);
            CommandBuffer.ClearRenderTarget(true, true, Color.black);
            CommandBuffer.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
            for (int i = 0; i < _rendererList.Count; i++)
            {
                var renderer = _rendererList[i];
                Material targetMat = mat ?? renderer.sharedMaterial;
                CommandBuffer.DrawRenderer(renderer, targetMat);
            }

            //然后接受物体的材质使用这张RT作为主纹理
            //this.GetComponent<Renderer>().sharedMaterial.mainTexture = renderTexture;

            //Camera.main.AddCommandBuffer(CameraEvent.AfterForwardOpaque, CommandBuffer);
            //Camera.main.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);
        }

        public void Execute()
        {
            Graphics.ExecuteCommandBuffer(CommandBuffer);
        }

        public void OnGet(params object[] args)
        {
            Deprecated = false;
        }

        public void OnRelease()
        {
            Deprecated = false;
            Camera = null;
            if (_rendererList != null)
            {
                PoolMgr.singleton.ReleaseCsharpList(_rendererList);
            }
            if (CommandBuffer != null)
            {
                CommandBuffer.Clear();
            }
            if (RenderTexture)
            {
                RenderTexture.ReleaseTemporary(RenderTexture);
            }
        }
    }
}