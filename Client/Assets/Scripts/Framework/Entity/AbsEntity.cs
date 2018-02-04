/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/08 00:32:30
** desc:  实体抽象基类
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsEntity
    {
        private long _id;
        public long ID { get { return _id; } }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }

        protected AbsEntity()
        {
            _id = IdGenerater.GenerateId();
        }

        protected virtual void ResetEntity()
        {
            _id = 0;
        }

        protected virtual void DestroyEntity()
        {
            _id = 0;
        }
    }
}
