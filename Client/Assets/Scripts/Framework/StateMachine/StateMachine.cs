/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:45:00
** desc:  状态机;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework
{
    public sealed class StateMachine
    {
        private AbsState _defaultState;
        private List<AbsState> _stateList = new List<AbsState>();
        private List<AbsTransition> _transitionList = new List<AbsTransition>();

        public bool Enable;
        public AbsState CurrentState;
        public AbsState DefaultState;
        public AbsTransition CurTrans;

        public string Name { get; private set; }
        public AbsEntity Entity { get; private set; }
        public List<AbsState> StateList { get { return _stateList; } }
        public List<AbsTransition> TransitionList { get { return _transitionList; } }

        public StateMachine(AbsEntity entity, string name,
            List<AbsState> stateList, AbsState defaultState, List<AbsTransition> transitionList)
        {
            Enable = false;
            Name = name;
            Entity = entity;
            _stateList.Clear();
            _transitionList.Clear();
            _stateList = stateList;
            _defaultState = defaultState;
            CurTrans = null;
            CurrentState = _defaultState;
            _transitionList = transitionList;
        }

        public void Update(float interval)
        {
            if (Enable)
            {
                if (CurrentState != null)
                {
                    CurrentState.Update(interval);
                }
            }
        }

        public void LateUpdate(float interval)
        {
            if (Enable)
            {
                if (CurrentState != null)
                {
                    CurrentState.LateUpdate(interval);
                }
            }
        }

        public bool OnChangeState(AbsState fromState, AbsState toState)
        {
            if (CurTrans == null)
            {
                foreach (var target in TransitionList)
                {
                    if (target.FromState == fromState && target.ToState == toState && target.TransState == StateMachineTransitionState.Finish && target.IsCanTrans())
                    {
                        CurTrans = target;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}