/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/07/11 00:25:08
** desc:  抽象节点父类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsComposite : AbsBehavior
    {
        public AbsComposite(Hashtable table) : base(table) { }

        public abstract void Serialize(List<AbsBehavior> behaviorList);
    }
}