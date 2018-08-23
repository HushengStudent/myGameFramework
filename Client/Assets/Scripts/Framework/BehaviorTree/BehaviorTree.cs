/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 13:13:25
** desc:  行为树;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public sealed class BehaviorTree
    {
        private AbsBehavior _root;
        private AbsEntity _entity;

        public bool Enable { get; set; }
        public AbsBehavior Root { get { return _root; } }
        public AbsEntity Entity { get { return _entity; } }

        public OnBehaviorTreeStartHandler OnStart { get; set; }
        public OnBehaviorTreeSuccesstHandler OnSuccess { get; set; }
        public OnBehaviorTreeFailureHandler OnFailure { get; set; }
        public OnBehaviorTreeResetHandler OnReset { get; set; }

        public BehaviorTree(AbsBehavior root, AbsEntity entity)
        {
            _root = root;
            _entity = entity;
            Enable = false;
        }

        public void Update(float interval)
        {
            if (Enable && _entity != null && (_root.Reslut == BehaviorState.Reset || _root.Reslut == BehaviorState.Running))
            {
                BehaviorState reslut = _root.Behave(_entity, interval);
                switch (reslut)
                {
                    case BehaviorState.Reset:
                        break;
                    case BehaviorState.Failure:
                        break;
                    case BehaviorState.Running:
                        break;
                    case BehaviorState.Success:
                        break;
                    case BehaviorState.Finish:
                        break;
                    default:
                        Enable = false;
                        LogUtil.LogUtility.PrintError("[BehaviorTree]error state.");
                        break;
                }
            }
        }
    }
}