/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 14:16:25
** desc:  ECS组件管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogUtil;
using System;
using MEC;

namespace Framework
{
    public class ComponentMgr : MonoSingleton<ComponentMgr>, IMgr
    {
        #region Field

        /// <summary>
        /// ComponentDict;
        /// </summary>
        private Dictionary<long, BaseComponent> ComponentDict = new Dictionary<long, BaseComponent>();
        /// <summary>
        /// ComponentList;
        /// </summary>
        private List<BaseComponent> ComponentList = new List<BaseComponent>();

        #endregion

        #region Unity api

        public void InitMgr()
        {
            ComponentDict.Clear();
            ComponentList.Clear();
        }

        public override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < ComponentList.Count; i++)
            {
                if (ComponentList[i].Enable)
                {
                    ComponentList[i].UpdateEx();
                }
            }
        }

        public override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            for (int i = 0; i < ComponentList.Count; i++)
            {
                if (ComponentList[i].Enable)
                {
                    ComponentList[i].LateUpdateEx();
                }
            }
        }

        #endregion

        #region Function
        /// <summary>
        /// 创建Component;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="go"></param>
        /// <param name="initCallBack"></param>
        /// <returns></returns>
        public T CreateComponent<T>(BaseEntity entity, GameObject go,
            Action<BaseComponent> initCallBack) where T : BaseComponent, new()
        {
            T _Component = PoolMgr.Instance.Get<T>();//get from pool;
            if (AddComponent(_Component))
            {
                _Component.InitCallBack = initCallBack;
                _Component.Create(entity, go);
                return _Component;
            }
            else
            {
                LogUtil.LogUtility.PrintError("[ComponentMgr]CreateComponent " + typeof(T).ToString() + " error!");
                return null;
            }
        }
        /// <summary>
        /// 移除Component;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        public void ReleaseComponent<T>(BaseComponent component) where T : BaseComponent, new()
        {
            RemoveComponent(component);
            component.Reset();
            PoolMgr.Instance.Release<T>(component as T);//release to pool;
        }
        /// <summary>
        /// 添加Component;
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool AddComponent(BaseComponent component)
        {
            if (ComponentDict.ContainsKey(component.ID))
            {
                return false;
            }
            ComponentDict[component.ID] = component;
            ComponentList.Add(component);
            return true;
        }
        /// <summary>
        /// 移除Component;
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool RemoveComponent(BaseComponent component)
        {
            if (!ComponentDict.ContainsKey(component.ID))
            {
                return false;
            }
            ComponentDict.Remove(component.ID);
            ComponentList.Remove(component);
            return true;
        }

        #endregion
    }
}
