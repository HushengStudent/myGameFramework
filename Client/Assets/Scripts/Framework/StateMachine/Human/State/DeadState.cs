/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/05 23:00:29
** desc:  死亡状态;
*********************************************************************************/

namespace Framework
{
    public partial class HumanStateMachine
    {
        public class DeadState : AbsState
        {
            public DeadState(AbsStateMachine machine) : base(machine)
            {

            }

            public override string StateName
            {
                get
                {
                    return StateNameEnum.Dead.ToString();
                }
            }

            public override string StateCommand => StateCommandEnum.ToDead.ToString();

            public override bool Loop { get; set; } = false;

            public override bool Default => false;
        }
    }
}