/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:22:57
** desc:  组件抽象基类
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsComponent : IPool
    {
        private long _id;
        private bool _enable = true;
        private bool _isLoaded = false;
        private AbsEntity _entity;
        private GameObject _componentGo = null;
        private Action<AbsComponent> _initCallBack;

        public long ID { get { return _id; } }
        public bool Enable { get { return _enable; } set { _enable = value; } }
        public bool IsLoaded { get { return _isLoaded; } set { _isLoaded = value; } }
        public AbsEntity Entity { get { return _entity; } }
        public GameObject ComponentGo { get { return _componentGo; } set { _componentGo = value; } }
        public Action<AbsComponent> InitCallBack { get { return _initCallBack; } set { _initCallBack = value; } }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }

        public virtual IEnumerator<float> OnLoad()
        {
            yield return Timing.WaitForOneFrame;
        }

        /// <summary>
        /// 异步初始化Component;
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="isUsePool">是否使用对象池</param>
        /// <returns></returns>
        public virtual void OnInit(AbsEntity entity, bool isUsePool)
        {
            _id = IdGenerater.GenerateId();
            _entity = entity;
            if (isUsePool)
            {

            }
            else
            {
                if (InitCallBack != null)
                {
                    InitCallBack(this);
                }
            }
        }

        public virtual void ResetComponent()
        {
            _id = 0;
            _entity = null;
            _enable = true;
            _componentGo = null;
            _initCallBack = null;
        }

        public virtual void OnGet(params System.Object[] args) { }
        public virtual void OnRelease() { }
    }
}
