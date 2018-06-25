/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 13:12:11
** desc:  行为抽象基类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsBehavior
    {
        private bool _awake = false;
        private BaseEntity _entity = null;
        private BehaviorState _reslut = BehaviorState.Reset;

        public BaseEntity Entity { get { return _entity; } }
        public BehaviorState Reslut { get { return _reslut; } set { _reslut = value; } }

        public BehaviorState Behave(BaseEntity entity)
        {
            if (!_awake)
            {
                _awake = true;
                _entity = entity;
                Reslut = BehaviorState.Running;
                AwakeEx();
            }
            UpdateEx();
            return Reslut;
        }

        public bool ResetBehavior()
        {
            if (Reslut == BehaviorState.Running)
                return false;
            Reset();
            _awake = false;
            _entity = null;
            Reslut = BehaviorState.Reset;
            return true;
        }

        public AbsBehavior(object[] args) { }

        protected abstract void AwakeEx();

        protected abstract void UpdateEx();

        protected abstract void Reset();
    }
}
