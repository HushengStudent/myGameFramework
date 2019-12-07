/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:45:00
** desc:  状态机基类;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum StateNameEnum
    {
        Idle,
        Move,
        Dead,
        Skill,
        Special,
    }

    public enum StateCommandEnum
    {
        ToIdle,
        ToMove,
        ToDead,
        ToSkill,
        ToSpecial,
    }

    public abstract class AbsStateMachine
    {
        public bool Enable;
        public AbsState CurrentState { get; set; }
        public AbsEntity Entity { get; private set; }
        public Animator Animator { get; private set; }
        public AnimatorOverrideController AnimatorOverrideController { get; private set; }

        protected List<AbsState> StateList { get; private set; }

        private AssetBundleAssetProxy _runtimeAnimatorProxy;

        public abstract string AssetPath { get; }

        public AbsStateMachine(AbsEntity entity)
        {
            Enable = false;
            Entity = entity;
            StateList = new List<AbsState>();
            CurrentState = null;
        }

        public void Initialize()
        {
            if (null == Entity)
            {
                return;
            }
            Animator = Entity.Animator;
            _runtimeAnimatorProxy = ResourceMgr.singleton.LoadAssetAsync(AssetPath);
            _runtimeAnimatorProxy.AddLoadFinishCallBack(() =>
            {
                RuntimeAnimatorController ctrl = _runtimeAnimatorProxy.GetUnityAsset<RuntimeAnimatorController>();
                if (ctrl)
                {
                    Animator.runtimeAnimatorController = ctrl;
                    AnimatorOverrideController = new AnimatorOverrideController(ctrl);
                    Animator.Rebind();
                    Enable = true;
                    TransToDefault();
                }
            });
        }

        public void UnInitialize()
        {
            Enable = false;
            for (int i = 0; i < StateList.Count; i++)
            {
                var state = StateList[i];
                state.UnInitialize();
            }
            CurrentState = null;
            Entity = null;
            if (_runtimeAnimatorProxy != null)
            {
                _runtimeAnimatorProxy.UnloadProxy();
            }
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

        public void TransToDefault()
        {
            for (int i = 0; i < StateList.Count; i++)
            {
                var state = StateList[i];
                if (state.Default)
                {
                    state.OnEnterState();
                    return;
                }
            }
            LogHelper.PrintError($"[AbsStateMachine]Not find default state.");
        }

        public void ExcuteCommand(string command, bool loop = false)
        {
            if (command == StateNameEnum.Idle.ToString())
            {
                loop = true;
            }
            for (int i = 0; i < StateList.Count; i++)
            {
                var state = StateList[i];
                if (state.StateCommand == command)
                {
                    state.Loop = loop;
                    state.OnEnterState();
                    return;
                }
            }
            LogHelper.PrintError($"[AbsStateMachine]Not find state to excute command:{command}.");
        }
    }
}