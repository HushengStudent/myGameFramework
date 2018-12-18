/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:46:02
** desc:  状态机状态基类;
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
        public virtual StateTypeEnum FsmStateType { get { return StateTypeEnum.Idel; } }

        public string Name { get; private set; }
        public AbsEntity Entity { get; private set; }
        public StateMachine Machine { get; private set; }

        public AbsState(StateMachine machine, string name)
        {
            Name = name;
            Entity = machine.Entity;
            Machine = machine;
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