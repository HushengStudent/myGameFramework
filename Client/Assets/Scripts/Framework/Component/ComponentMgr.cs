/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 14:16:25
** desc:  #####
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogUtil;

namespace Framework
{
	public class ComponentMgr : MonoSingleton<ComponentMgr>
	{
        public override void Init()
        {
            base.Init();
            LogUtility.Print("[ComponentMgr]Init!", LogColor.Green);
        }

        #region Field

        private Dictionary<ulong, AbsComponent> ComponentDict = new Dictionary<ulong, AbsComponent>();

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

        public void StartComponentMgr()
        {

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
