/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:50:49
** desc:  Unity Utility;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
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
        public static T AddOrGetComponent<T>(GameObject go, Action<T> action = null) where T : MonoBehaviour
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
    }
}
