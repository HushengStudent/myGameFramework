/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/21 23:13:04
** desc:  fps;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public partial class DebugMgr
    {
        private class DebugFps
        {
            private float _updateInterval;
            private int _frames;
            private float _accumulator;
            private float _timeLeft;

            public DebugFps(float updateInterval)
            {
                //Application.targetFrameRate = 30;
                if (updateInterval <= 0f)
                {
                    Debug.LogError("Update interval is invalid.");
                    return;
                }
                _updateInterval = updateInterval;
                Reset();
            }

            public float UpdateInterval
            {
                get
                {
                    return _updateInterval;
                }
                set
                {
                    if (value <= 0f)
                    {
                        Debug.LogError("Update interval is invalid.");
                        return;
                    }
                    _updateInterval = value;
                    Reset();
                }
            }

            public float CurrentFps { get; private set; }

            public void UpdateFps(float elapseSeconds, float realElapseSeconds)
            {
                _frames++;
                _accumulator += realElapseSeconds;
                _timeLeft -= realElapseSeconds;

                if (_timeLeft <= 0f)
                {
                    CurrentFps = _accumulator > 0f ? _frames / _accumulator : 0f;
                    _frames = 0;
                    _accumulator = 0f;
                    _timeLeft += UpdateInterval;
                }
            }

            private void Reset()
            {
                CurrentFps = 0f;
                _frames = 0;
                _accumulator = 0f;
                _timeLeft = 0f;
            }
        }
    }
}