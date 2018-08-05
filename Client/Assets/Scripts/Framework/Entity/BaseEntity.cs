/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:32:30
** desc:  ECS实体抽象基类;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BaseEntity : IPool
    {
        private long _id;
        private ulong _uid;
        private bool _enable = false;
        private string _entityName = string.Empty;
        private GameObject _entityObject = null;
        private EntityInitEventHandler _entityInitHandler;

        public long ID { get { return _id; } }
        public ulong UID { get { return _uid; } }
        public bool Enable { get { return _enable; } set { _enable = value; } }
        public string EntityName { get { return _entityName; } set { _entityName = value; } }
        public GameObject EntityObject { get { return _entityObject; } set { _entityObject = value; } }
        public EntityInitEventHandler EntityInitHandler { get { return _entityInitHandler; } set { _entityInitHandler = value; } }

        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }

        /// <summary>
        /// 初始化Entity;
        /// </summary>
        /// <param name="go"></param>
        public void Create(GameObject go, ulong uid, string name)
        {
            _id = IdGenerater.GenerateId();
            _uid = uid;
            _entityName = name;
            OnAttachEntityObject(go);
            EventSubscribe();
            OnInit();
            _enable = true;
            if (EntityInitHandler != null)
            {
                EntityInitHandler(this);
            }
        }
        /// <summary>
        /// 重置Entity;
        /// </summary>
        public void Reset()
        {
            DeAttachEntityObject();
            EventUnsubscribe();
            OnReset();
            _id = 0;
            _enable = false;
            _entityInitHandler = null;
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
        /// Entity附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnAttachEntityObject(GameObject go)
        {
            _entityObject = go;
        }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachEntityObject()
        {
            _entityObject = null;
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
