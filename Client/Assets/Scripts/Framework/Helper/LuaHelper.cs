/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/07/30 00:50:46
** desc:  Lua帮助类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
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

        public static void SetTransParent(Transform trans, Transform parent)
        {
            if (trans)
            {
                trans.parent = parent;
            }
        }
    }
}