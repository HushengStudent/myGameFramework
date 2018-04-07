/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:22:57
** desc:  ECS组件抽象基类
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
    public abstract class AbsComponent : IPool
    {
        private long _id;
        private bool _enable = false;
        private AbsEntity _entity;
        private GameObject _componentGo = null;
        private Action<AbsComponent> _initCallBack;

        public long ID { get { return _id; } }
        public bool Enable { get { return _enable; } set { _enable = value; } }
        public AbsEntity Entity { get { return _entity; } set { _entity = value; } }
        public GameObject ComponentGo { get { return _componentGo; } set { _componentGo = value; } }
        public Action<AbsComponent> InitCallBack { get { return _initCallBack; } set { _initCallBack = value; } }

        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }

        /// <summary>
        /// 初始化Component;
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="go">gameObject</param>
        public virtual void OnInitComponent(AbsEntity entity, GameObject go)
        {
            _id = IdGenerater.GenerateId();
            OnAttachEntity(entity);
            OnAttachComponentGo(go);
            EventSubscribe();
            if (InitCallBack != null)
            {
                InitCallBack(this);
            }
            _enable = true;
        }
        /// <summary>
        /// 重置Component;
        /// </summary>
        public virtual void OnResetComponent()
        {
            DeAttachEntity();
            DeAttachComponentGo();
            EventUnsubscribe();
            _id = 0;
            _entity = null;
            _enable = false;
            _componentGo = null;
            _initCallBack = null;
        }
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
        protected virtual void OnAttachComponentGo(GameObject go)
        {
            ComponentGo = go;
        }
        /// <summary>
        /// 重置Entity的附加;
        /// </summary>
        protected virtual void DeAttachEntity() { }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachComponentGo() { }
        /// <summary>
        /// 注册事件;
        /// </summary>
        protected virtual void EventSubscribe() { }
        /// <summary>
        /// 注销事件;
        /// </summary>
        protected virtual void EventUnsubscribe() { }
        /// <summary>
        /// 对象池Get;
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnGet(params System.Object[] args) { }
        /// <summary>
        /// 对象池Release;
        /// </summary>
        public virtual void OnRelease() { }
    }
}
