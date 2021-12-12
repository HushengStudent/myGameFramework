/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/24 23:24:46
** desc:  RoleEntity;
*********************************************************************************/

using Framework.BuffModule;
using Framework.ECSModule;

namespace Framework
{
    public class RoleEntity : AbsEntity
    {
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Role;
            }
        }

        public BuffComponent BuffComp { get; private set; }
        public HumanStateMachineComponent HumanStateMachineComp { get; private set; }
        public BehaviorTreeComponent BehaviorTreeComp { get; private set; }

        protected override void RegisterComponent()
        {
            base.RegisterComponent();
            BuffComp = AddComponent<BuffComponent>();
            HumanStateMachineComp = AddComponent<HumanStateMachineComponent>();
            BehaviorTreeComp = AddComponent<BehaviorTreeComponent>();
        }

        protected override void UnRegisterComponent()
        {
            base.UnRegisterComponent();
            ReleaseComponent<BuffComponent>();
            ReleaseComponent<HumanStateMachineComponent>();
            ReleaseComponent<BehaviorTreeComponent>();
        }
    }
}