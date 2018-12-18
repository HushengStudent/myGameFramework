/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:22:57
** desc:  ECS组件抽象基类;
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 组件抽象基类;
    /// </summary>
    public abstract class AbsComponent : ObjectEx
    {
        protected AbsComponent() : base() { }

        public ComponentInitEventHandler ComponentInitHandler = null;

        public AbsEntity Entity { get; private set; }

        public virtual void FixedUpdateEx(float interval) { }
        public virtual void UpdateEx(float interval) { }
        public virtual void LateUpdateEx(float interval) { }

        /// <summary>
        /// 初始化Component;
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="go">gameObject</param>
        public void Init(AbsEntity entity)
        {
            OnAttachEntity(entity);
            if (Entity.gameObjectEx.IsLoadFinish)
            {
                OnAttachGoEx(Entity.gameObjectEx);
            }
            else
            {
                Entity.gameObjectEx.AddLoadFinishHandler(OnAttachGoEx);
            }
            EventSubscribe();
            InitEx();
            Enable = true;
            if (ComponentInitHandler != null)
            {
                ComponentInitHandler(this);
            }
        }
        /// <summary>
        /// 重置Component;
        /// </summary>
        public void Uninit()
        {
            DeAttachEntity();
            DeAttachGoEx();
            EventUnsubscribe();
            UninitEx();
            Enable = false;
            ComponentInitHandler = null;
        }
        /// <summary>
        /// 初始化;
        /// </summary>
        protected virtual void InitEx() { }
        /// <summary>
        /// 重置;
        /// </summary>
        protected virtual void UninitEx() { }
        /// <summary>
        /// Component附加Entity;
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnAttachEntity(AbsEntity entity)
        {
            Entity = entity;
        }
        /// <summary>
        /// Component附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnAttachGoEx(GameObjectEx goEx) { }
        /// <summary>
        /// 重置Entity的附加;
        /// </summary>
        protected virtual void DeAttachEntity()
        {
            Entity = null;
        }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachGoEx()
        {
            if (!Entity.gameObjectEx.IsLoadFinish)
            {
                Entity.gameObjectEx.RemoveLoadFinishHandler(OnAttachGoEx);
            }
        }
        /// <summary>
        /// 注册事件;
        /// </summary>
        protected virtual void EventSubscribe() { }
        /// <summary>
        /// 注销事件;
        /// </summary>
        protected virtual void EventUnsubscribe() { }
        /// <summary>
        /// 进入场景;
        /// </summary>
        /// <param name="sceneId"></param>
        protected virtual void OnEnterScene(int sceneId) { }
        /// <summary>
        /// 离开场景;
        /// </summary>
        /// <param name="sceneId"></param>
        protected virtual void OnExitScene(int sceneId) { }

        protected void AddEvent(EventType type, EventHandler handler)
        {
            EventMgr.Instance.AddEvent(Entity, type, handler);
        }

        protected void RemoveEvent(EventType type)
        {
            EventMgr.Instance.RemoveEvent(Entity, type);
        }

        protected void FireEvent(EventType type, IEventArgs eventArgs)
        {
            EventMgr.Instance.FireEvent(Entity, type, eventArgs);
        }
    }
}
