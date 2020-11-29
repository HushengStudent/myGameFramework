/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/03/22 23:12:49
** desc:  事件系统;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 事件委托;
    /// </summary>
    /// <param name="eventArgs">事件参数</param>
    /// <returns></returns>
    public delegate void EventHandler(IEventArgs eventArgs);

    public class EventMgr : Singleton<EventMgr>
    {
        /// <summary>
        /// 委托集合;
        /// </summary>
        private Dictionary<ObjectEx, Dictionary<EventType, List<EventHandler>>> _eventDict
            = new Dictionary<ObjectEx, Dictionary<EventType, List<EventHandler>>>();

        /// <summary>
        /// 委托集合;
        /// </summary>
        private Dictionary<EventType, List<EventHandler>> _globalEventDict
            = new Dictionary<EventType, List<EventHandler>>();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _eventDict.Clear();
            _globalEventDict.Clear();
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
            if (!_eventDict.TryGetValue(receiver, out var dict))
            {
                _eventDict[receiver] = new Dictionary<EventType, List<EventHandler>>();
            }
            dict = _eventDict[receiver];
            if (!dict.TryGetValue(type, out var list))
            {
                list = new List<EventHandler>();
                dict[type] = list;
            }
            list = dict[type];
            if (list.Contains(callBack))
            {
                LogHelper.PrintWarning($"[EventMgr]AddEvent repeat,receiver:{receiver.ID},eventType:{type.ToString()}.");
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
            if (_eventDict.TryGetValue(receiver, out var dict) && null != dict && dict.Count > 0)
            {
                if (dict.ContainsKey(type))
                {
                    dict.Remove(type);
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
            if (_eventDict.TryGetValue(receiver, out var dict))
            {
                _eventDict.Remove(receiver);
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
            if (_eventDict.TryGetValue(receiver, out var dict) && null != dict && dict.Count > 0)
            {
                if (dict.TryGetValue(type, out var list) && null != list && list.Count > 0)
                {
                    foreach (var callback in list)
                    {
                        callback?.Invoke(eventArgs);
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
            if (!_globalEventDict.TryGetValue(type, out var list))
            {
                _globalEventDict[type] = new List<EventHandler>();
            }
            list = _globalEventDict[type];
            if (list.Contains(callBack))
            {
                LogHelper.PrintWarning($"[EventMgr]AddGlobalEvent repeat,EventType:{type.ToString()}.");
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
            if (_globalEventDict.ContainsKey(type))
            {
                _globalEventDict.Remove(type);
            }
        }

        /// <summary>
        /// 广播事件;
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="eventArgs">事件参数</param>
        public void FireGlobalEvent(EventType type, IEventArgs eventArgs)
        {
            if (_globalEventDict.TryGetValue(type, out var list) && null != list && list.Count > 0)
            {
                foreach (var callback in list)
                {
                    callback?.Invoke(eventArgs);
                }
            }
        }
    }
}