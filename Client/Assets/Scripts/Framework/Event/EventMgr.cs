/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/22 23:12:49
** desc:  事件系统;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 事件委托;
    /// </summary>
    /// <param name="eventArgs">事件参数</param>
    /// <returns></returns>
    public delegate bool EventHandler(IEventArgs eventArgs);

    public class EventMgr : Singleton<EventMgr>
    {
        /// <summary>
        /// 委托集合;
        /// </summary>
        private Dictionary<ObjectEx, Dictionary<EventType, List<EventHandler>>> EventDict
            = new Dictionary<ObjectEx, Dictionary<EventType, List<EventHandler>>>();

        /// <summary>
        /// 委托集合;
        /// </summary>
        private Dictionary<EventType, List<EventHandler>> GlobalEventDict
            = new Dictionary<EventType, List<EventHandler>>();

        protected override void CreateInstance()
        {
            base.CreateInstance();
            EventDict.Clear();
            GlobalEventDict.Clear();
        }

        /// <summary>
        /// 添加事件;
        /// </summary>
        /// <param name="receiver">接收者</param>
        /// <param name="type">事件类型</param>
        /// <param name="callBack">事件回调</param>
        public void AddEvent(ObjectEx receiver, EventType type, EventHandler callBack)
        {
            if (null == receiver)
            {
                LogHelper.PrintError("[EventMgr]AddEvent error,the receiver is null.");
                return;
            }
            Dictionary<EventType, List<EventHandler>> dict;
            List<EventHandler> list;
            if (!EventDict.TryGetValue(receiver, out dict))
            {
                EventDict[receiver] = new Dictionary<EventType, List<EventHandler>>();
            }
            dict = EventDict[receiver];
            if (!dict.TryGetValue(type, out list))
            {
                list = new List<EventHandler>();
                dict[type] = list;
            }
            list = dict[type];
            if (list.Contains(callBack))
            {
                LogHelper.PrintWarning(string.Format("[EventMgr]AddEvent repeat,receiver:{0},eventType:{1}.",
                    receiver.ID, type.ToString()));
            }
            else
            {
                list.Add(callBack);
            }
        }

        /// <summary>
        /// 移除事件;
        /// </summary>
        /// <param name="receiver">接收者</param>
        /// <param name="type">事件类型</param>
        public void RemoveEvent(ObjectEx receiver, EventType type)
        {
            if (null == receiver)
            {
                LogHelper.PrintError("[EventMgr]RemoveEvent error,the receiver is null.");
                return;
            }
            Dictionary<EventType, List<EventHandler>> dict;
            if (EventDict.TryGetValue(receiver, out dict))
            {
                if (null != dict && dict.Count > 0)
                {
                    if (dict.ContainsKey(type))
                    {
                        dict.Remove(type);
                    }
                }
            }
        }

        /// <summary>
        /// 移除事件;
        /// </summary>
        /// <param name="receiver">接收者</param>
        public void RemoveEvent(ObjectEx receiver)
        {
            if (null == receiver)
            {
                LogHelper.PrintError("[EventMgr]RemoveEvent error,the receiver is null.");
                return;
            }
            Dictionary<EventType, List<EventHandler>> dict;
            if (EventDict.TryGetValue(receiver, out dict))
            {
                EventDict.Remove(receiver);
            }
        }

        /// <summary>
        /// 广播事件;
        /// </summary>
        /// <param name="receiver">接收者</param>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        public void FireEvent(ObjectEx receiver, EventType type, IEventArgs eventArgs)
        {
            if (null == receiver)
            {
                LogHelper.PrintError("[EventMgr]FireEvent error,the receiver is null.");
                return;
            }
            Dictionary<EventType, List<EventHandler>> dict;
            List<EventHandler> list;
            if (EventDict.TryGetValue(receiver, out dict))
            {
                if (null != dict && dict.Count > 0)
                {
                    if (dict.TryGetValue(type, out list))
                    {
                        if (null != list && list.Count > 0)
                        {
                            foreach (var callback in list)
                            {
                                if (callback != null)
                                {
                                    callback(eventArgs);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加事件;
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callBack">事件回调</param>
        public void AddGlobalEvent(EventType type, EventHandler callBack)
        {
            List<EventHandler> list;
            if (!GlobalEventDict.TryGetValue(type, out list))
            {
                GlobalEventDict[type] = new List<EventHandler>();
            }
            list = GlobalEventDict[type];
            if (list.Contains(callBack))
            {
                LogHelper.PrintWarning(string.Format("[EventMgr]AddGlobalEvent repeat,EventType:{0}.",
                    type.ToString()));
            }
            else
            {
                list.Add(callBack);
            }
        }

        /// <summary>
        /// 移除事件;
        /// </summary>
        /// <param name="type">事件类型</param>
        public void RemoveGlobalEvent(EventType type)
        {
            if (GlobalEventDict.ContainsKey(type))
            {
                GlobalEventDict.Remove(type);
            }
        }

        /// <summary>
        /// 广播事件;
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        public void FireGlobalEvent(EventType type, IEventArgs eventArgs)
        {
            List<EventHandler> list;
            if (GlobalEventDict.TryGetValue(type, out list))
            {
                if (null != list && list.Count > 0)
                {
                    foreach (var callback in list)
                    {
                        if (callback != null)
                        {
                            callback(eventArgs);
                        }
                    }
                }
            }
        }
    }
}