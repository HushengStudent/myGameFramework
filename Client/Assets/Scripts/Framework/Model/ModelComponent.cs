/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/02/05 17:45:37
** desc:  模型组件;
*********************************************************************************/

using Framework.ECSModule;
using System;
using UnityEngine;

namespace Framework
{
    public abstract class ModelComponent : AbsComponent
    {
        private Action _onLoadFinishHandler;
        public Action OnLoadFinishHandler
        {
            get
            {
                return _onLoadFinishHandler;
            }
            set
            {
                _onLoadFinishHandler = value;
                if (IsInit)
                {
                    value?.Invoke();
                }
            }
        }

        public GameObject GameObject;
        public bool IsInit { get; protected set; }

        protected override void InitializeEx()
        {
            base.InitializeEx();
        }

        protected override void UnInitializeEx()
        {
            base.UnInitializeEx();
            IsInit = false;
            GameObject = null;
            OnLoadFinishHandler = null;
        }

        protected void OnLoadFinish()
        {
            OnLoadFinishEx();
            IsInit = true;
            OnLoadFinishHandler?.Invoke();
        }

        protected virtual void OnLoadFinishEx() { }
    }
}