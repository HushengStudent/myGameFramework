/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:45:00
** desc:  ×´Ì¬»ú;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public sealed class StateMachine
    {
        private AbsState _defaultState = null;
        private List<AbsState> _stateList = new List<AbsState>();
        private List<AbsTransition> _transitionList = new List<AbsTransition>();

        public bool Enable { get; set; }
        public string Name { get; private set; }
        public AbsEntity Entity { get; private set; }
        public AbsState CurrentState { get; set; }
        public AbsState DefaultState { get; set; }
        public List<AbsState> StateList { get { return _stateList; } }
        public AbsTransition CurTrans { get; set; }
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

        private bool OnChangeState(AbsState fromState, AbsState toState)
        {
            if (CurTrans == null)
            {
                foreach (var target in TransitionList)
                {
                    if (target.FromState == fromState && target.ToState == toState && target.TransState == TransitionTypeEnum.Finish && target.IsCanTrans())
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