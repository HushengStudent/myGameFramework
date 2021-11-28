/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/05 23:02:39
** desc:  ÈËÐÎ×´Ì¬»ú;
*********************************************************************************/

using Framework.ECSModule;

namespace Framework
{
    public partial class HumanStateMachine : AbsStateMachine
    {
        public HumanStateMachine(AbsEntity entity) : base(entity)
        {
            StateList.Add(new IdleState(this));
            StateList.Add(new MoveState(this));
            StateList.Add(new SkillState(this));
            StateList.Add(new SpecialState(this));
            StateList.Add(new DeadState(this));
        }

        public override string AssetPath
        {
            get
            {
                return "Assets/Bundles/Animator/Common/Player.controller";
            }
        }
    }
}