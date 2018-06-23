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

        public bool Enable { get { return _enable; } set { _enable = value; } }
        public AbsBehavior Root { get { return _root; } set { _root = value; } }
        public BaseEntity Entity { get { return _entity; } set { _entity = value; } }

        public void SerilizeRoot()
        {

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
                    default:
                        _enable = false;
                        LogUtil.LogUtility.PrintError("[BehaviorTree]error state.");
                        break;
                }
            }
        }
    }
}