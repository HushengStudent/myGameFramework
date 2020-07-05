/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/05/16 00:20:37
** desc:  RenderTexture Helper;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class RenderTextureHelper : MonoBehaviour
    {
        public static RenderTexture GetRT(int width, int height, string rtName = "Name_Null_RT",
            RenderTextureFormat rtFormat = RenderTextureFormat.Default)
        {
            if (!SystemInfo.SupportsRenderTextureFormat(rtFormat))
            {
                rtFormat = RenderTextureFormat.Default;
            }

            //antiAliasing 会导致rt内存变大;
            //return RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);

            var descriptor = new RenderTextureDescriptor(width, height, rtFormat);
            var rt = RenderTexture.GetTemporary(descriptor);
            rt.anisoLevel = 1;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Bilinear;
            rt.name = $"{rtName}_{width}x{height}";
            return rt;
        }

        public static void ReleaseRT(RenderTexture rt)
        {
            if (rt)
            {
                RenderTexture.ReleaseTemporary(rt);
            }
        }
    }
}