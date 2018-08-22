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
        private int _id;
        private AbsEntity _entity = null;
        private BehaviorState _reslut = BehaviorState.Reset;

        public int Id { get { return _id; } set { _id = value; } }
        public AbsEntity Entity { get { return _entity; } }
        public BehaviorState Reslut { get { return _reslut; } set { _reslut = value; } }

        public virtual bool IsComposite { get { return false; } }

        public BehaviorState Behave(AbsEntity entity, float interval)
        {
            if (!_awake)
            {
                _awake = true;
                _entity = entity;
                Reslut = BehaviorState.Running;
                AwakeEx();
            }
            Update(interval);
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

        public AbsBehavior(Hashtable table) { }

        protected abstract void AwakeEx();

        protected abstract void Update(float interval);

        protected abstract void UpdateEx(float interval);

        protected abstract void Reset();

    }
}
