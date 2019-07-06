/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 13:13:25
** desc:  行为树;
*********************************************************************************/

namespace Framework
{
    public sealed class BehaviorTree
    {
        public bool Enable;
        public OnBehaviorTreeStartHandler OnStart = null;
        public OnBehaviorTreeSuccesstHandler OnSuccess = null;
        public OnBehaviorTreeFailureHandler OnFailure = null;
        public OnBehaviorTreeResetHandler OnReset = null;

        public AbsBehavior Root { get; private set; }
        public AbsEntity Entity { get; private set; }

        public BehaviorTree(AbsBehavior root, AbsEntity entity)
        {
            Root = root;
            Entity = entity;
            Enable = false;
        }

        public void Update(float interval)
        {
            if (Enable && Entity != null && (Root.Reslut == BehaviorState.Reset || Root.Reslut == BehaviorState.Running))
            {
                BehaviorState reslut = Root.Behave(Entity, interval);
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
                        LogHelper.PrintError("[BehaviorTree]error state.");
                        break;
                }
            }
        }
    }
}