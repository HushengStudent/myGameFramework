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
    public enum FsmStateTypeEnum
    {
        Idel = 0,
        Walk = 1,
        Running = 2,

    }

    public abstract class AbsFsmState
    {
        private string _name;
        private AbsEntity _entity = null;
        private FsmMachine _machine;

        public virtual FsmStateTypeEnum FsmStateType { get { return FsmStateTypeEnum.Idel; } }

        public string Name { get { return _name; } }
        public AbsEntity Entity { get { return _entity; } }
        public FsmMachine Machine { get { return _machine; } }

        public AbsFsmState(FsmMachine machine, string name)
        {
            _name = name;
            _entity = machine.Entity;
            _machine = machine;
        }

        private void OnEnterState(AbsFsmState lastState)
        {
            OnEnterStateEx(lastState);
            Machine.CurrentState = this;
        }
        protected virtual void OnEnterStateEx(AbsFsmState lastState) { }

        private void OnExitState(AbsFsmState nextState)
        {
            OnExitStateEx(nextState);
        }
        protected virtual void OnExitStateEx(AbsFsmState nextState) { }

        public void Update(float interval)
        {
            if (Machine.CurTrans == null)
            {
                UpdateEx(interval);

            }
            else
            {
                OnExitState(Machine.CurTrans.ToState);
                FsmTransitionStateEnum state = Machine.CurTrans.ExcuteTrans(this, Machine.CurTrans.ToState);
                if (state == FsmTransitionStateEnum.Finish)
                {
                    Machine.CurrentState.OnEnterStateEx(this);
                    Machine.CurTrans = null;
                    Machine.CurTrans.TransState = FsmTransitionStateEnum.Transing;
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