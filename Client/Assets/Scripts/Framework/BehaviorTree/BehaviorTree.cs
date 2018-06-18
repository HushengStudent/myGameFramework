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
            if (_enable && _entity != null && (_root._returnCode == BehavioResult.Reset || _root._returnCode == BehavioResult.Running))
            {
                _root.Behave(_entity);
            }
        }
    }
}