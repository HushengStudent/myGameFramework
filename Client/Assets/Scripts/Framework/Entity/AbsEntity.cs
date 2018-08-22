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
        protected AbsEntity() : base() { }

        private ulong _uid;
        private int _entityId;
        private string _entityName = string.Empty;
        private string _resPath = string.Empty;
        private GameObjectEx _goEx = null;
        private EntityLoadFinishEventHandler _entityLoadFinishHandler = null;


        public ulong UID { get { return _uid; } }
        public int EntityId { get { return _entityId; } }
        public string EntityName { get { return _entityName; } }
        public string ResPath { get { return _resPath; } }
        public GameObjectEx GoEx { get { return _goEx; } }
        public EntityInitEventHandler EntityInitHandler { get; set; }
        public EntityLoadFinishEventHandler EntityLoadFinishHandler
        {
            get { return _entityLoadFinishHandler; }
            set
            {
                if (_goEx != null && _goEx.Go != null)
                {
                    _entityLoadFinishHandler = value;
                    _entityLoadFinishHandler(this, _goEx.Go);
                }
            }
        }

        public virtual void FixedUpdateEx(float interval) { }
        public virtual void UpdateEx(float interval) { }
        public virtual void LateUpdateEx(float interval) { }

        /// <summary>
        /// 初始化Entity;
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        public void Create(int entityId, ulong uid, string name)
        {
            _uid = uid;
            _entityName = name;
            _entityId = entityId;

            _goEx = PoolMgr.Instance.Get<GameObjectEx>();
            _goEx.Init(this, _resPath, OnAttachGoEx);

            EventSubscribe();
            OnInitEx();
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
            DeAttachGoEx();
            EventUnsubscribe();
            OnResetEx();
            Enable = false;
            EntityInitHandler = null;
            EntityLoadFinishHandler = null;
        }

        /// <summary>
        /// 初始化;
        /// </summary>
        protected virtual void OnInitEx() { }
        /// <summary>
        /// 重置;
        /// </summary>
        protected virtual void OnResetEx() { }
        /// <summary>
        /// Entity附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnAttachGoEx(GameObjectEx go)
        {
            _goEx = go;
            if (_entityLoadFinishHandler != null)
            {
                _entityLoadFinishHandler(this, _goEx.Go);
            }
        }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachGoEx()
        {
            _goEx.Uninit();
            PoolMgr.Instance.Release<GameObjectEx>(_goEx);
            _goEx = null;
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
    }
}
