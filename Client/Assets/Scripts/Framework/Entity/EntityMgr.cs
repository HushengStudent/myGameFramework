/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:26:38
** desc:  ECS实体管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogUtil;
using System;

namespace Framework
{
    public class EntityMgr : MonoSingleton<EntityMgr>, IManager
    {
        #region Fields
        /// <summary>
        /// EntityDict;
        /// </summary>
        private Dictionary<long, BaseEntity> EntityDict = new Dictionary<long, BaseEntity>();
        /// <summary>
        /// EntityList;
        /// </summary>
        private List<BaseEntity> EntityList = new List<BaseEntity>();

        private Dictionary<ulong, BaseEntity> EntityIndexDict = new Dictionary<ulong, BaseEntity>();

        #endregion

        #region Unity api

        protected override void FixedUpdateEx(float interval)
        {
            base.FixedUpdateEx(interval);
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i].Enable)
                {
                    EntityList[i].FixedUpdateEx(interval);
                }
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i].Enable)
                {
                    EntityList[i].UpdateEx(interval);
                }
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i].Enable)
                {
                    EntityList[i].LateUpdateEx(interval);
                }
            }
        }

        #endregion

        #region Functions

        public void Init()
        {
            EntityDict.Clear();
            EntityList.Clear();
            EntityIndexDict.Clear();
        }
        /// <summary>
        /// 创建Entity;同步/异步完善;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="initHandler"></param>
        /// <returns></returns>
        public T CreateEntity<T>(GameObject go, ulong uid, string name, EntityInitEventHandler initHandler) where T : BaseEntity, new()
        {
            T _Entity = PoolMgr.Instance.Get<T>();//get from pool;
            if (AddEntity(_Entity))
            {
                _Entity.EntityInitHandler = initHandler;
                _Entity.Create(go, uid, name);
                return _Entity;
            }
            else
            {
                LogUtil.LogUtility.PrintError("[EntityMgr]CreateEntity " + typeof(T).ToString() + " error!");
                return null;
            }
        }
        /// <summary>
        /// 移除Entity;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void ReleaseEntity<T>(BaseEntity entity) where T : BaseEntity, new()
        {
            RemoveEntity(entity);
            entity.Reset();
            PoolMgr.Instance.Release<T>(entity as T);//release to pool;
        }
        /// <summary>
        /// 获取Entity;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T GetEntity<T>(ulong uid) where T : BaseEntity, new()
        {
            T target = null;
            BaseEntity temp = null;
            if (EntityIndexDict.TryGetValue(uid, out temp))
            {
                target = temp as T;
            }
            return target;
        }
        /// <summary>
        /// 添加Entity;
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool AddEntity(BaseEntity entity)
        {
            if (EntityDict.ContainsKey(entity.ID))
            {
                return false;
            }
            EntityDict[entity.ID] = entity;
            EntityIndexDict[entity.UID] = entity;
            EntityList.Add(entity);
            return true;
        }
        /// <summary>
        /// 移除Entity;
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool RemoveEntity(BaseEntity entity)
        {
            if (!EntityDict.ContainsKey(entity.ID))
            {
                return false;
            }
            EntityDict.Remove(entity.ID);
            EntityIndexDict.Remove(entity.UID);
            EntityList.Remove(entity);
            return true;
        }

        #endregion
    }
}
