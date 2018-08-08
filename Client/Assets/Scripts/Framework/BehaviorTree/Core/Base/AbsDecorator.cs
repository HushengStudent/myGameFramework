/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/07/12 00:26:53
** desc:  实体节点父类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsDecorator : AbsBehavior
    {
        protected AbsBehavior _nextNode = null;

        public AbsBehavior NextNode { get { return _nextNode; } }

        public AbsDecorator(Hashtable table) : base(table)
        {
            _nextNode = null;
        }

        public void Serialize(AbsBehavior node)
        {
            _nextNode = node;
        }

        protected sealed override void Update(float interval)
        {
            if (Reslut == BehaviorState.Running)
            {
                UpdateEx(interval);
            }
            else if (Reslut == BehaviorState.Finish)
            {
                if (_nextNode == null)
                {
                    Reslut = BehaviorState.Success;
                    return;
                }
                if (_nextNode != null && _nextNode.Behave(Entity, interval) == BehaviorState.Success)
                {
                    Reslut = BehaviorState.Success;
                }
            }
        }
    }
}