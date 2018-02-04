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

        public long ID { get { return _id; } }
        public AbsEntity Entity { get { return _entity; } }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }

        protected AbsComponent(AbsEntity entity)
        {
            _id = IdGenerater.GenerateId();
            _entity = entity;
        }

        protected virtual void ResetComponent()
        {
            _id = 0;
            _entity = null;
        }

        protected virtual void DestroyComponent()
        {
            _id = 0;
            _entity = null;
        }
    }
}
