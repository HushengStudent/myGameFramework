/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/05 23:00:18
** desc:  移动状态;
*********************************************************************************/

namespace Framework
{
    public partial class HumanStateMachine
    {
        public class MoveState : AbsState
        {
            public MoveState(AbsStateMachine machine) : base(machine)
            {

            }

            public override string StateName
            {
                get
                {
                    return StateNameEnum.Move.ToString();
                }
            }

            public override string StateCommand => StateCommandEnum.ToMove.ToString();

            public override bool Loop { get; set; } = false;

            public override bool Default => false;
        }
    }
}
