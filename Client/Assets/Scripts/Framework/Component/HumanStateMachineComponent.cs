/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/07 17:04:14
** desc:  人形状态机组件;
*********************************************************************************/

using Framework.ECSModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class HumanStateMachineComponent : AbsComponent
    {
        private AbsStateMachine _stateMachine;

        protected override void OnAttachGameObject(GameObjectEx goEx)
        {
            base.OnAttachGameObject(goEx);
            _stateMachine = StateMachineMgr.singleton.CreateStateMachine(Entity);
        }

        protected override void OnDetachGameObject()
        {
            base.OnDetachGameObject();
            _stateMachine = null;
            StateMachineMgr.singleton.RemoveStateMachine(Entity);
        }
    }
}