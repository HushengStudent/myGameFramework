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
            private float _curFps;
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
                this._updateInterval = updateInterval;
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

            public float CurrentFps
            {
                get
                {
                    return _curFps;
                }
            }

            public void UpdateFps(float elapseSeconds, float realElapseSeconds)
            {
                _frames++;
                _accumulator += realElapseSeconds;
                _timeLeft -= realElapseSeconds;

                if (_timeLeft <= 0f)
                {
                    _curFps = _accumulator > 0f ? _frames / _accumulator : 0f;
                    _frames = 0;
                    _accumulator = 0f;
                    _timeLeft += UpdateInterval;
                }
            }

            private void Reset()
            {
                _curFps = 0f;
                _frames = 0;
                _accumulator = 0f;
                _timeLeft = 0f;
            }
        }


    }
}