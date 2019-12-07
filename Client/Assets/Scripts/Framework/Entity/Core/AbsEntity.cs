/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:32:30
** desc:  ECS实体抽象基类;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public enum EntityType : byte
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
        public string AssetPath { get; private set; } = "Prefab/Models/Common/Skeleton.prefab";
        public GameObjectEx GameObjectEx { get; private set; }
        public Animator Animator { get; private set; }

        public EntityLoadFinishEventHandler EntityLoadFinishHandler
        {
            get
            {
                return _entityLoadFinishHandler;
            }
            set
            {
                _entityLoadFinishHandler = value;

                if (GameObjectEx != null && GameObjectEx.gameObject != null)
                {
                    _entityLoadFinishHandler(this, GameObjectEx.gameObject);
                }
            }
        }
        public abstract EntityType EntityType { get; }

        public virtual void FixedUpdateEx(float interval) { }
        public virtual void UpdateEx(float interval) { }
        public virtual void LateUpdateEx(float interval) { }

        /// <summary>
        /// 初始化Entity;
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        public void Initialize(int entityId, ulong uid, string name)
        {
            UID = uid;
            EntityName = name;
            EntityId = entityId;
            Enable = true;
            GameObjectEx = PoolMgr.singleton.GetCsharpObject<GameObjectEx>();

            InternalInitialize();
            GameObjectEx.AddLoadFinishHandler((goex) =>
            {
                InternalAttachGameObject(goex);
                _entityLoadFinishHandler?.Invoke(this, GameObjectEx.gameObject);

            });
            GameObjectEx.Init(this, AssetPath);
        }

        /// <summary>
        /// 重置Entity;
        /// </summary>
        public void UnInitialize()
        {
            InternalDetachGameObject();
            InternalUnInitialize();
            Enable = false;
            EntityLoadFinishHandler = null;
        }

        private void InternalAttachGameObject(GameObjectEx go)
        {
            GameObjectEx = go;
            Animator = GameObjectEx.gameObject.GetComponent<Animator>();
            OnAttachGameObject(go);
        }

        private void InternalDetachGameObject()
        {
            OnDetachGameObject();
            GameObjectEx.Uninit();
            PoolMgr.singleton.ReleaseCsharpObject(GameObjectEx);
            GameObjectEx = null;
        }

        /// <summary>
        /// Entity附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnAttachGameObject(GameObjectEx go) { }
        /// <summary>
        /// Entity移除GameObject;
        /// </summary>
        protected virtual void OnDetachGameObject() { }
    }
}
