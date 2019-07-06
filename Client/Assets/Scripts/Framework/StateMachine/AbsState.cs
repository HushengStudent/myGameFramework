/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:46:02
** desc:  状态机状态基类;
*********************************************************************************/

namespace Framework
{
    public enum StateMachineState
    {
        Idel = 0,
        Walk = 1,
        Running = 2,

    }

    public abstract class AbsState
    {
        public virtual StateMachineState FsmStateType { get { return StateMachineState.Idel; } }

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
                if (Machine.CurTrans.TransState == StateMachineTransitionState.Ready)
                {
                    OnExitState(this);
                    Machine.CurTrans.TransState = StateMachineTransitionState.Transing;
                }
                StateMachineTransitionState state = Machine.CurTrans.ExcuteTrans(this, Machine.CurTrans.ToState);
                if (state == StateMachineTransitionState.Finish)
                {
                    Machine.CurrentState.OnEnterStateEx(Machine.CurTrans.ToState);
                    Machine.CurTrans = null;
                    Machine.CurTrans.TransState = StateMachineTransitionState.Ready;
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