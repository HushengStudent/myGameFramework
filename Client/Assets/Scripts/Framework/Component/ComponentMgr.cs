/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 14:16:25
** desc:  ECS组件管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MEC;

namespace Framework
{
    public class ComponentMgr : MonoSingleton<ComponentMgr>
    {
        #region Field

        private List<AbsComponent> _componentList = new List<AbsComponent>();

        #endregion

        #region Unity api

        public override void Init()
        {
            base.Init();
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
        /// <param name="handler"></param>
        /// <returns></returns>
        public T CreateComponent<T>(AbsEntity entity, ComponentInitEventHandler handler = null) where T : AbsComponent, new()
        {
            T _component = PoolMgr.Instance.GetCsharpObject<T>();
            if (null != entity && entity.AddComponent(_component))
            {
                _component.ComponentInitHandler += handler;
                _component.Init(entity);
                return _component;
            }
            else
            {
                LogHelper.PrintError("[ComponentMgr]CreateComponent " + typeof(T).ToString() + " error!");
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
            if (null == component.Owner)
            {
                return;
            }
            component.Owner.RemoveComponent(component);
            PoolMgr.Instance.ReleaseCsharpObject<T>(component as T);
        }

        /// <summary>
        /// 移除Component;
        /// </summary>
        /// <param name="component"></param>
        public void DestroyComponent(AbsComponent component)
        {
            if (null == component.Owner)
            {
                return;
            }
            component.Owner.RemoveComponent(component);
            component.Uninit();
        }

        #endregion
    }
}
