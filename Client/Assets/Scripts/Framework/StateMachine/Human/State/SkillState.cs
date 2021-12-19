/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/05 23:00:44
** desc:  技能状态;
*********************************************************************************/

namespace Framework
{
    public partial class HumanStateMachine
    {
        public class SkillState : AbsState
        {
            public SkillState(AbsStateMachine machine) : base(machine)
            {

            }

            public override string StateName
            {
                get
                {
                    return StateNameEnum.Skill.ToString();
                }
            }

            public override string StateCommand => StateCommandEnum.ToSkill.ToString();

            public override bool Loop { get; set; } = false;

            public override bool Default => false;
        }
    }
}
