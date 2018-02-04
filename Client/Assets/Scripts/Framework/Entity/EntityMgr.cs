/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:26:38
** desc:  实体管理
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogUtil;

namespace Framework
{
    public class EntityMgr : MonoSingleton<EntityMgr>
    {
        public override void Init()
        {
            base.Init();
            LogUtility.Print("[EntityMgr]Init!", LogColor.Green);
        }

        #region Field

        private Dictionary<long, AbsEntity> EntityDict = new Dictionary<long, AbsEntity>();

        private List<AbsEntity> EntityList = new List<AbsEntity>();

        #endregion

        #region Unity api

        public override void AwakeEx()
        {
            base.AwakeEx();
            for (int i = 0; i < EntityList.Count; i++)
            {
                EntityList[i].AwakeEx();
            }
        }

        public override void UpdateEx()
        {
            base.UpdateEx();
            for (int i = 0; i < EntityList.Count; i++)
            {
                EntityList[i].UpdateEx();
            }
        }

        public override void LateUpdateEx()
        {
            base.LateUpdateEx();
            for (int i = 0; i < EntityList.Count; i++)
            {
                EntityList[i].LateUpdateEx();
            }
        }

        public override void OnDestroyEx()
        {
            base.OnDestroyEx();
            for (int i = 0; i < EntityList.Count; i++)
            {
                EntityList[i].OnDestroyEx();
            }
        }

        #endregion

        #region Function

        public void StartEntityMgr()
        {

        }

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
