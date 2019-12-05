/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/05 23:01:04
** desc:  ÌØÊâ×´Ì¬;
*********************************************************************************/

namespace Framework
{
    public partial class HumanStateMachine
    {
        public class SpecialState : AbsState
        {
            public SpecialState(AbsStateMachine machine) : base(machine)
            {

            }

            public override string StateName
            {
                get
                {
                    return StateNameEnum.Special.ToString();
                }
            }

            public override string StateCommand => StateCommandEnum.ToSpecial.ToString();

            public override bool Loop { get; set; } = false;

            public override bool Default => false;
        }
    }
}