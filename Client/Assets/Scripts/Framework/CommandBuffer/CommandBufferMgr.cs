/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/25 00:04:02
** desc:  CommandBuffer Mgr;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public class CommandBufferMgr : MonoSingleton<CommandBufferMgr>
    {
        private CommandBuffer _commandBuffer;
        private Camera _camera;
        private HashSet<Renderer> _rendererHashSet;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _commandBuffer = new CommandBuffer
            {
                name = "CommandBufferMgr"
            };
            var cameraObject = new GameObject("CommandBufferMgr_Camera");
            cameraObject.SetActive(true);
            _camera = cameraObject.AddComponent<Camera>();
            //_camera.cullingMask 必须有;
            _camera.clearFlags = CameraClearFlags.Depth;
            _camera.orthographic = true;
            _camera.allowDynamicResolution = false;
            _camera.useOcclusionCulling = false;
            _camera.allowHDR = false;
            _camera.allowMSAA = false;
            _camera.enabled = false;
            _rendererHashSet = new HashSet<Renderer>();
        }

        protected override void OnUninitialize()
        {
            base.OnUninitialize();
            if (_commandBuffer != null)
            {
                _commandBuffer.Release();
                _commandBuffer = null;
            }
            if (_camera)
            {
                Destroy(_camera.gameObject);
                _camera = null;
            }
            _rendererHashSet.Clear();
            _rendererHashSet = null;
        }

        /// <summary>
        /// 2D相机渲染;
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="width">RT宽</param>
        /// <param name="high">RT高</param>
        /// <param name="camWidth">相机宽</param>
        /// <param name="camHigh">相机高</param>
        /// <param name="originalCamSize">原始相机的Size</param>
        /// <param name="camScale">相机的Scale</param>
        /// <param name="mat">材质</param>
        /// <returns></returns>
        public CommandBufferRender DrawRenderer(GameObject target, int width, int high, int camWidth, int camHigh
            , float originalCamSize, Vector3 camScale, Material mat = null)
        {
            if (!target || !_camera)
            {
                return null;
            }
            _rendererHashSet.Clear();
            _rendererHashSet.Add(target.GetComponent<Renderer>());
            foreach (var r in target.GetComponentsInChildren<Renderer>(true))
            {
                _rendererHashSet.Add(r);
            }
            if (_rendererHashSet.Count < 1)
            {
                return null;
            }

            var x = width / camWidth / camScale.x;
            var y = high / camHigh / camScale.y;

            _camera.orthographicSize = originalCamSize * y;
            _camera.rect = new Rect(0, 0, x, y);

            var render = PoolMgr.singleton.GetCsharpObject<CommandBufferRender>(width, high);
            _commandBuffer.Clear();
            _commandBuffer.SetRenderTarget(render.RenderTexture);
            _commandBuffer.ClearRenderTarget(true, true, Color.clear);
            _commandBuffer.SetViewProjectionMatrices(_camera.worldToCameraMatrix, _camera.projectionMatrix);

            foreach (var r in _rendererHashSet)
            {
                Material targetMat = mat ?? r.sharedMaterial;
                _commandBuffer.DrawRenderer(r, targetMat);
            }
            _rendererHashSet.Clear();

            Graphics.ExecuteCommandBuffer(_commandBuffer);

            //然后接受物体的材质使用这张RT作为主纹理
            //this.GetComponent<Renderer>().sharedMaterial.mainTexture = renderTexture;

            //Camera.main.AddCommandBuffer(CameraEvent.AfterForwardOpaque, CommandBuffer);
            //Camera.main.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);

            return render;
        }
    }
}