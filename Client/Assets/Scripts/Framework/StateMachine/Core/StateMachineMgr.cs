/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:15:17
** desc:  状态机管理;
*********************************************************************************/

using Framework.ECSModule;
using System.Collections.Generic;

namespace Framework
{
    public class StateMachineMgr : MonoSingleton<StateMachineMgr>
    {
        private readonly Dictionary<AbsEntity, AbsStateMachine> _stateMachineDict
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
            if (!_stateMachineDict.TryGetValue(entity, out var machine))
            {
                machine = new HumanStateMachine(entity);
                machine.Initialize();
                _stateMachineDict[entity] = machine;
            }
            return machine;
        }

        public void RemoveStateMachine(AbsEntity entity)
        {
            if (_stateMachineDict.TryGetValue(entity, out var machine))
            {
                machine.UnInitialize();
                _stateMachineDict.Remove(entity);
            }
        }
    }
}
