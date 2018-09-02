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
    public enum StateTypeEnum
    {
        Idel = 0,
        Walk = 1,
        Running = 2,

    }

    public abstract class AbsState
    {
        private string _name;
        private AbsEntity _entity = null;
        private StateMachine _machine;

        public virtual StateTypeEnum FsmStateType { get { return StateTypeEnum.Idel; } }

        public string Name { get { return _name; } }
        public AbsEntity Entity { get { return _entity; } }
        public StateMachine Machine { get { return _machine; } }

        public AbsState(StateMachine machine, string name)
        {
            _name = name;
            _entity = machine.Entity;
            _machine = machine;
        }

        private void OnEnterState(AbsState lastState)
        {
            OnEnterStateEx(lastState);
            Machine.CurrentState = this;
        }
        protected virtual void OnEnterStateEx(AbsState lastState) { }

        private void OnExitState(AbsState nextState)
        {
            OnExitStateEx(nextState);
        }
        protected virtual void OnExitStateEx(AbsState nextState) { }

        public void Update(float interval)
        {
            if (Machine.CurTrans == null)
            {
                UpdateEx(interval);

            }
            else
            {
                OnExitState(Machine.CurTrans.ToState);
                TransitionTypeEnum state = Machine.CurTrans.ExcuteTrans(this, Machine.CurTrans.ToState);
                if (state == TransitionTypeEnum.Finish)
                {
                    Machine.CurrentState.OnEnterStateEx(this);
                    Machine.CurTrans = null;
                    Machine.CurTrans.TransState = TransitionTypeEnum.Transing;
                }
            }
        }
        protected virtual void UpdateEx(float interval) { }

        public void LateUpdate(float interval)
        {
            LateUpdateEx(interval);
        }
        protected virtual void LateUpdateEx(float interval) { }
    }
}