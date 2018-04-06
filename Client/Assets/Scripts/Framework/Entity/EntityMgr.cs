/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:26:38
** desc:  ECS实体管理
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
        #region Field
        /// <summary>
        /// EntityDict;
        /// </summary>
        private Dictionary<long, AbsEntity> EntityDict = new Dictionary<long, AbsEntity>();
        /// <summary>
        /// EntityList;
        /// </summary>
        private List<AbsEntity> EntityList = new List<AbsEntity>();

        #endregion

        #region Unity api

        public override void UpdateEx()
        {
            base.UpdateEx();
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i].Enable)
                {
                    EntityList[i].UpdateEx();
                }
            }
        }

        public override void LateUpdateEx()
        {
            base.LateUpdateEx();
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i].Enable)
                {
                    EntityList[i].LateUpdateEx();
                }
            }
        }

        #endregion

        #region Function
        /// <summary>
        /// 创建Entity;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="initCallBack"></param>
        /// <returns></returns>
        public T CreateEntity<T>(GameObject go, Action<AbsEntity> initCallBack) where T : AbsEntity, new()
        {
            T _Entity = PoolMgr.Instance.Get<T>();
            if (AddEntity(_Entity))
            {
                _Entity.InitCallBack = initCallBack;
                _Entity.OnInitEntity(go);
                return _Entity;
            }
            else
            {
                LogUtil.LogUtility.PrintError("[EntityMgr]CreateEntity " + typeof(T).ToString() + " error!");
                return null;
            }
        }
        /// <summary>
        /// 添加Entity;
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool AddEntity(AbsEntity entity)
        {
            if (EntityDict.ContainsKey(entity.ID))
            {
                return false;
            }
            EntityDict[entity.ID] = entity;
            EntityList.Add(entity);
            return true;
        }
        /// <summary>
        /// 移除Entity;
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool RemoveComponent(AbsEntity entity)
        {
            if (!EntityDict.ContainsKey(entity.ID))
            {
                return false;
            }
            EntityDict.Remove(entity.ID);
            EntityList.Remove(entity);
            return true;
        }

        #endregion
    }
}
