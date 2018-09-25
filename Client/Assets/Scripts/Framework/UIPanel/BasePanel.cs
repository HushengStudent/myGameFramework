/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/16 23:30:38
** desc:  ui面板基类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum PanelType : byte
    {
        Non = 1,
        Login,
    }

    public abstract class BasePanel
    {
        private PanelType _type = PanelType.Non;

        public List<BaseUI> BaseUIList = new List<BaseUI>();
        public PanelType Type { get { return _type; } }

        public virtual void Update()
        {
            for (int i = 0; i < BaseUIList.Count; i++)
            {
                if (BaseUIList[i].Enable)
                {
                    BaseUIList[i].Update();
                }
            }
        }

        /// <summary>
        /// 进入面板;
        /// </summary>
        public abstract void OnEnter();
        /// <summary>
        /// 离开面板;
        /// </summary>
        public abstract void OnExit();
    }
}