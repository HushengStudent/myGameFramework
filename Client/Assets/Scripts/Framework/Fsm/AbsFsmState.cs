/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:46:02
** desc:  ×´Ì¬»ú×´Ì¬»ùÀà;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsFsmState
    {
        private string _name;
        private float _time;
        private BaseEntity _entity = null;
        private List<AbsFsmTransition> _toTransitionList = new List<AbsFsmTransition>();
        private AbsFsmState _lastState;
        private AbsFsmState _nextState;
        private FsmMachine _machine;
        private AbsFsmTransition _triggerTrans;
        private FsmTransitionEventHandler _transitionHandler = null;

        public string Name { get { return _name; } set { _name = value; } }
        public BaseEntity Entity { get { return _entity; } }
        public AbsFsmState LastState { get { return _lastState; } }
        public AbsFsmState NextState { get { return _nextState; } }
        public FsmMachine Machine { get { return _machine; } }
        public AbsFsmTransition TriggerTrans { get { return _triggerTrans; } set { _triggerTrans = value; } }
        public FsmTransitionEventHandler TransitionHandler { get { return _transitionHandler; } set { _transitionHandler = value; } }

        public AbsFsmState(FsmMachine machine, string name)
        {
            _name = name;
            _entity = machine.Entity;
            _machine = machine;
            _lastState = null;
            _nextState = null;
            _triggerTrans = null;
            _toTransitionList.Clear();
        }

        public void OnEnterState(AbsFsmState lastState)
        {
            _lastState = lastState;
            OnEnterStateEx(lastState);
        }
        protected abstract void OnEnterStateEx(AbsFsmState lastState);

        public virtual void OnExitState(AbsFsmState nextState)
        {
            _nextState = nextState;
            OnExitStateEx(nextState);
            _machine.CurrentState = nextState;
        }
        protected abstract void OnExitStateEx(AbsFsmState nextState);

        protected virtual void OnUpdate(float deltaTime)
        {
            if(_triggerTrans == null)
            {
                for (int i = 0; i < _toTransitionList.Count; i++)
                {
                    var trans = _toTransitionList[i];
                    if (trans.IsCanTrans())
                    {
                        _triggerTrans = trans;
                        _triggerTrans.TransState = FsmTransitionStateEnum.Transing;
                        OnExitState(trans.ToState);
                        break;
                    }
                }
            }
            if (_triggerTrans == null)
            {
                OnUpdateEx(deltaTime);
            }
            else
            {
                FsmTransitionStateEnum state = _triggerTrans.ExcuteTrans(this, _triggerTrans.ToState);
                if(state == FsmTransitionStateEnum.Finish)
                {
                    _triggerTrans.TransState = FsmTransitionStateEnum.Transing;
                    _machine.CurrentState.OnEnterState(this);
                    _triggerTrans = null;
                }
            }
        }
        protected abstract void OnUpdateEx(float deltaTime);

        protected virtual void OnLateUpdate(float deltaTime) { }

        public bool AddTransition(AbsFsmTransition trans)
        {
            var lastState = trans.FromState;
            var nextState = trans.ToState;
            if (lastState != this && nextState != this)
                return false;
            if (nextState == this)
            {
                return lastState.AddTransition(trans);
            }
            if (lastState == this)
            {
                _toTransitionList.Add(trans);
                return true;
            }
            return false;
        }

        public bool RemoveTransition(AbsFsmTransition trans)
        {
            var lastState = trans.FromState;
            var nextState = trans.ToState;
            if (lastState != this && nextState != this)
                return false;
            if (nextState == this)
            {
                return lastState.RemoveTransition(trans);
            }
            if (lastState == this && _toTransitionList.Contains(trans))
            {
                _toTransitionList.Remove(trans);
                return true;
            }
            return false;
        }

        public AbsFsmTransition GetTransByName(string transName)
        {
            for (int i = 0; i < _toTransitionList.Count; i++)
            {
                var target = _toTransitionList[i];
                if (target.Name == transName)
                {
                    return target;
                }
            }
            return null;
        }
    }
}