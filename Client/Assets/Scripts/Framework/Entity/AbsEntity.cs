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
    public abstract class AbsEntity : ObjectEx
    {
        private ulong _uid;
        private string _entityName = string.Empty;
        private string _resPath = string.Empty;
        private GameObject _entityObject = null;

        public ulong UID { get { return _uid; } }
        public string EntityName { get { return _entityName; } }
        public string ResPath { get { return _resPath; } }
        public GameObject EntityObject { get { return _entityObject; } set { _entityObject = value; } }
        public EntityInitEventHandler EntityInitHandler { get; set; }
        public EntityLoadFinishEventHandler EntityLoadFinishHandler { get; set; }

        public virtual void FixedUpdateEx(float interval) { }
        public virtual void UpdateEx(float interval) { }
        public virtual void LateUpdateEx(float interval) { }

        /// <summary>
        /// 初始化Entity;
        /// </summary>
        /// <param name="go"></param>
        public void Create(GameObject go, ulong uid, string name)
        {
            _uid = uid;
            _entityName = name;
            OnAttachEntityObject(go);
            EventSubscribe();
            OnInit();
            Enable = true;
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
            Enable = false;
            EntityInitHandler = null;
            EntityLoadFinishHandler = null;
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

        private void LoadEntitySync()
        {
            //TODO;
            if (EntityLoadFinishHandler != null)
            {
                EntityLoadFinishHandler(this, EntityObject);
            }
        }

        private IEnumerator LoadEntityAsyn()
        {
            IEnumerator itor = null;
            //TODO;
            if (EntityLoadFinishHandler != null)
            {
                EntityLoadFinishHandler(this, EntityObject);
            }
            return itor;
        }
    }
}
