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
    public enum EntityTypeEnum : byte
    {
        Non = 0,
        Role = 1,
        Player = 2,
        Monster = 3,
    }

    public abstract class AbsEntity : ObjectEx
    {
        protected AbsEntity() : base() { }

        private ulong _uid;
        private int _entityId;
        private string _entityName = string.Empty;
        private string _resPath = string.Empty;
        private GameObjectEx _gameObjectEx = null;
        private EntityLoadFinishEventHandler _entityLoadFinishHandler = null;

        public ulong UID { get { return _uid; } }
        public int EntityId { get { return _entityId; } }
        public string EntityName { get { return _entityName; } }
        public string ResPath { get { return _resPath; } }
        public List<AbsComponent> _componentList = new List<AbsComponent>();
        public GameObjectEx gameObjectEx { get { return _gameObjectEx; } }
        public EntityInitEventHandler EntityInitHandler { get; set; }
        public EntityLoadFinishEventHandler EntityLoadFinishHandler
        {
            get { return _entityLoadFinishHandler; }
            set
            {
                if (_gameObjectEx != null && _gameObjectEx.gameObject != null)
                {
                    _entityLoadFinishHandler = value;
                    _entityLoadFinishHandler(this, _gameObjectEx.gameObject);
                }
            }
        }
        public virtual EntityTypeEnum EntityType { get { return EntityTypeEnum.Non; } }

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
            _componentList.Clear();

            _gameObjectEx = PoolMgr.Instance.Get<GameObjectEx>();
            _gameObjectEx.AddLoadFinishHandler(OnAttachGoEx);
            _gameObjectEx.Init(this, _resPath);

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
            for (int i = 0; i < _componentList.Count; i++)
            {
                ComponentMgr.Instance.DestroyComponent(_componentList[i]);
            }
            _componentList.Clear();
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
            _gameObjectEx = go;
            if (_entityLoadFinishHandler != null)
            {
                _entityLoadFinishHandler(this, _gameObjectEx.gameObject);
            }
        }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachGoEx()
        {
            _gameObjectEx.Uninit();
            PoolMgr.Instance.Release<GameObjectEx>(_gameObjectEx);
            _gameObjectEx = null;
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
