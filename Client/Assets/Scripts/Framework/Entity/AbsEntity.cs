/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:32:30
** desc:  ECS实体抽象基类;
*********************************************************************************/

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
                _entityLoadFinishHandler = value;

                if (gameObjectEx != null && gameObjectEx.gameObject != null)
                {
                    _entityLoadFinishHandler(this, gameObjectEx.gameObject);
                }
            }
        }
        public virtual EntityType EntityType { get { return EntityType.Non; } }

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
            ResPath = "Prefab/Models/Avatar/ch_pc_hou_004.prefab";
            gameObjectEx = PoolMgr.Instance.GetCsharpObject<GameObjectEx>();
            gameObjectEx.AddLoadFinishHandler(OnAttachGoEx);
            gameObjectEx.Init(this, ResPath);
            InitializeEx();
        }

        /// <summary>
        /// 重置Entity;
        /// </summary>
        public void UnInitialize()
        {
            DetachGoEx();
            UnInitializeEx();
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
        protected virtual void DetachGoEx()
        {
            gameObjectEx.Uninit();
            PoolMgr.Instance.ReleaseCsharpObject<GameObjectEx>(gameObjectEx);
            gameObjectEx = null;
        }
    }
}
