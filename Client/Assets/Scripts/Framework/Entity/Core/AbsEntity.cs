/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:32:30
** desc:  ECS实体抽象基类;
*********************************************************************************/

using Framework.ObjectPoolModule;
using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Framework.ECSModule
{
    public enum EntityType : byte /// (curEntityType & EntityType.Role) > 0
    {
        Non = 1 << 0,

        Role = 1 << 1,
        Player = 1 << 2,

        Monster = 1 << 3,
        Boss = 1 << 4,
    }

    public abstract class AbsEntity : ObjectEx
    {
        protected AbsEntity() : base() { }

        public ulong UID { get; private set; }
        public int EntityId { get; private set; }
        public string EntityName { get; private set; }
        public string AssetPath { get; private set; }
        public GameObjectEx GameObjectEx { get; private set; }
        public Animator Animator { get; private set; }

        public abstract EntityType EntityType { get; }

        public event Action<AbsEntity> LoadFinishEventHandler;

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
                LoadFinishEventHandler?.Invoke(this);
                LoadFinishEventHandler = null;

            });
            GameObjectEx.Initialize(this);
        }

        /// <summary>
        /// 重置Entity;
        /// </summary>
        public void UnInitialize()
        {
            InternalDetachGameObject();
            InternalUnInitialize();
            Enable = false;
        }

        private void InternalAttachGameObject(GameObjectEx go)
        {
            GameObjectEx = go;
            Animator = GameObjectEx.gameObject.GetComponent<Animator>();
            UnityObject.DontDestroyOnLoad(GameObjectEx.gameObject);
            OnAttachGameObject(go);
        }

        private void InternalDetachGameObject()
        {
            OnDetachGameObject();
            GameObjectEx.UnInitialize();
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
