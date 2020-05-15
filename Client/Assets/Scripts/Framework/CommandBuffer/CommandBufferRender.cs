/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/05/16 00:16:57
** desc:  CommandBuffer Render;
*********************************************************************************/

using System;
using UnityEngine;

namespace Framework
{
    public class CommandBufferRender : IPool
    {
        public RenderTexture RenderTexture { get; private set; }

        private void ReleaseRT()
        {
            RenderTextureHelper.ReleaseRT(RenderTexture);
            RenderTexture = null;
        }

        public void Release()
        {
            PoolMgr.singleton.ReleaseCsharpObject(this);
        }

        void IPool.OnGet(params object[] args)
        {
            try
            {
                var width = (int)args[0];
                var high = (int)args[1];
                RenderTexture = RenderTextureHelper.GetRT(width, high);
            }
            catch (Exception)
            {
                RenderTexture = null;
            }
        }

        void IPool.OnRelease()
        {
            ReleaseRT();
        }

        ~CommandBufferRender()
        {
            ReleaseRT();
        }
    }
}