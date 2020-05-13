/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/07/30 00:50:46
** desc:  Lua帮助类;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public static class LuaHelper
    {
        public static long Long(object target)
        {
            return (long)target;
        }

        public static int Int(object target)
        {
            return (int)target;
        }

        public static void SetLocalPosition(this GameObject target, Vector3 vect)
        {
            if (target)
            {
                target.transform.localPosition = vect;
            }
        }

        public static void SetLocalRotation(this GameObject target, Vector3 vect)
        {
            if (target)
            {
                target.transform.localRotation = Quaternion.Euler(vect);
            }
        }

        public static void SetTransParent(Transform trans, Transform parent)
        {
            if (trans)
            {
                trans.parent = parent;
            }
        }

        public static bool IsNull(object obj)
        {
            return obj == null || ReferenceEquals(obj, null);
        }

        public static Vector2 WorldToScreenPoint(Vector3 position)
        {
            var mainCamera = CameraMgr.singleton.MainCamera;
            if (mainCamera)
            {
                position = mainCamera.WorldToScreenPoint(position);
                var mainUICamera = CameraMgr.singleton.MainUICamera;
                if (mainUICamera)
                {
                    var uiRoot = UIMgr.singleton.UIRoot;
                    if (uiRoot.transform as RectTransform)
                    {
                        var localPoint = Vector2.zero;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiRoot.transform as RectTransform, position, mainUICamera, out localPoint);
                        return localPoint;
                    }
                }
            }
            return Vector2.zero;
        }
    }
}