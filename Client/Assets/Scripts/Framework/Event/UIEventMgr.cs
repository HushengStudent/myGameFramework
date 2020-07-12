/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 23:46:32
** desc:  UI事件系统;
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class UIEventMgr<TKey> where TKey : struct
    {
        private static Dictionary<TKey, List<Delegate>> eventDict = new Dictionary<TKey, List<Delegate>>();

        public static void RemoveEvent(TKey key)
        {
            eventDict.Remove(key);
        }

        public static void Init()
        {
            eventDict.Clear();
        }

        private static bool JudeEvent(TKey key, Delegate delegateFunc)
        {
            if (eventDict.TryGetValue(key, out var eventList))
            {
                if (null == eventList || eventList.Count == 0)
                {
                    eventDict[key] = new List<Delegate>();
                    return true;
                }
                foreach (Delegate func in eventList)
                {
                    if (func == delegateFunc) return false;
                }
            }
            else
            {
                eventDict[key] = new List<Delegate>();
            }
            return true;
        }

        #region Action

        /// <summary>
        /// 注册无参Action委托;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public static void AddEvent(TKey key, Action handler)
        {
            if (JudeEvent(key, handler))
            {
                eventDict[key].Add(handler);
            }
            else
            {
                Debug.LogError($"[EventMgr]Register {key.ToString()} failure!");
            }
        }

        /// <summary>
        /// 移除无参Action委托;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public static void RemoveEvent(TKey key, Action handler)
        {
            if (!JudeEvent(key, handler))
            {
                eventDict[key].Remove(handler);
            }
            else
            {
                Debug.LogError($"[EventMgr]Remove {key.ToString()} failure!");
            }
        }

        /// <summary>
        /// 广播无参Action委托;
        /// </summary>
        /// <param name="key"></param>
        public static void FireEvent(TKey key)
        {
            if (eventDict.TryGetValue(key, out var eventList))
            {
                Action callback;
                for (int i = 0; i < eventList.Count; ++i)
                {
                    try
                    {
                        callback = (eventList[i]) as Action;
                        callback?.Invoke();
                    }
                    catch (Exception exp)
                    {
                        Debug.LogException(exp);
                    }
                }
            }
        }

        #endregion

        #region Action<T>

        /// <summary>
        /// 注册含一个参数的委托;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public static void AddEvent<T>(TKey key, Action<T> handler)
        {
            if (JudeEvent(key, handler))
            {
                eventDict[key].Add(handler);
            }
            else
            {
                Debug.LogError($"[EventMgr]Register {key.ToString()} failure!");
            }
        }

        /// <summary>
        /// 移除含一个参数的Action委托;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public static void RemoveEvent<T>(TKey key, Action<T> handler)
        {
            if (!JudeEvent(key, handler))
            {
                eventDict[key].Remove(handler);
            }
            else
            {
                Debug.LogError($"[EventMgr]Remove {key.ToString()} failure!");
            }
        }

        /// <summary>
        /// 广播含一个参数的Action委托;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="arg1"></param>
        public static void FireEvent<T>(TKey key, T arg1)
        {
            if (eventDict.TryGetValue(key, out var eventList))
            {
                Action<T> callback;
                for (int i = 0; i < eventList.Count; ++i)
                {
                    try
                    {
                        callback = (eventList[i]) as Action<T>;
                        callback?.Invoke(arg1);
                    }
                    catch (Exception exp)
                    {
                        Debug.LogException(exp);
                    }
                }
            }
        }

        #endregion

        #region Action<T1,T2>

        /// <summary>
        /// 注册含两个参数的Action委托;
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public static void AddEvent<T1, T2>(TKey key, Action<T1, T2> handler)
        {
            if (JudeEvent(key, handler))
            {
                eventDict[key].Add(handler);
            }
            else
            {
                Debug.LogError($"[EventMgr]Register {key.ToString()} failure!");
            }
        }

        /// <summary>
        /// 移除含两个参数的Action委托;
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public static void RemoveEvent<T1, T2>(TKey key, Action<T1, T2> handler)
        {
            if (!JudeEvent(key, handler))
            {
                eventDict[key].Remove(handler);
            }
            else
            {
                Debug.LogError($"[EventMgr]Remove {key.ToString()} failure!");
            }
        }

        /// <summary>
        /// 广播含两个参数的Action委托
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="key"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public static void FireEvent<T1, T2>(TKey key, T1 arg1, T2 arg2)
        {
            if (eventDict.TryGetValue(key, out var eventList))
            {
                Action<T1, T2> callback;
                for (int i = 0; i < eventList.Count; ++i)
                {
                    try
                    {
                        callback = (eventList[i]) as Action<T1, T2>;
                        callback?.Invoke(arg1, arg2);
                    }
                    catch (Exception exp)
                    {
                        Debug.LogException(exp);
                    }
                }
            }
        }

        #endregion
    }
}
