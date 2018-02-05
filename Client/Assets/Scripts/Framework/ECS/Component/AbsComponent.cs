/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:22:57
** desc:  组件抽象基类
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsComponent
    {
        private long _id;
        private AbsEntity _entity;
        private bool _enable = true;
        private Action<AbsComponent> _initCallBack;

        public long ID { get { return _id; } }
        public AbsEntity Entity { get { return _entity; } }
        public bool Enable { get { return _enable; } set { _enable = value; } }
        public Action<AbsComponent> InitCallBack { get { return _initCallBack; } set { _initCallBack = value; } }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }
        public virtual void OnInit(AbsEntity entity)
        {
            //TODO:use pool and async;
            _id = IdGenerater.GenerateId();
            _entity = entity;
            if (InitCallBack != null)
            {
                InitCallBack(this);
            }
        }
        public virtual void ResetComponent()
        {
            _id = 0;
            _entity = null;
            _enable = true;
            _initCallBack = null;
        }
    }
}
