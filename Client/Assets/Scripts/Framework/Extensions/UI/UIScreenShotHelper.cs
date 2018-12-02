/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/29 23:46:32
** desc:  ½ØÆÁ;
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class UIScreenShotHelper
    {
        public static void ExecuteScreenShot(Action<Texture2D> action)
        {
            CoroutineMgr.Instance.StartCoroutine(WaitForEndOfFrameItor(() =>
            {
                var width = Screen.width;
                var height = Screen.height;
                Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                byte[] texByte = tex.EncodeToPNG();
                LogUtil.LogUtility.Print("[UIScreenShotHelper]Save Image to:" + Application.dataPath.ToLower() + "/../ScreenShot/");
                FileHelper.Write2Bytes(Application.dataPath.ToLower() + "/../ScreenShot/ScreenShot.png", texByte);

                if (action != null)
                {
                    action(tex);
                }
            }));
        }

        public static void ExecuteCameraShot(Camera camera, Action<Texture2D> action)
        {
            CoroutineMgr.Instance.StartCoroutine(WaitForEndOfFrameItor(() =>
            {
                var width = camera.pixelWidth;
                var height = camera.pixelHeight;
                var targetRt = camera.targetTexture;
                var activeRt = RenderTexture.active;
                RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                camera.targetTexture = rt;
                camera.Render();
                RenderTexture.active = rt;

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                camera.targetTexture = targetRt;
                RenderTexture.active = activeRt;

                byte[] texByte = tex.EncodeToPNG();
                LogUtil.LogUtility.Print("[UIScreenShotHelper]Save Image to:" + Application.dataPath.ToLower() + "/../ScreenShot/");
                FileHelper.Write2Bytes(Application.dataPath.ToLower() + "/../ScreenShot/CameraShot.png", texByte);

                if (action != null)
                {
                    action(tex);
                }
            }));
        }

        public static IEnumerator WaitForEndOfFrameItor(Action action)
        {
            yield return new WaitForEndOfFrame();
            if (action != null)
            {
                action();
            }
        }
    }
}