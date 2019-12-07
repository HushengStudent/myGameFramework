/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/29 23:46:32
** desc:  ½ØÆÁ;
*********************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    public static class UIScreenShotHelper
    {
        public static void ExecuteScreenShot(Action<Texture2D> action)
        {
            CoroutineMgr.singleton.StartCoroutine(WaitForEndOfFrameItor(() =>
            {
                var width = Screen.width;
                var height = Screen.height;
                Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                byte[] texByte = tex.EncodeToPNG();
                LogHelper.Print($"[UIScreenShotHelper]Save Image to:{Application.dataPath.ToLower()}/../ScreenShot/");
                FileHelper.Write2Bytes($"{Application.dataPath.ToLower()}/../ScreenShot/ScreenShot.png", texByte);

                action?.Invoke(tex);
            }));
        }

        public static void ExecuteCameraShot(Camera camera, Action<Texture2D> action)
        {
            CoroutineMgr.singleton.StartCoroutine(WaitForEndOfFrameItor(() =>
            {
                var width = camera.pixelWidth;
                var height = camera.pixelHeight;
                var targetRt = camera.targetTexture;
                var activeRt = RenderTexture.active;
                RenderTexture rt = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
                camera.targetTexture = rt;
                camera.Render();
                RenderTexture.active = rt;

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                camera.targetTexture = targetRt;
                RenderTexture.active = activeRt;

                RenderTexture.ReleaseTemporary(rt);

                byte[] texByte = tex.EncodeToPNG();
                LogHelper.Print($"[UIScreenShotHelper]Save Image to:{Application.dataPath.ToLower()}/../ScreenShot/");
                FileHelper.Write2Bytes($"{Application.dataPath.ToLower()}/../ScreenShot/CameraShot.png", texByte);

                action?.Invoke(tex);
            }));
        }

        public static IEnumerator WaitForEndOfFrameItor(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
    }
}