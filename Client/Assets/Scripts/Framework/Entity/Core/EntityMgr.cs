/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:26:38
** desc:  ECS实体管理;
*********************************************************************************/

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

        #endregion

        #region Unity api

        protected override void FixedUpdateEx(float interval)
        {
            base.FixedUpdateEx(interval);
            for (int i = 0; i < _entityList.Count; i++)
            {
                if (_entityList[i].Enable)
                {
                    _entityList[i].FixedUpdateEx(interval);
                }
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < _entityList.Count; i++)
            {
                if (_entityList[i].Enable)
                {
                    _entityList[i].UpdateEx(interval);
                }
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            for (int i = 0; i < _entityList.Count; i++)
            {
                if (_entityList[i].Enable)
                {
                    _entityList[i].LateUpdateEx(interval);
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
            T entity = PoolMgr.singleton.GetCsharpObject<T>();
            if (AddEntity(uid, entity))
            {
                entity.Initialize(entityId, uid, name);
                return entity;
            }
            else
            {
                LogHelper.PrintError($"[EntityMgr]CreateEntity {typeof(T).ToString()} error,entityId:{entityId},uid:{uid},name:{name}!");
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
            PoolMgr.singleton.ReleaseCsharpObject<T>(entity as T);
        }

        /// <summary>
        /// 获取Entity;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T GetEntity<T>(ulong uid) where T : AbsEntity, new()
        {
            T target = null;
            AbsEntity temp = null;
            if (_entityDict.TryGetValue(uid, out temp))
            {
                target = temp as T;
            }
            return target;
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
