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
    public class BehaviorTree
    {
        private bool _enable = false;
        private AbsBehavior _root;
        private BaseEntity _entity;

        private OnBehaviorTreeStartHandler _onStart;
        private OnBehaviorTreeSuccesstHandler _onSuccess;
        private OnBehaviorTreeFailureHandler _onFailure;
        private OnBehaviorTreeResetHandler _onReset;

        public bool Enable { get { return _enable; } set { _enable = value; } }
        public AbsBehavior Root { get { return _root; } set { _root = value; } }
        public BaseEntity Entity { get { return _entity; } set { _entity = value; } }

        public OnBehaviorTreeStartHandler OnStart { get { return _onStart; } set { _onStart = value; } }
        public OnBehaviorTreeSuccesstHandler OnSuccess { get { return _onSuccess; } set { _onSuccess = value; } }
        public OnBehaviorTreeFailureHandler OnFailure { get { return _onFailure; } set { _onFailure = value; } }
        public OnBehaviorTreeResetHandler OnReset { get { return _onReset; } set { _onReset = value; } }

        public BehaviorTree(AbsBehavior root,BaseEntity entity)
        {
            _root = root;
            _entity = entity;
            _enable = false;
        }

        public void Update()
        {
            if (_enable && _entity != null && (_root.Reslut == BehaviorState.Reset || _root.Reslut == BehaviorState.Running))
            {
                BehaviorState reslut = _root.Behave(_entity);
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
                        _enable = false;
                        LogUtil.LogUtility.PrintError("[BehaviorTree]error state.");
                        break;
                }
            }
        }
    }
}