/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:32:30
** desc:  ECS实体抽象基类
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsEntity : IPool
    {
        private long _id;
        private bool _enable = false;
        private GameObject _entityGo = null;
        private Action<AbsEntity> _initCallBack;

        public long ID { get { return _id; } }
        public bool Enable { get { return _enable; } set { _enable = value; } }
        public GameObject EntityGO { get { return _entityGo; } set { _entityGo = value; } }
        public Action<AbsEntity> InitCallBack { get { return _initCallBack; } set { _initCallBack = value; } }

        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }

        /// <summary>
        /// 初始化Entity;
        /// </summary>
        /// <param name="go"></param>
        public virtual void OnInitEntity(GameObject go)
        {
            _id = IdGenerater.GenerateId();
            OnAttachEntityGo(go);
            EventSubscribe();
            if (InitCallBack != null)
            {
                InitCallBack(this);
            }
            _enable = true;
        }
        /// <summary>
        /// 重置Entity;
        /// </summary>
        protected virtual void OnResetEntity()
        {
            DeAttachEntityGo();
            EventUnsubscribe();
            _id = 0;
            _enable = false;
            _entityGo = null;
            _initCallBack = null;
        }
        /// <summary>
        /// Entity附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnAttachEntityGo(GameObject go)
        {
            _entityGo = go;
        }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachEntityGo() { }
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
