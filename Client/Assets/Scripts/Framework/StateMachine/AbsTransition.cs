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
        private TransitionTypeEnum _transState = TransitionTypeEnum.Finish;

        public string Name { get; private set; }
        public AbsState FromState { get; private set; }
        public AbsState ToState { get; private set; }
        public AbsEntity Entity { get; private set; }
        public StateMachine Machine { get; private set; }
        public TransitionTypeEnum TransState { get { return _transState; } set { _transState = value; } }

        public AbsTransition(StateMachine machine, string name, AbsState formState, AbsState toState)
        {
            Name = name;
            Entity = machine.Entity;
            Machine = machine;
            _transState = TransitionTypeEnum.Transing;
            FromState = formState;
            ToState = toState;
        }

        public virtual bool IsCanTrans() { return true; }

        public TransitionTypeEnum ExcuteTrans(AbsState formState, AbsState toState)
        {
            FromState = formState;
            ToState = toState;
            _transState = ExcuteTransEx();
            return _transState;
        }

        public virtual TransitionTypeEnum ExcuteTransEx() { return TransitionTypeEnum.Finish; }
    }
}