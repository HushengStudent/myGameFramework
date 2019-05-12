/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/05/12 00:03:35
** desc:  tolua profiler;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToluaProfiler
{
    private static Dictionary<string, ulong> _luaCallCountDict = new Dictionary<string, ulong>();

    private static Dictionary<string, ulong> _frameCallCountDict = new Dictionary<string, ulong>();

    public static void AddCallRecord(string name)
    {
#if UNITY_EDITOR
        //if (_luaCallCountDict.TryGetValue(name, out var count))
        {

        }
#endif
    }

    public static void Update()
    {
#if UNITY_EDITOR

#endif
    }

}
