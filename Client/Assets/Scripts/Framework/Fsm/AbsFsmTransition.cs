/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:47:11
** desc:  状态机状态转换基类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum FsmTransitionStateEnum
    {
        Transing = 0,
        Finish = 1
    }

    public abstract class AbsFsmTransition
    {
        private string _name;
        private AbsFsmState _formState;
        private AbsFsmState _toState;
        private AbsEntity _entity = null;
        private FsmMachine _machine;
        private FsmTransitionStateEnum _transState = FsmTransitionStateEnum.Finish;

        public string Name { get { return _name; } }
        public AbsFsmState FromState { get { return _formState; } }
        public AbsFsmState ToState { get { return _toState; } }
        public AbsEntity Entity { get { return _entity; } }
        public FsmMachine Machine { get { return _machine; } }
        public FsmTransitionStateEnum TransState { get { return _transState; } set { _transState = value; } }

        public AbsFsmTransition(FsmMachine machine, string name, AbsFsmState formState, AbsFsmState toState)
        {
            _name = name;
            _entity = machine.Entity;
            _machine = machine;
            _transState = FsmTransitionStateEnum.Transing;
            _formState = formState;
            _toState = toState;
        }

        public virtual bool IsCanTrans() { return true; }

        public FsmTransitionStateEnum ExcuteTrans(AbsFsmState formState, AbsFsmState toState)
        {
            _formState = formState;
            _toState = toState;
            _transState = ExcuteTransEx();
            return _transState;
        }

        public virtual FsmTransitionStateEnum ExcuteTransEx() { return FsmTransitionStateEnum.Finish; }
    }
}