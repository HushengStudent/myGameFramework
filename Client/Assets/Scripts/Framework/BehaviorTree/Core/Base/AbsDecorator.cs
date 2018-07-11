/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/07/12 00:26:53
** desc:  实体节点父类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class AbsDecorator : AbsBehavior
    {
        protected AbsBehavior _nextNode = null;

        public AbsDecorator(Hashtable table) : base(table)
        {
            _nextNode = null;
        }

        public void Serialize(AbsBehavior node)
        {
            _nextNode = node;
        }
    }
}