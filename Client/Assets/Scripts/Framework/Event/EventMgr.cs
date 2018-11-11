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
    /// <param name="sender">发送者</param>
    /// <param name="eventArgs">事件参数</param>
    /// <returns></returns>
    public delegate bool EventHandler(AbsEntity sender, IEventArgs eventArgs);

    /// <summary>
    /// 事件委托;
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <returns></returns>
    public delegate bool GlobalEventHandler(IEventArgs eventArgs);

    public class EventMgr : Singleton<EventMgr>
    {
        /// <summary>
        /// 委托集合;
        /// </summary>
        private Dictionary<EventType, Dictionary<AbsEntity, List<EventHandler>>> EventDict
            = new Dictionary<EventType, Dictionary<AbsEntity, List<EventHandler>>>();

        /// <summary>
        /// 委托集合;
        /// </summary>
        private Dictionary<EventType, List<GlobalEventHandler>> GlobalEventDict
            = new Dictionary<EventType, List<GlobalEventHandler>>();

        public override void Init()
        {
            base.Init();
            EventDict.Clear();
            GlobalEventDict.Clear();
        }

        /// <summary>
        /// 添加事件;
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="receiver">接收者</param>
        /// <param name="callBack">事件回调</param>
        public void AddEvent(EventType type, AbsEntity receiver, EventHandler callBack)
        {
            if (null == receiver)
            {
                LogUtil.LogUtility.PrintError("[EventMgr]AddEvent error,the receiver is null.");
            }
            Dictionary<AbsEntity, List<EventHandler>> dict;
            List<EventHandler> list;
            if (!EventDict.TryGetValue(type, out dict))
            {
                EventDict[type] = new Dictionary<AbsEntity, List<EventHandler>>();
            }
            dict = EventDict[type];
            if (!dict.TryGetValue(receiver, out list))
            {
                list = new List<EventHandler>();
                dict[receiver] = list;
            }
            list = dict[receiver];
            if (list.Contains(callBack))
            {
                LogUtil.LogUtility.PrintWarning(string.Format("[EventMgr]AddEvent repeat,EventType:{0},Receiver:{1}.",
                    type.ToString(), receiver.EntityName));
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
        /// <param name="receiver">接收者</param>
        public void RemoveEvent(EventType type, AbsEntity receiver)
        {
            if (null == receiver)
            {
                LogUtil.LogUtility.PrintError("[EventMgr]RemoveEvent error,the receiver is null.");
            }
            Dictionary<AbsEntity, List<EventHandler>> dict;
            if (EventDict.TryGetValue(type, out dict))
            {
                if (null != dict && dict.Count > 0)
                {
                    if (dict.ContainsKey(receiver))
                    {
                        dict.Remove(receiver);
                    }
                }
            }
        }

        /// <summary>
        /// 广播事件;
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="sender">发送者</param>
        /// <param name="receiver">接收者</param>
        /// <param name="eventArgs">事件参数</param>
        public void FireEvent(EventType type, AbsEntity sender, AbsEntity receiver, IEventArgs eventArgs)
        {
            if (null == receiver)
            {
                LogUtil.LogUtility.PrintError("[EventMgr]RemoveEvent error,the receiver is null.");
            }
            if (null == sender)
            {
                LogUtil.LogUtility.PrintError("[EventMgr]RemoveEvent error,the sender is null.");
            }
            Dictionary<AbsEntity, List<EventHandler>> dict;
            List<EventHandler> list;
            if (EventDict.TryGetValue(type, out dict))
            {
                if (null != dict && dict.Count > 0)
                {
                    if (dict.TryGetValue(receiver, out list))
                    {
                        if (null != list && list.Count > 0)
                        {
                            foreach (var callback in list)
                            {
                                callback(sender, eventArgs);
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
        public void AddGlobalEvent(EventType type, GlobalEventHandler callBack)
        {
            List<GlobalEventHandler> list;
            if (!GlobalEventDict.TryGetValue(type, out list))
            {
                GlobalEventDict[type] = new List<GlobalEventHandler>();
            }
            list = GlobalEventDict[type];
            if (list.Contains(callBack))
            {
                LogUtil.LogUtility.PrintWarning(string.Format("[EventMgr]AddGlobalEvent repeat,EventType:{0}.",
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
            List<GlobalEventHandler> list;
            if (GlobalEventDict.TryGetValue(type, out list))
            {
                if (null != list && list.Count > 0)
                {
                    foreach (var callback in list)
                    {
                        callback(eventArgs);
                    }
                }
            }
        }
    }
}