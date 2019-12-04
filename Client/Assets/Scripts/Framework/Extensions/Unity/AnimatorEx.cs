/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/20 01:45:07
** desc:  AnimatorÀ©Õ¹;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// ×´Ì¬»ú;
    /// </summary>
    public class AnimatorEx
    {
        private Animator _animator = null;
        private AnimatorOverrideController _animatorOverrideController = null;
        private Dictionary<string, AnimationClip> _animationInfo = new Dictionary<string, AnimationClip>();

        private AssetBundleAssetProxy _runtimeAnimatorProxy;
        private AssetBundleAssetProxy _animationClipProxy;

        public void Initialize(Animator animator, string path)
        {
            _animator = animator;
            _runtimeAnimatorProxy = ResourceMgr.Instance.LoadAssetAsync(path);
            _runtimeAnimatorProxy.AddLoadFinishCallBack(() =>
            {
                RuntimeAnimatorController ctrl = _runtimeAnimatorProxy.GetUnityAsset<RuntimeAnimatorController>();
                if (ctrl)
                {
                    _animator.runtimeAnimatorController = ctrl;
                    _animatorOverrideController = new AnimatorOverrideController(ctrl);
                    _animator.Rebind();
                }
            });
        }

        public void OverrideAnimationClip(string name, string path, bool autoPlay = true)
        {
            _animationClipProxy = ResourceMgr.Instance.LoadAssetAsync(path);
            _animationClipProxy.AddLoadFinishCallBack(() =>
            {
                AnimationClip clip = _animationClipProxy.GetUnityAsset<AnimationClip>();
                if (_animatorOverrideController && clip)
                {
                    _animatorOverrideController[name] = clip;
                    _animator.runtimeAnimatorController = _animatorOverrideController;
                    _animationInfo[name] = clip;
                    //_animator.Rebind();

                    if (autoPlay)
                    {
                        _animator.Play(name, 0, 0f);
                    }
                }
            });
        }

        public void SetBool(string name, bool value)
        {

        }

        public void SetFloat(string name, float value)
        {

        }

        public void SetTrigger(string name)
        {

        }
    }
}