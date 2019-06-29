/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/25 00:04:02
** desc:  CommandBuffer Mgr;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public class CommandBufferMgr : MonoSingleton<CommandBufferMgr>
    {
        private CommandBuffer _commandBuffer = null;
        private RenderTexture _renderTexture = null;
        private Renderer _renderer = null;
        public GameObject _targetObject = null;
        public Material _replaceMaterial = null;

        public void DrawRenderer()
        {
        }
    }
}