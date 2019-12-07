/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:46:02
** desc:  状态机状态基类;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public abstract class AbsState
    {
        private float _stateStartTicks;
        private float _animationLength;
        private string _animationPath;
        private AnimationClip _animationClip;
        private AssetBundleAssetProxy _animationClipProxy;

        public AbsEntity Entity { get; private set; }
        public AbsStateMachine Machine { get; private set; }

        public abstract string StateName { get; }
        public abstract string StateCommand { get; }
        public abstract bool Loop { get; set; }
        public abstract bool Default { get; }

        public AbsState(AbsStateMachine machine)
        {
            Entity = machine.Entity;
            Machine = machine;
        }

        public void Initialize(string path)
        {
            OverrideAnimationClip(path);
        }

        public void UnInitialize()
        {
            _animationClip = null;
            _stateStartTicks = 0;
            if (_animationClipProxy != null)
            {
                _animationClipProxy.UnloadProxy();
            }
        }

        public void OnEnterState()
        {
            if (Machine.CurrentState == this)
            {
                return;
            }
            OnExitState();
            Machine.CurrentState = this;
            Machine.Animator.SetTrigger(StateCommand);
            OnEnterStateEx();
        }

        private void OnExitState()
        {
            OnExitStateEx();
        }

        public void Update(float interval)
        {
            if (_animationClip == null && Machine.AnimatorOverrideController != null)
            {
                _animationClip = Machine.AnimatorOverrideController[StateName];
            }
            if (_animationClip)
            {
                if (_stateStartTicks <= 0)
                {
                    _animationLength = _animationClip.length;
                    _stateStartTicks = Time.realtimeSinceStartup;
                    Machine.Animator.Play(StateName, 0, 0f);
                }
                var pass = Time.realtimeSinceStartup - _stateStartTicks;
                if (pass - 0.2 >= _animationLength)
                {
                    _stateStartTicks = 0;
                    if (!Loop)
                    {
                        Machine.TransToDefault();
                    }
                }

            }
            UpdateEx(interval);
        }

        public void LateUpdate(float interval)
        {
            LateUpdateEx(interval);
        }

        protected virtual void OnEnterStateEx() { }
        protected virtual void OnExitStateEx() { }
        protected virtual void UpdateEx(float interval) { }
        protected virtual void LateUpdateEx(float interval) { }

        public void OverrideAnimationClip(string path)
        {
            if (_animationPath == path)
            {
                return;
            }
            _animationPath = path;
            if (_animationClipProxy != null)
            {
                _animationClipProxy.UnloadProxy();
            }
            _animationClipProxy = ResourceMgr.singleton.LoadAssetAsync(path);
            _animationClipProxy.AddLoadFinishCallBack(() =>
            {
                AnimationClip clip = _animationClipProxy.GetUnityAsset<AnimationClip>();
                if (Machine.AnimatorOverrideController && clip)
                {
                    Machine.AnimatorOverrideController[StateName] = clip;
                    Machine.Animator.runtimeAnimatorController = Machine.AnimatorOverrideController;
                    //_animator.Rebind();

                    _animationClip = clip;
                }
            });
        }
    }
}