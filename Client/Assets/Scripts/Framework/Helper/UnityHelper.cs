/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:50:49
** desc:  Unity Utility;
*********************************************************************************/

using System;
using UnityEngine;

namespace Framework
{
    public static class UnityHelper
    {
        /// <summary>
        /// 添加组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T AddOrGetComponent<T>(this GameObject go, Action<T> action = null) where T : MonoBehaviour
        {
            T temp = null;
            if (go == null)
            {
                LogHelper.PrintError("[UnityUtility]AddOrGetComponent error:gameObject is null!");
                return null;
            }
            temp = go.GetComponent<T>();
            if (temp == null)
            {
                go.AddComponent<T>();
                temp = go.GetComponent<T>();
            }
            if (action != null)
                action(temp);
            return temp;
        }

        /// <summary>
        /// 正交相机大小计算;
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static Vector2 GetCameraSize(this Camera camera)
        {
            float orthographicSize = camera.orthographicSize;
            float aspectRatio = Screen.width * 1.0f / Screen.height;
            float height = orthographicSize * 2;
            float width = height * aspectRatio;
            return new Vector2(height, width);
        }
    }
}
