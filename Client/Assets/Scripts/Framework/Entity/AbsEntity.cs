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
        private ulong id;
        public ulong ID
        {
            get { return id; }
            set { id = value; }
        }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }

        public abstract void CreateEntity();
        public abstract void DestroyEntity();
    }
}
