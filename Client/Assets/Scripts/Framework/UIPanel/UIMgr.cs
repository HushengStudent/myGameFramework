/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2017/12/25 00:30:02
** desc:  UI管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class UIMgr : MonoSingleton<UIMgr>
    {
        private BasePanel _curPanel = null;
        private PanelType _curPanelType = PanelType.Non;

        public BasePanel CurPanel { get { return _curPanel; } }
        public PanelType CurPanelType { get { return _curPanelType; } }

        public override void Init()
        {
            base.Init();
            _curPanel = null;
            _curPanelType = PanelType.Non;
            LogUtil.LogUtility.Print("[UIMgr]UIMgr init!");
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            if (CurPanel != null)
            {
                CurPanel.Update();
            }
        }

        /// <summary>
        /// 面板跳转;
        /// </summary>
        /// <param name="type"></param>
        public void TransToPanel(PanelType type)
        {
            if (CurPanel != null)
            {
                CurPanel.OnExit();
            }
            else
            {
                _curPanel = CreatePanel(type);
                if (CurPanel == null)
                {
                    LogUtil.LogUtility.PrintError(string.Format("[UIMgr]trans to panel {0} error.", type.ToString()));
                    return;
                }
            }
            CurPanel.OnEnter();
        }

        /// <summary>
        /// 创建面板;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private BasePanel CreatePanel(PanelType type)
        {
            BasePanel panel = null;
            switch (type)
            {
                case PanelType.Login:
                    //...
                    break;
                case PanelType.Non:
                default:
                    break;
            }
            return panel;
        }
    }
}
