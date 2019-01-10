/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:26:38
** desc:  ECS实体管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framework
{
    public class EntityMgr : MonoSingleton<EntityMgr>
    {
        #region Fields

        /// <summary>
        /// EntityDict;
        /// </summary>
        private Dictionary<ulong, AbsEntity> _entityDict = new Dictionary<ulong, AbsEntity>();

        /// <summary>
        /// EntityList;
        /// </summary>
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

        public override void Init()
        {
            base.Init();
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
            T entity = PoolMgr.Instance.GetCsharpObject<T>();
            if (AddEntity(uid, entity))
            {
                entity.Initialize(entityId, uid, name);
                return entity;
            }
            else
            {
                LogHelper.PrintError(string.Format("[EntityMgr]CreateEntity " + typeof(T).ToString() +
                    " error,entityId:{0},uid:{1},name:{2}!", entityId, uid, name));
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
            PoolMgr.Instance.ReleaseCsharpObject<T>(entity as T);
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

        /// <summary>
        /// 添加Entity;
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 删除Entity;
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
