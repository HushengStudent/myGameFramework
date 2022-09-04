/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2022/05/28 19:51:55
** desc:  多线程;
*********************************************************************************/

using Framework;
using System;
using System.Collections.Generic;
using System.Threading;

public class ThreadMgr : MonoSingleton<ThreadMgr>
{
    private static int _runID = 0;
    private static int RunID
    {
        get
        {
            if (_runID == int.MaxValue)
            {
                _runID = 0;
            }
            return _runID++;
        }
    }

    private readonly Dictionary<int, Action> _onFinishDict = new Dictionary<int, Action>();
    private readonly Dictionary<int, string> _exceptionDict = new Dictionary<int, string>();
    private static readonly object _lock = new object();

    protected override void AwakeEx()
    {
        base.AwakeEx();
        var count = System.Environment.ProcessorCount;
        ThreadPool.SetMaxThreads(count, count);
    }

    public void RunAsync(Action runAtThread, Action runAtMainThread)
    {
        var id = RunID;
        _onFinishDict[id] = runAtMainThread;
        ThreadPool.QueueUserWorkItem((_) => RunAction(runAtThread, id));
    }

    private void RunAction(Action action, int id)
    {
        if (!ApplicationIsPlaying)
        {
            return;
        }

        var msg = string.Empty;
        try
        {
            action?.Invoke();
        }
        catch (Exception e)
        {
            msg = $"[ThreadMgr]{e}";
        }
        finally
        {
            lock (_lock)
            {
                _exceptionDict[id] = msg;
            }
        }
    }

    protected override void UpdateEx(float interval)
    {
        base.UpdateEx(interval);
        lock (_lock)
        {
            if (_exceptionDict.Count < 1)
            {
                return;
            }
            foreach (var temp in _exceptionDict)
            {
                if (_onFinishDict.TryGetValue(temp.Key, out var onFinish))
                {
                    onFinish?.Invoke();
                    if (string.IsNullOrEmpty(temp.Value))
                    {
                        LogHelper.PrintError(temp.Value);
                    }
                }
            }
            _exceptionDict.Clear();
        }
    }

    protected override void OnDestroyEx()
    {
        base.OnDestroyEx();
        _onFinishDict.Clear();
        _exceptionDict.Clear();
    }
}