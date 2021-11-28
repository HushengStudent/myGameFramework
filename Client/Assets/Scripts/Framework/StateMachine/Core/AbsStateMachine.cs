/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:45:00
** desc:  状态机基类;
*********************************************************************************/

using Framework.ECSModule;
using Framework.ResourceModule;
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

        private AbsAssetProxy _runtimeAnimatorProxy;

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
                var ctrl = _runtimeAnimatorProxy.GetUnityAsset<RuntimeAnimatorController>();
                if (ctrl)
                {
                    AnimatorOverrideController = new AnimatorOverrideController(ctrl);
                    Animator.runtimeAnimatorController = AnimatorOverrideController;
                    Animator.Rebind();
                    Enable = true;
                    TransToDefault();

                    //TODO
                    foreach (var state in StateList)
                    {
                        var str = string.Empty;
                        if (state.StateName == StateNameEnum.Idle.ToString())
                            str = "Assets/Bundles/Animation/Skeleton/Idle.anim";
                        if (state.StateName == StateNameEnum.Move.ToString())
                            str = "Assets/Bundles/Animation/Skeleton/Run.anim";
                        if (state.StateName == StateNameEnum.Skill.ToString())
                            str = "Assets/Bundles/Animation/Skeleton/Attack.anim";
                        if (state.StateName == StateNameEnum.Special.ToString())
                            str = "Assets/Bundles/Animation/Skeleton/Damage.anim";
                        if (state.StateName == StateNameEnum.Dead.ToString())
                            str = "Assets/Bundles/Animation/Skeleton/Death.anim";
                        state.Initialize(str);
                    }
                }
            });
        }

        public void UnInitialize()
        {
            Enable = false;
            for (var i = 0; i < StateList.Count; i++)
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
            for (var i = 0; i < StateList.Count; i++)
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
            for (var i = 0; i < StateList.Count; i++)
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