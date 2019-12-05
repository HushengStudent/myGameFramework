/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:15:17
** desc:  状态机管理;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework
{
    public class StateMachineMgr : MonoSingleton<StateMachineMgr>
    {
        private Dictionary<AbsEntity, AbsStateMachine> _stateMachineDict
            = new Dictionary<AbsEntity, AbsStateMachine>();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _stateMachineDict.Clear();
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            foreach (var target in _stateMachineDict)
            {
                target.Value.Update(interval);
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            foreach (var target in _stateMachineDict)
            {
                target.Value.LateUpdate(interval);
            }
        }

        public AbsStateMachine CreateStateMachine(AbsEntity entity)
        {
            AbsStateMachine machine = null;
            if (!_stateMachineDict.TryGetValue(entity, out machine))
            {
                machine = new HumanStateMachine(entity);
                machine.Initialize();
            }
            return machine;
        }

        public void RemoveStateMachine(AbsEntity entity)
        {
            AbsStateMachine machine = null;
            if (_stateMachineDict.TryGetValue(entity, out machine))
            {
                machine.UnInitialize();
            }
        }
    }
}
