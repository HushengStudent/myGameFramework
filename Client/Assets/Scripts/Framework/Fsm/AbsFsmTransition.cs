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
        private FsmTransitionStateEnum _transState = FsmTransitionStateEnum.Transing;
        private FsmTransitionEventHandler _transitionHandler = null;

        public string Name { get { return _name; } set { _name = value; } }
        public AbsFsmState FromState { get { return _formState; } set { _formState = value; } }
        public AbsFsmState ToState { get { return _toState; } set { _toState = value; } }
        public AbsEntity Entity { get { return _entity; } }
        public FsmMachine Machine { get { return _machine; } set { _machine = value; } }
        public FsmTransitionStateEnum TransState { get { return _transState; } set { _transState = value; } }
        public FsmTransitionEventHandler TransitionHandler { get { return _transitionHandler; } set { _transitionHandler = value; } }

        public AbsFsmTransition(FsmMachine machine, string name)
        {
            _name = name;
            _entity = machine.Entity;
            _machine = machine;
            _transState = FsmTransitionStateEnum.Transing;
            _transitionHandler = null;
            _formState = null;
            _toState = null;
        }

        public abstract bool IsCanTrans();

        public FsmTransitionStateEnum ExcuteTrans(AbsFsmState formState, AbsFsmState toState)
        {
            _formState = formState;
            _toState = toState;
            if (ExcuteTransEx() == FsmTransitionStateEnum.Transing)
            {
                return FsmTransitionStateEnum.Transing;
            }
            if (TransitionHandler != null)
            {
                TransitionHandler(_formState, _toState);
            }
            return FsmTransitionStateEnum.Finish;
        }

        public abstract FsmTransitionStateEnum ExcuteTransEx();
    }
}