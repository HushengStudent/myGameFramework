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

        private Dictionary<long, AbsComponent> ComponentDict = new Dictionary<long, AbsComponent>();

        private List<AbsComponent> ComponentList = new List<AbsComponent>();

        #endregion

        #region Unity api

        public override void AwakeEx()
        {
            base.AwakeEx();
            for(int i = 0; i < ComponentList.Count; i++)
            {
                ComponentList[i].AwakeEx();
            }
        }

        public override void UpdateEx()
        {
            base.UpdateEx();
            for (int i = 0; i < ComponentList.Count; i++)
            {
                ComponentList[i].UpdateEx();
            }
        }

        public override void LateUpdateEx()
        {
            base.LateUpdateEx();
            for (int i = 0; i < ComponentList.Count; i++)
            {
                ComponentList[i].LateUpdateEx();
            }
        }

        public override void OnDestroyEx()
        {
            base.OnDestroyEx();
            for (int i = 0; i < ComponentList.Count; i++)
            {
                ComponentList[i].OnDestroyEx();
            }
        }

        #endregion

        #region Function

        public T CreateComponent<T>(AbsEntity entity, Action<AbsComponent> initCallBack) where T : AbsComponent, new()
        {
            //TODO:use pool and async;
            T _Component = new T();
            if (AddComponent(_Component))
            {
                _Component.InitCallBack = initCallBack;
                _Component.OnInit(entity);
                return _Component;
            }
            else
            {
                LogUtil.LogUtility.PrintError("[ComponentMgr]CreateComponent " + typeof(T).ToString() + " error!");
                return null;
            }
        }

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
