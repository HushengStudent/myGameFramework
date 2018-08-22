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

        private AbsEntity _entity;

        public AbsEntity Entity { get { return _entity; } }
        public ComponentInitEventHandler ComponentInitHandler { get; set; }

        public virtual void FixedUpdateEx(float interval) { }
        public virtual void UpdateEx(float interval) { }
        public virtual void LateUpdateEx(float interval) { }

        /// <summary>
        /// 初始化Component;
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="go">gameObject</param>
        public void Create(AbsEntity entity)
        {
            OnAttachEntity(entity);
            //OnAttachGoEx(go);
            EventSubscribe();
            OnInit();
            Enable = true;
            if (ComponentInitHandler != null)
            {
                ComponentInitHandler(this);
            }
        }
        /// <summary>
        /// 重置Component;
        /// </summary>
        public void Reset()
        {
            DeAttachEntity();
            DeAttachGoEx();
            EventUnsubscribe();
            OnReset();
            Enable = false;
            ComponentInitHandler = null;
        }
        /// <summary>
        /// 初始化;
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// 重置;
        /// </summary>
        protected virtual void OnReset() { }
        /// <summary>
        /// Component附加Entity;
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnAttachEntity(AbsEntity entity)
        {
            _entity = entity;
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
            _entity = null;
        }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachGoEx() { }
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
    }
}
