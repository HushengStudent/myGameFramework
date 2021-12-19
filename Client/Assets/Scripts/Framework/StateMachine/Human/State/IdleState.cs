/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/05 23:00:07
** desc:  默认状态;
*********************************************************************************/

namespace Framework
{
    public partial class HumanStateMachine
    {
        public class IdleState : AbsState
        {
            public IdleState(AbsStateMachine machine) : base(machine)
            {

            }

            public override string StateName
            {
                get
                {
                    return StateNameEnum.Idle.ToString();
                }
            }

            public override string StateCommand => StateCommandEnum.ToIdle.ToString();

            public override bool Loop { get; set; } = true;

            public override bool Default => true;
        }
    }
}
