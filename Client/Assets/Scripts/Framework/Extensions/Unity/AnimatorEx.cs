/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/20 01:45:07
** desc:  Animator¿©’π;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public class AnimatorEx
    {
        private Animator _animator = null;
        private AnimatorOverrideController _animatorOverrideController = null;
        private DictEx<string, AnimationClip> _AnimationInfo = new DictEx<string, AnimationClip>();

        private AssetAsyncProxy _runtimeAnimatorProxy;
        private AssetAsyncProxy _animationClipProxy;

        public void Init(Animator animator, string path)
        {
            _animator = animator;
            _runtimeAnimatorProxy = ResourceMgr.Instance.LoadAssetProxy(AssetType.AnimeCtrl, path
                , (obj) =>
            {
                RuntimeAnimatorController ctrl = obj as RuntimeAnimatorController;
                if (ctrl)
                {
                    _animator.runtimeAnimatorController = ctrl;
                    _animatorOverrideController = ctrl as AnimatorOverrideController;
                    _animator.Rebind();
                }
            });
        }

        public void OverrideAnimationClip(string name, string path, bool autoPlay = true)
        {
            _animationClipProxy = ResourceMgr.Instance.LoadAssetProxy(AssetType.AnimeClip, path
                , (obj) =>
                {
                    AnimationClip clip = obj as AnimationClip;
                    if (_animatorOverrideController && clip)
                    {
                        _animatorOverrideController[name] = clip;
                        _animator.runtimeAnimatorController = _animatorOverrideController;
                        _AnimationInfo.Data[name] = clip;
                        _animator.Rebind();
                        if (autoPlay)
                        {
                            _animator.Play(name);
                        }
                    }
                });
        }

        public void SetBool(int id, bool value)
        {

        }

        public void SetBool(string name, bool value)
        {

        }

        public void SetFloat(int id, float value)
        {

        }

        public void SetFloat(string name, float value)
        {

        }

        public void SetTrigger(int id)
        {

        }

        public void SetTrigger(string name)
        {

        }
    }
}