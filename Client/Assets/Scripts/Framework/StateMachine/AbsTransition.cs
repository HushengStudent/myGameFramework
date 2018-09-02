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
    public enum TransitionTypeEnum
    {
        Transing = 0,
        Finish = 1
    }

    public abstract class AbsTransition
    {
        private string _name;
        private AbsState _formState;
        private AbsState _toState;
        private AbsEntity _entity = null;
        private StateMachine _machine;
        private TransitionTypeEnum _transState = TransitionTypeEnum.Finish;

        public string Name { get { return _name; } }
        public AbsState FromState { get { return _formState; } }
        public AbsState ToState { get { return _toState; } }
        public AbsEntity Entity { get { return _entity; } }
        public StateMachine Machine { get { return _machine; } }
        public TransitionTypeEnum TransState { get { return _transState; } set { _transState = value; } }

        public AbsTransition(StateMachine machine, string name, AbsState formState, AbsState toState)
        {
            _name = name;
            _entity = machine.Entity;
            _machine = machine;
            _transState = TransitionTypeEnum.Transing;
            _formState = formState;
            _toState = toState;
        }

        public virtual bool IsCanTrans() { return true; }

        public TransitionTypeEnum ExcuteTrans(AbsState formState, AbsState toState)
        {
            _formState = formState;
            _toState = toState;
            _transState = ExcuteTransEx();
            return _transState;
        }

        public virtual TransitionTypeEnum ExcuteTransEx() { return TransitionTypeEnum.Finish; }
    }
}