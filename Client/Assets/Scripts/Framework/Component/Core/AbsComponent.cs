/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:22:57
** desc:  ECS组件抽象基类;
*********************************************************************************/

namespace Framework
{
    /// <summary>
    /// 组件抽象基类;
    /// </summary>
    public abstract class AbsComponent
    {
        protected AbsComponent() { }

        public ObjectEx Owner { get; private set; }
        public AbsEntity Entity { get; private set; }
        public bool Enable { get; private set; }

        public virtual void FixedUpdateEx(float interval) { }
        public virtual void UpdateEx(float interval) { }
        public virtual void LateUpdateEx(float interval) { }

        public abstract string UID { get; }

        /// <summary>
        /// 初始化Component;
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="go">gameObject</param>
        public void Initialize(ObjectEx owner)
        {
            Enable = true;
            InternalAttachObject(owner);
            if (Entity.GameObjectEx.IsLoadFinish)
            {
                OnAttachGameObject(Entity.GameObjectEx);
            }
            else
            {
                Entity.GameObjectEx.AddLoadFinishHandler(OnAttachGameObject);
            }
            EventSubscribe();
            InitializeEx();
        }

        /// <summary>
        /// 重置Component;
        /// </summary>
        public void UnInitialize()
        {
            InternalDetachObject();
            OnDetachGameObject();
            EventUnsubscribe();
            UnInitializeEx();
            Enable = false;
        }

        private void InternalAttachObject(ObjectEx owner)
        {
            Owner = owner;
            Entity = owner as AbsEntity;
            OnAttachObject(owner);
        }

        private void InternalDetachObject()
        {
            OnDetachObjectEx();
            Owner = null;
            Entity = null;
        }

        private void InternalDetachGameObject()
        {
            if (!Entity.GameObjectEx.IsLoadFinish)
            {
                Entity.GameObjectEx.RemoveLoadFinishHandler(OnAttachGameObject);
            }
            OnDetachGameObject();
        }

        /// <summary>
        /// 初始化;
        /// </summary>
        protected virtual void InitializeEx() { }
        /// <summary>
        /// 重置;
        /// </summary>
        protected virtual void UnInitializeEx() { }
        /// <summary>
        /// Component附加Entity;
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnAttachObject(ObjectEx owner) { }
        /// <summary>
        /// 重置Entity的附加;
        /// </summary>
        protected virtual void OnDetachObjectEx() { }
        /// <summary>
        /// Component附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        protected virtual void OnAttachGameObject(GameObjectEx goEx) { }
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        protected virtual void OnDetachGameObject() { }
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

        protected void AddEvent(EventType type, EventHandler handler)
        {
            EventMgr.Instance.AddEvent(Owner, type, handler);
        }

        protected void RemoveEvent(EventType type)
        {
            EventMgr.Instance.RemoveEvent(Owner, type);
        }

        protected void FireEvent(EventType type, IEventArgs eventArgs)
        {
            EventMgr.Instance.FireEvent(Owner, type, eventArgs);
        }
    }
}
