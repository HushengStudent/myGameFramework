/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/04 23:14:51
** desc:  时间管理;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class TimerMgr : MonoSingleton<TimerMgr>, ISingleton
    {
        #region Time Sync

        /// <summary>
        /// 服务器同步过来的时间;
        /// </summary>
        private DateTime serverTime;

        /// <summary>
        /// 客户端开启游戏时本机的时间,单位秒;
        /// </summary>
        private float clientStartTime;

        /// <summary>
        /// 当前服务器时间;
        /// </summary>
        public DateTime CurentSeverTime
        {
            get
            {
                return serverTime.AddSeconds(Time.realtimeSinceStartup - clientStartTime);
            }
        }
        /// <summary>
        /// 同步服务器的时间到客户端;
        /// </summary>
        /// <param name="time">服务器时间</param>
        public void SyncServerTime(DateTime time)
        {
            serverTime = time;
            clientStartTime = Time.realtimeSinceStartup;
            LogHelper.PrintWarning(string.Format("[Sync Server Time] ServerTime = {0}", serverTime));
        }

        #endregion

        #region Timer Event

        /// <summary>
        /// 定时器事件;
        /// </summary>
        public class TimerEvent
        {
            /// <summary>
            /// 首次执行延时;
            /// </summary>
            public float DelayTime;

            /// <summary>
            /// 执行次数;
            /// </summary>
            public int RepeatTimes;

            /// <summary>
            /// 已经执行的次数;
            /// </summary>
            public int ExeTimes;

            /// <summary>
            /// 执行间隔;
            /// </summary>
            public float IntervalTime;

            /// <summary>
            /// 无参回调函数;
            /// </summary>
            public Action CallBackFunc;

            /// <summary>
            /// 参数;
            /// </summary>
            public object Param;

            /// <summary>
            /// 包含参数的回调函数;
            /// </summary>
            public Action<object> CallBackFuncWithParam;

            /// <summary>
            /// 定时器事件结束后的回调函数;
            /// </summary>
            public Action OnFinish;

            /// <summary>
            /// 事件开始时间;
            /// </summary>
            public float StartTime;

            /// <summary>
            /// 下一次运行时间;
            /// </summary>
            public float NextRunTime;

            /// <summary>
            /// 事件是否已经结束;
            /// </summary>
            public bool IsFinish;

            /// <summary>
            /// 是否暂停;
            /// </summary>
            public bool IsPause;

            /// <summary>
            /// 当前时间;
            /// </summary>
            public float CurTime
            {
                get { return Time.realtimeSinceStartup; }
            }

            /// <summary>
            /// 时间差;
            /// </summary>
            public float DeltaTime
            {
                get { return Time.unscaledDeltaTime; }
            }

            /// <summary>
            /// 时间初始化;
            /// </summary>
            public void InitTimerEvent()
            {
                StartTime = CurTime;
                NextRunTime = StartTime + DelayTime;
            }

            /// <summary>
            /// 结束时移除所有事件;
            /// </summary>
            public void RemoveCallBackFunc()
            {
                CallBackFunc = null;
                CallBackFuncWithParam = null;
                OnFinish = null;
            }

            /// <summary>
            /// 执行回调;
            /// </summary>
            public void DoCallBack()
            {
                if (CallBackFunc != null)
                {
                    CallBackFunc();
                }
                if (CallBackFuncWithParam != null)
                {
                    CallBackFuncWithParam(Param);
                }
            }

            /// <summary>
            /// 移除事件判断函数;
            /// </summary>
            /// <returns></returns>
            public bool IsCanRemoveEvent()
            {
                if (IsFinish)
                {
                    return true;
                }
                if (CountDown())
                {
                    DoCallBack();
                    if (RepeatTimes >= 0 && ExeTimes >= RepeatTimes)
                    {
                        return true;
                    }
                    ExeTimes++;
                }
                return false;
            }

            /// <summary>
            /// 计时函数;
            /// </summary>
            /// <returns></returns>
            protected bool CountDown()
            {
                if (IsPause)
                {
                    NextRunTime += DeltaTime;
                }
                else if (NextRunTime <= CurTime)
                {
                    NextRunTime += IntervalTime;
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Timer Event Register

        private List<TimerEvent> EventLists = new List<TimerEvent>();

        public void OnInitialize()
        {
            EventLists.Clear();
        }

        /// <summary>
        /// 注册计时事件;
        /// </summary>
        /// <param name="delayTime">第一次执行延时(s)</param>
        /// <param name="repeatTimes">重复执行次数(s)</param>
        /// <param name="intervalTime">两次执行间隔时间(s)</param>
        /// <param name="callBackFunc">无参回调函数</param>
        /// <param name="callBackFuncWithParam">回调函数</param>
        /// <param name="param">参数</param>
        /// <param name="onFinish">事件结束回调函数</param>
        /// <returns></returns>
        public TimerHandler RegisterTimerEvent(float delayTime, int repeatTimes, float intervalTime, Action callBackFunc,
            Action<object> callBackFuncWithParam, object param, Action onFinish)
        {
            TimerEvent timerEvent = new TimerEvent
            {
                DelayTime = delayTime,
                RepeatTimes = repeatTimes,
                IntervalTime = intervalTime,
                CallBackFunc = callBackFunc,
                CallBackFuncWithParam = callBackFuncWithParam,
                Param = param,
                OnFinish = onFinish,
                IsFinish = false,
                IsPause = false
            };
            timerEvent.InitTimerEvent();
            EventLists.Add(timerEvent);
            TimerHandler handler = new TimerHandler(timerEvent);
            return handler;
        }

        /// <summary>
        /// 注册计时事件;
        /// </summary>
        /// <param name="delayTime">第一次执行延时(s)</param>
        /// <param name="callBackFunc">回调函数</param>
        /// <param name="onFinish">结束回调函数</param>
        /// <returns></returns>
        public TimerHandler RegisterTimerEvent(float delayTime, Action callBackFunc, Action onFinish)
        {
            return RegisterTimerEvent(delayTime, 0, 1, callBackFunc, null, null, onFinish);
        }

        #endregion

        #region Timer Handler

        /// <summary>
        /// 计时事件操作类;
        /// </summary>
        public class TimerHandler
        {
            /// <summary>
            /// 计时事件;
            /// </summary>
            private TimerEvent mTimer;

            public TimerHandler(TimerEvent timer)
            {
                mTimer = timer;
            }
            /// <summary>
            /// 暂停;
            /// </summary>
            public void Pause()
            {
                if (mTimer != null) mTimer.IsPause = true;
            }
            /// <summary>
            /// 继续;
            /// </summary>
            public void Replay()
            {
                if (mTimer != null) mTimer.IsPause = false;
            }
            /// <summary>
            /// 移除时间;
            /// </summary>
            public void RemoveTimer()
            {
                mTimer.IsFinish = true;
                mTimer.RemoveCallBackFunc();
                mTimer = null;
            }
        }

        #endregion

        #region Function

        private TimerEvent tmpEvent = null;

        // Update is called once per frame
        void Update()
        {
            for (int i = EventLists.Count - 1; i >= 0; --i)
            {
                tmpEvent = EventLists[i];
                try
                {
                    if (tmpEvent.IsCanRemoveEvent())
                    {
                        tmpEvent.IsFinish = true;
                    }
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                    tmpEvent.IsFinish = true;
                }
            }

            for (int i = EventLists.Count - 1; i >= 0; --i)
            {
                tmpEvent = EventLists[i];
                if (tmpEvent.IsFinish)
                {
                    try
                    {
                        if (tmpEvent.OnFinish != null)
                            tmpEvent.OnFinish();
                    }
                    catch (Exception exp)
                    {
                        Debug.LogException(exp);
                    }
                    finally
                    {
                        EventLists.Remove(tmpEvent);
                    }
                }
            }
        }

        #endregion
    }
}
