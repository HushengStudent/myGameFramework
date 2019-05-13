/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/05/13 00:03:15
** desc:  Unity Profiler扩展;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

public static class LuaProfilerExtension
{
    private static int _depthValue;
    private const int  _maxDepthValue = 100;
    private static readonly Dictionary<int, string> _showNamesDict = new Dictionary<int, string>();

    [Conditional("UNITY_EDITOR")]
    public static void BeginSample(int id)
    {
        string name;
        _showNamesDict.TryGetValue(id, out name);
        name = name ?? string.Empty;
        Profiler.BeginSample(name);
        ++_depthValue;
    }

    [Conditional("UNITY_EDITOR")]
    public static void BeginSample(int id, string name)
    {
        name = name ?? string.Empty;
        _showNamesDict[id] = name;
        Profiler.BeginSample(name);
        ++_depthValue;
    }

    [Conditional("UNITY_EDITOR")]
    internal static void BeginSample(string name)
    {
        name = name ?? string.Empty;
        Profiler.BeginSample(name);
        ++_depthValue;
    }

    [Conditional("UNITY_EDITOR")]
    public static void EndSample()
    {
        if (_depthValue > 0)
        {
            --_depthValue;
            Profiler.EndSample();
        }
    }
}
