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
        public BehavioResult _returnCode;

        public abstract BehavioResult Behave(BaseEntity entity);

        public virtual void UpdateEx() { }

        protected abstract void Reset();

        public bool ResetBehavior()
        {
            if (_returnCode == BehavioResult.Running)
                return false;
            Reset();
            _returnCode = BehavioResult.Reset;
            return true;
        }
    }
}
