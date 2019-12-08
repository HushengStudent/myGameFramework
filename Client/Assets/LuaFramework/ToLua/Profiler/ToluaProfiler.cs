/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/05/12 00:03:35
** desc:  tolua profiler;
*********************************************************************************/

using System.Collections.Generic;

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

    public static bool ProfilerEnable = false;

    public static void AddCallRecord(string name)
    {
#if UNITY_EDITOR
        if (ProfilerEnable)
        {
            ulong count = 0;
            _frameCallCountDict.TryGetValue(name, out count);
            _frameCallCountDict[name] = count + 1;

            CallInfo info;
            if (!_luaCallCountDict.TryGetValue(name, out info))
            {
                info = new CallInfo
                {
                    AllCount = 0,
                    FrameCount = 0
                };
            }
            info.AllCount = info.AllCount + 1;
            _luaCallCountDict[name] = info;

        }

#endif
    }

    public static void Update()
    {
#if UNITY_EDITOR
        if (ProfilerEnable)
        {
            foreach (var target in _frameCallCountDict)
            {
                var name = target.Key;
                var count = target.Value;
                CallInfo info;
                if (_luaCallCountDict.TryGetValue(name, out info))
                {
                    info.FrameCount = info.FrameCount > count ? info.FrameCount : count;
                }
            }
            _frameCallCountDict.Clear();
        }
#endif
    }

}
