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

        private EntityLoadFinishEventHandler _entityLoadFinishHandler = null;

        public ulong UID { get; private set; }
        public int EntityId { get; private set; }
        public string EntityName { get; private set; }
        public string ResPath { get; private set; }
        public GameObjectEx gameObjectEx { get; private set; }

        public EntityLoadFinishEventHandler EntityLoadFinishHandler
        {
            get
            {
                return _entityLoadFinishHandler;
            }
            set
            {
                if (gameObjectEx != null && gameObjectEx.gameObject != null)
                {
                    _entityLoadFinishHandler = value;
                    _entityLoadFinishHandler(this, gameObjectEx.gameObject);
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
        public void Init(int entityId, ulong uid, string name)
        {
            UID = uid;
            EntityName = name;
            EntityId = entityId;
            Enable = true;
            ResPath = "Prefab/Models/Avatar/ch_pc_hou_004.prefab";
            gameObjectEx = PoolMgr.Instance.GetCsharpObject<GameObjectEx>();
            gameObjectEx.AddLoadFinishHandler(OnAttachGoEx);
            gameObjectEx.Init(this, ResPath);
            InitEx();
        }

        /// <summary>
        /// 重置Entity;
        /// </summary>
        public void Uninit()
        {
            DeAttachGoEx();
            UninitEx();
            Enable = false;
            EntityLoadFinishHandler = null;
        }

        /// <summary>
        /// Entity附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnAttachGoEx(GameObjectEx go)
        {
            gameObjectEx = go;
            if (_entityLoadFinishHandler != null)
            {
                _entityLoadFinishHandler(this, gameObjectEx.gameObject);
            }
        }

        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void DeAttachGoEx()
        {
            gameObjectEx.Uninit();
            PoolMgr.Instance.ReleaseCsharpObject<GameObjectEx>(gameObjectEx);
            gameObjectEx = null;
        }
    }
}
