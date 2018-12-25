/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/20 01:44:34
** desc:  Object扩展;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ObjectEx : IPool
    {
        protected ObjectEx()
        {
            ID = IdGenerateHelper.GenerateId();
            Enable = false;
            ComponentList.Clear();
        }

        public bool Enable;

        public long ID { get; private set; }

        public List<AbsComponent> ComponentList = new List<AbsComponent>();

        public bool AddComponent(AbsComponent component)
        {
            for (int i = 0; i < ComponentList.Count; i++)
            {
                var target = ComponentList[i];
                if (target == component)
                {
                    LogHelper.PrintError("[ComponentMgr]AddComponent " + component.GetType().ToString() + " repeat!");
                    return false;
                }
            }
            return true;
        }

        public bool RemoveComponent(AbsComponent component)
        {
            for (int i = 0; i < ComponentList.Count; i++)
            {
                var target = ComponentList[i];
                if (target == component)
                {
                    component.Uninit();
                    return true;
                }
            }
            LogHelper.PrintError("[ComponentMgr]RemoveComponent " + component.GetType().ToString() + " error!");
            return true;
        }

        /// <summary>
        /// 对象池Get;
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnGet(params System.Object[] args) { }

        /// <summary>
        /// 对象池Release;
        /// </summary>
        public virtual void OnRelease() { }

        protected void AddEvent(EventType type, EventHandler handler)
        {
            EventMgr.Instance.AddEvent(this, type, handler);
        }

        protected void RemoveEvent(EventType type)
        {
            EventMgr.Instance.RemoveEvent(this, type);
        }

        protected void FireEvent(EventType type, IEventArgs eventArgs)
        {
            EventMgr.Instance.FireEvent(this, type, eventArgs);
        }
    }
}