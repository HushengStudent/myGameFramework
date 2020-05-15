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
        public static RenderTexture GetRT(int width, int high)
        {
            //antiAliasing 会导致rt内存变大;
            //return RenderTexture.GetTemporary(512, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
            var rt = RenderTexture.GetTemporary(width, high);
            rt.anisoLevel = 1;
            rt.autoGenerateMips = false;
            rt.filterMode = FilterMode.Bilinear;
            rt.useMipMap = false;
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