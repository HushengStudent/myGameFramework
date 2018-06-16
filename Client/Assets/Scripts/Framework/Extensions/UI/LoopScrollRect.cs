/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 00:45:42
** desc:  UI复用;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public enum ScroolRectDirection
    {
        Horizontal = 0,
        Vertical = 1
    }

    public delegate void OnLoopItemUpdate(int index, GameObject go);

    [RequireComponent(typeof(RectTransform))]
    public class LoopScrollRect : MonoBehaviour
    {

    }
}
