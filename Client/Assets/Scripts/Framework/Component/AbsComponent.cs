/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:22:57
** desc:  组件抽象基类
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public abstract class AbsComponent
    {
        private ulong id;
        public ulong ID
        {
            get { return id; }set { id = value; }
        }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }

        public abstract void CreateComponent();
        public abstract void DestroyComponent();

    }
}
