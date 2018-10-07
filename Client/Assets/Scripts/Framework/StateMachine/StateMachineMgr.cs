/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:15:17
** desc:  状态机管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class StateMachineMgr : MonoSingleton<StateMachineMgr>
    {
        private Dictionary<AbsEntity, StateMachine> _fsmDict = new Dictionary<AbsEntity, StateMachine>();

        public override void Init()
        {
            base.Init();
            _fsmDict.Clear();
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            foreach (var target in _fsmDict)
            {
                target.Value.Update(interval);
            }
        }

        protected override void LateUpdateEx(float interval)
        {
            base.LateUpdateEx(interval);
            foreach (var target in _fsmDict)
            {
                target.Value.LateUpdate(interval);
            }
        }

        public void CreateFsmMachine(AbsEntity entity, string name,
            List<AbsState> stateList, AbsState defaultState, List<AbsTransition> transitionList)
        {
            StateMachine fsm = new StateMachine(entity, name, stateList, defaultState, transitionList);
            _fsmDict.Add(entity, fsm);
        }
    }
}
