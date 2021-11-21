/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:26:38
** desc:  ECS实体管理;
*********************************************************************************/

using Framework.ObjectPoolManager;
using System.Collections.Generic;

namespace Framework
{
    public class EntityMgr : MonoSingleton<EntityMgr>
    {
        #region Fields

        /// EntityDict;
        private Dictionary<ulong, AbsEntity> _entityDict = new Dictionary<ulong, AbsEntity>();

        /// EntityList;
        private List<AbsEntity> _entityList = new List<AbsEntity>();

        /// Call Update EntityList;
        private List<AbsEntity> _updateEntityList = new List<AbsEntity>();
        /// Release at this frame;
        private List<ulong> _releaseEntityUIDList = new List<ulong>();

        #endregion

        #region Unity api

        protected override void FixedUpdateEx(float interval)
        {
            base.FixedUpdateEx(interval);

            _updateEntityList.Clear();
            _updateEntityList.AddRange(_entityList);
            _releaseEntityUIDList.Clear();

            for (var i = 0; i < _updateEntityList.Count; i++)
            {
                var entity = _updateEntityList[i];
                if (entity.Enable && !_releaseEntityUIDList.Contains(entity.UID))
                {
                    entity.FixedUpdate(interval);
                }
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (var i = 0; i < _updateEntityList.Count; i++)
            {
                var entity = _updateEntityList[i];
                if (entity.Enable && !_releaseEntityUIDList.Contains(entity.UID))
                {
                    entity.Update(interval);
                }
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            for (var i = 0; i < _updateEntityList.Count; i++)
            {
                var entity = _updateEntityList[i];
                if (entity.Enable && !_releaseEntityUIDList.Contains(entity.UID))
                {
                    entity.LateUpdate(interval);
                }
            }
        }

        #endregion

        #region Functions

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _entityList.Clear();
            _entityDict.Clear();
        }

        /// <summary>
        /// 创建Entity;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T CreateEntity<T>(int entityId, ulong uid, string name) where T : AbsEntity, new()
        {
            var entity = PoolMgr.singleton.GetCsharpObject<T>();
            if (AddEntity(uid, entity))
            {
                entity.Initialize(entityId, uid, name);
                return entity;
            }
            else
            {
                LogHelper.PrintError($"[EntityMgr]CreateEntity:{typeof(T).ToString()} error,entityId:{entityId},uid:{uid},name:{name}.");
                PoolMgr.singleton.ReleaseCsharpObject(entity);
                return null;
            }
        }

        /// <summary>
        /// 移除Entity;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void ReleaseEntity<T>(AbsEntity entity) where T : AbsEntity, new()
        {
            RemoveEntity(entity);
            entity.UnInitialize();
            _releaseEntityUIDList.Add(entity.UID);
            PoolMgr.singleton.ReleaseCsharpObject(entity as T);
        }

        /// <summary>
        /// 获取Entity;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T GetEntity<T>(ulong uid) where T : AbsEntity, new()
        {
            if (_entityDict.TryGetValue(uid, out var temp))
            {
                return temp as T;
            }
            else
            {
                return null;
            }
        }

        /// 添加Entity;
        private bool AddEntity(ulong UID, AbsEntity entity)
        {
            if (_entityDict.ContainsKey(UID))
            {
                return false;
            }
            _entityList.Add(entity);
            _entityDict[UID] = entity;
            return true;
        }

        /// 删除Entity;
        private bool RemoveEntity(AbsEntity entity)
        {
            if (!_entityDict.ContainsKey(entity.UID))
            {
                return false;
            }
            _entityList.Remove(entity);
            _entityDict.Remove(entity.UID);
            return true;
        }

        #endregion
    }
}
