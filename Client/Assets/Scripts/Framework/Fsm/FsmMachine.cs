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
    public sealed class FsmMachine
    {
        private string _name;
        private AbsEntity _entity = null;
        private AbsFsmState _defaultState = null;
        private List<AbsFsmState> _stateList = new List<AbsFsmState>();
        private AbsFsmTransition _curTrans = null;
        private List<AbsFsmTransition> _transitionList = new List<AbsFsmTransition>();

        public bool Enable { get; set; }
        public string Name { get { return _name; } }
        public AbsEntity Entity { get { return _entity; } }
        public AbsFsmState CurrentState { get; set; }
        public AbsFsmState DefaultState { get; set; }
        public List<AbsFsmState> StateList { get { return _stateList; } }
        public AbsFsmTransition CurTrans { get { return _curTrans; } set { _curTrans = value; } }
        public List<AbsFsmTransition> TransitionList { get { return _transitionList; } }

        public FsmMachine(AbsEntity entity, string name,
            List<AbsFsmState> stateList, AbsFsmState defaultState, List<AbsFsmTransition> transitionList)
        {
            Enable = false;
            _name = name;
            _curTrans = null;
            _entity = entity;
            _stateList.Clear();
            _transitionList.Clear();
            _stateList = stateList;
            _defaultState = defaultState;
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

        private bool OnChangeState(AbsFsmState fromState, AbsFsmState toState)
        {
            if (CurTrans == null)
            {
                foreach (var target in TransitionList)
                {
                    if (target.FromState == fromState && target.ToState == toState && target.TransState == FsmTransitionStateEnum.Finish && target.IsCanTrans())
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