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
    public class EventMgr : Singleton<EventMgr>
    {
        /// <summary>
        /// 事件委托;
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public delegate bool EventDelegate(Object[] args);

        /// <summary>
        /// 委托集合;
        /// </summary>
        private Dictionary<EventType, Dictionary<Object, EventDelegate>> EventDict
            = new Dictionary<EventType, Dictionary<Object, EventDelegate>>();

        protected override void InitEx()
        {
            EventDict.Clear();
            LogUtil.LogUtility.Print("[EventMgr]EventMgr init!");
        }

        /// <summary>
        /// 添加事件;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <param name="callBack"></param>
        public void AddEvent(EventType type, Object target, EventDelegate callBack)
        {
            if (EventDict.ContainsKey(type))
            {
                if (EventDict[type].ContainsKey(target))
                {
                    LogUtil.LogUtility.PrintError(string.Format("[EventMgr]AddEvent error:Type:{0},Target:{1}!", type.ToString(), target.name));
                }
                else
                {
                    EventDict[type][target] = callBack;
                }
            }
            else
            {
                if (EventDict[type].ContainsKey(target))
                {
                    LogUtil.LogUtility.PrintError(string.Format("[EventMgr]AddEvent error:Type:{0},Target:{1}!", type.ToString(), target.name));
                }
                else
                {
                    EventDict[type][target] = callBack;
                }
            }
        }

        /// <summary>
        /// 移除事件;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public void RemoveEvent(EventType type, Object target)
        {
            if (EventDict.ContainsKey(type))
            {
                if (EventDict[type].ContainsKey(target))
                {
                    EventDict[type].Remove(target);
                }
            }
        }

        /// <summary>
        /// 广播事件;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <param name="args"></param>
        public void FireEvent(EventType type, Object target, Object[] args)
        {
            Dictionary<Object, EventDelegate> dict = null;
            if (EventDict.TryGetValue(type, out dict))
            {
                if (dict != null)
                {
                    EventDelegate eventDelegate = null;
                    if (dict.TryGetValue(target, out eventDelegate))
                    {
                        if (eventDelegate != null)
                        {
                            eventDelegate(args);
                        }
                    }
                }
            }
        }
    }
}