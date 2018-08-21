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

        public void Init(string path)
        {
            _animator = ResourceMgr.Instance.LoadAssetSync<Animator>(AssetType.AnimeCtrl, Path.GetFileName(path));
            _animatorOverrideController = new AnimatorOverrideController();
            RuntimeAnimatorController controller = _animator.runtimeAnimatorController;
            _animatorOverrideController = controller as AnimatorOverrideController;
        }

        public void OverrideAnimationCli(string name, string path, bool autoPlay = true)
        {
            AnimationClip clip = ResourceMgr.Instance.LoadAssetSync<AnimationClip>(AssetType.AnimeClip, Path.GetFileName(path));
            _animatorOverrideController[name] = clip;
            _animator.runtimeAnimatorController = _animatorOverrideController;
            _animator.Rebind();
            if (autoPlay)
            {
                _animator.Play(name);
            }
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