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
    public class EntityMgr : MonoSingleton<EntityMgr>
    {
        #region Fields
        /// <summary>
        /// EntityDict;
        /// </summary>
        private Dictionary<long, AbsEntity> _entityDict = new Dictionary<long, AbsEntity>();
        /// <summary>
        /// EntityList;
        /// </summary>
        private List<AbsEntity> _entityList = new List<AbsEntity>();
        private List<AbsEntity> _list = new List<AbsEntity>();

        private Dictionary<ulong, AbsEntity> _entityIdDict = new Dictionary<ulong, AbsEntity>();

        #endregion

        #region Unity api

        protected override void FixedUpdateEx(float interval)
        {
            base.FixedUpdateEx(interval);
            _list.AddRange(_entityList);
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Enable)
                {
                    _list[i].FixedUpdateEx(interval);
                }
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Enable)
                {
                    _list[i].UpdateEx(interval);
                }
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Enable)
                {
                    _list[i].LateUpdateEx(interval);
                }
            }
        }

        #endregion

        #region Functions

        public override void Init()
        {
            base.Init();
            _entityDict.Clear();
            _entityList.Clear();
            _entityIdDict.Clear();
        }

        /// <summary>
        /// 创建Entity;同步/异步完善;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="initHandler"></param>
        /// <returns></returns>
        public T CreateEntity<T>(int entityId, ulong uid, string name, EntityInitEventHandler initHandler) where T : AbsEntity, new()
        {
            T _Entity = PoolMgr.Instance.Get<T>();
            if (AddEntity(_Entity))
            {
                _Entity.EntityInitHandler = initHandler;
                _Entity.Init(entityId, uid, name);
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
        public void ReleaseEntity<T>(AbsEntity entity) where T : AbsEntity, new()
        {
            RemoveEntity(entity);
            entity.Uninit();
            PoolMgr.Instance.Release<T>(entity as T);//release to pool;
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
            if (_entityIdDict.TryGetValue(uid, out temp))
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
        private bool AddEntity(AbsEntity entity)
        {
            if (_entityDict.ContainsKey(entity.ID))
            {
                return false;
            }
            _entityDict[entity.ID] = entity;
            _entityIdDict[entity.UID] = entity;
            _entityList.Add(entity);
            return true;
        }
        /// <summary>
        /// 移除Entity;
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool RemoveEntity(AbsEntity entity)
        {
            if (!_entityDict.ContainsKey(entity.ID))
            {
                return false;
            }
            _entityDict.Remove(entity.ID);
            _entityIdDict.Remove(entity.UID);
            _entityList.Remove(entity);
            return true;
        }

        #endregion
    }
}
