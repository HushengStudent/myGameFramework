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
    public class ComponentMgr : MonoSingleton<ComponentMgr>, IManager
    {
        #region Field

        /// <summary>
        /// ComponentDict;
        /// </summary>
        private Dictionary<long, AbsComponent> _componentDict = new Dictionary<long, AbsComponent>();
        /// <summary>
        /// ComponentList;
        /// </summary>
        private List<AbsComponent> _componentList = new List<AbsComponent>();

        #endregion

        #region Unity api

        public void Init()
        {
            _componentDict.Clear();
            _componentList.Clear();
        }

        protected override void FixedUpdateEx(float interval)
        {
            base.FixedUpdateEx(interval);
            for (int i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i].Enable)
                {
                    _componentList[i].FixedUpdateEx(interval);
                }
            }
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i].Enable)
                {
                    _componentList[i].UpdateEx(interval);
                }
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            for (int i = 0; i < _componentList.Count; i++)
            {
                if (_componentList[i].Enable)
                {
                    _componentList[i].LateUpdateEx(interval);
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
        /// <param name="handler"></param>
        /// <returns></returns>
        public T CreateComponent<T>(AbsEntity entity, ComponentInitEventHandler handler) where T : AbsComponent, new()
        {
            T _Component = PoolMgr.Instance.Get<T>();
            if (AddComponent(_Component))
            {
                _Component.ComponentInitHandler = handler;
                _Component.Create(entity);
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
        public void ReleaseComponent<T>(AbsComponent component) where T : AbsComponent, new()
        {
            RemoveComponent(component);
            component.Reset();
            PoolMgr.Instance.Release<T>(component as T);//Release To Pool;
        }
        /// <summary>
        /// 添加Component;
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool AddComponent(AbsComponent component)
        {
            if (_componentDict.ContainsKey(component.ID))
            {
                return false;
            }
            _componentDict[component.ID] = component;
            _componentList.Add(component);
            return true;
        }
        /// <summary>
        /// 移除Component;
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool RemoveComponent(AbsComponent component)
        {
            if (!_componentDict.ContainsKey(component.ID))
            {
                return false;
            }
            _componentDict.Remove(component.ID);
            _componentList.Remove(component);
            return true;
        }

        #endregion
    }
}
