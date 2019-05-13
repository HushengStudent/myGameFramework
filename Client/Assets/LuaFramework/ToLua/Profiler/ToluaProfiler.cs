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
    private struct CallInfo
    {
        public ulong AllCount;
        public ulong FrameCount;
    }

    private static Dictionary<string, CallInfo> _luaCallCountDict = new Dictionary<string, CallInfo>();
    private static Dictionary<string, ulong> _frameCallCountDict = new Dictionary<string, ulong>();
    private static ulong _allFrame = 0;
    private static bool _profilerEnable = false;

    public static void AddCallRecord(string name)
    {
#if UNITY_EDITOR
        if (_profilerEnable)
        {
            ulong count = 0;
            _frameCallCountDict.TryGetValue(name, out count);
            _frameCallCountDict[name] = count + 1;

            CallInfo info;
            if (!_luaCallCountDict.TryGetValue(name, out info))
            {
                info = new CallInfo();
                info.AllCount = 0;
                info.FrameCount = 0;
            }
            info.AllCount = info.AllCount + 1;
            _luaCallCountDict[name] = info;

        }

#endif
    }

    public static void Update()
    {
#if UNITY_EDITOR
        if (_profilerEnable)
        {
            
        }
#endif
    }

}
