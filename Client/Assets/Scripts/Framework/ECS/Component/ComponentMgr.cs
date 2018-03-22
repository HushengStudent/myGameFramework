/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 14:16:25
** desc:  组件管理
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogUtil;
using System;
using MEC;

namespace Framework
{
    public class ComponentMgr : MonoSingleton<ComponentMgr>
    {
        public override void Init()
        {
            base.Init();
            //...
        }

        #region Field
        /// <summary>
        /// ComponentDict;
        /// </summary>
        private Dictionary<long, AbsComponent> ComponentDict = new Dictionary<long, AbsComponent>();
        /// <summary>
        /// ComponentList;
        /// </summary>
        private List<AbsComponent> ComponentList = new List<AbsComponent>();

        #endregion

        #region Unity api

        public override void UpdateEx()
        {
            base.UpdateEx();
            for (int i = 0; i < ComponentList.Count; i++)
            {
                if (ComponentList[i].Enable)
                {
                    ComponentList[i].UpdateEx();
                }
            }
        }

        public override void LateUpdateEx()
        {
            base.LateUpdateEx();
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
        public T CreateComponent<T>(AbsEntity entity, GameObject go,
            Action<AbsComponent> initCallBack) where T : AbsComponent, new()
        {
            T _Component = PoolMgr.Instance.Get<T>();
            if (AddComponent(_Component))
            {
                _Component.InitCallBack = initCallBack;
                _Component.OnInitComponent(entity, go);
                return _Component;
            }
            else
            {
                LogUtil.LogUtility.PrintError("[ComponentMgr]CreateComponent " + typeof(T).ToString() + " error!");
                return null;
            }
        }
        /// <summary>
        /// 添加Component;
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool AddComponent(AbsComponent component)
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
        private bool RemoveComponent(AbsComponent component)
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
