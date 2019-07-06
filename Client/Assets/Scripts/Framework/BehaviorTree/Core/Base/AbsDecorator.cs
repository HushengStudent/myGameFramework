/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/07/12 00:26:53
** desc:  实体节点父类;
*********************************************************************************/

using System.Collections;

namespace Framework
{
    public abstract class AbsDecorator : AbsBehavior
    {
        public AbsBehavior NextNode { get; protected set; }

        public AbsDecorator(Hashtable table) : base(table)
        {
            NextNode = null;
        }

        public void Serialize(AbsBehavior node)
        {
            NextNode = node;
        }

        protected sealed override void Update(float interval)
        {
            if (Reslut == BehaviorState.Running)
            {
                UpdateEx(interval);
            }
            else if (Reslut == BehaviorState.Finish)
            {
                if (NextNode == null)
                {
                    Reslut = BehaviorState.Success;
                    return;
                }
                if (NextNode != null && NextNode.Behave(Entity, interval) == BehaviorState.Success)
                {
                    Reslut = BehaviorState.Success;
                }
            }
        }
    }
}