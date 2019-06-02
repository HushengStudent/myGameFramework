/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/06/02 00:03:35
** desc:  MonoPInvokeCallback Editor;
*********************************************************************************/

using Framework;
using UnityEditor;
using UnityEngine;

public class MonoPInvokeCallbackEditor : Editor
{
    private static bool _isEnable;

    [MenuItem("myGameFramework/Profiler/Lua Profiler Extension/Start MonoPInvokeCallback Analysis")]
    private static void _ExecuteAttach()
    {
        if (Application.isPlaying && LuaMgr.Instance)
        {
            if (_isEnable)
            {
                return;
            }
            _isEnable = true;
            ToluaProfiler.ProfilerEnable = _isEnable;
        }
    }

    [MenuItem("myGameFramework/Profiler/Lua Profiler Extension/Stop MonoPInvokeCallback Analysis")]
    private static void _ExecuteDetach()
    {
        if (!_isEnable)
        {
            return;
        }
        _isEnable = false;
        ToluaProfiler.ProfilerEnable = _isEnable;
    }
}
